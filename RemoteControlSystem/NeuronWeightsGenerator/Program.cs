﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AForge.Neuro;
using AForge.Neuro.Learning;
using ExtendedLibrary;

namespace NeuronWeightsGenerator
{
    public class Program
    {
        public const string SamplesFile = "../../../NeuronWeightsGenerator/Samples.txt";
        public const string WeightsFile = "../../../NeuronWeightsGenerator/Weights.txt";
        public const string NetworkFile = "../../../NeuronWeightsGenerator/Network.bin";

        public const int InputCount = 2;
        public const int OutputCount = 4;
        private readonly List<double[]> _inputList = new List<double[]>();
        private readonly List<double[]> _outputList = new List<double[]>();

        private bool _isUseElementaryTraining = false;

        #region Constants

        protected const int DR_MIN = Helper.DrMin + 2; // = 87.
        protected const int DR_MAX = Helper.DrMax;

        protected const int UD = 127;
        protected const int RLR = 128;
        protected const int FB = 127;
        protected const int LR = 128;

        #endregion

        private static void Main(string[] args)
        {
            new Program().Worker();
        }

        private void Worker()
        {
            ExtendedIOHelpers.ShowAlert("Generate new samples before training? y/n  ", () => GenerateSamples());

            if (!LoadSamples())
            {
                return;
            }

            Console.WriteLine("Loaded {0} samples", _inputList.Count);

            var network = new ActivationNetwork(new SigmoidFunction(1), InputCount, OutputCount);
            var teacher = new BackPropagationLearning(network)
            {
                LearningRate = 0.01
            };

            Console.WriteLine("Start training the network.");

            int iteration = 0;
            const int iterations = 5000;
            double error = 1.0;
            var st = new Stopwatch();
            st.Start();

            while (iteration < iterations && error > 0.00005)
            {
                error = teacher.RunEpoch(_inputList.ToArray(), _outputList.ToArray()) / _inputList.Count;
                iteration++;
            }

            var time = st.ElapsedMilliseconds / 1000.0;
            st.Stop();
            Console.WriteLine("Network successfully trained! Error = {0:0.######}, Iteration = {1}", error, iteration);
            Console.WriteLine("Time = {0:0.000} s\n", time);

            // Normalize weights and convert to string format.
            var weights = network.Layers
                .Select(layer => layer.Neurons
                    .Select(neuron => neuron.Weights
                        .Select(x => string.Format("{0,6}", Convert.ToInt32(x * 1000.0)))));

            ExtendedIOHelpers.ShowAlert("Do you want to save network to file? y/n  ", () =>
            {
                SaveWeights(weights);
                network.Save(NetworkFile);
            });

            Console.ReadLine();
        }

        private bool GenerateSamples()
        {
            using (var stream = new StreamWriter(SamplesFile))
            {
                try
                {
                    //  1   2
                    //    x
                    //  4   3
                    //
                    // Up/Down 0-127-255
                    // Rotate Left/Right 0-128-255
                    // Forward/Back 0-127-255
                    // Left/Right 0-128-255
                    //
                    // inputs: up, down, rLeft, rRight, forward, back, left, right
                    // outputs: 4

                    if (_isUseElementaryTraining)
                    {
                        #region Elementary sample

                        const double incK = 0.1;

                        // Forward 127-0
                        for (int value = FB; value >= 0; value--)
                        {
                            var engine = DR_MIN + (FB - value) * incK;
                            //WriteLine(stream, UD, UD, RLR, RLR, value, FB, LR, LR, DR_MIN, DR_MIN, engine, engine);
                            WriteLine(stream, value, LR, DR_MIN, DR_MIN, engine, engine);
                        }

                        // Back 127-255
                        for (int value = FB; value <= 255; value++)
                        {
                            var engine = DR_MIN + (value - FB) * incK;
//                        WriteLine(stream, UD, UD, RLR, RLR, FB, value, LR, LR, engine, engine, DR_MIN, DR_MIN);
                            WriteLine(stream, value, LR, engine, engine, DR_MIN, DR_MIN);
                        }

                        // Left 128-0
                        for (int value = LR; value >= 0; value--)
                        {
                            var engine = DR_MIN + (LR - value) * incK;
//                        WriteLine(stream, UD, UD, RLR, RLR, FB, FB, value, LR, DR_MIN, engine, engine, DR_MIN);
                            WriteLine(stream, FB, value, DR_MIN, engine, engine, DR_MIN);
                        }

                        // Right 128-255
                        for (int value = LR; value <= 255; value++)
                        {
                            var engine = DR_MIN + (value - LR) * incK;
//                        WriteLine(stream, UD, UD, RLR, RLR, FB, FB, LR, value, engine, DR_MIN, DR_MIN, engine);
                            WriteLine(stream, FB, value, engine, DR_MIN, DR_MIN, engine);
                        }

                        #endregion
                    }
                    else
                    {
                        #region Advanced sample

                        double incK = 0.05;

                        const int speed = 5;
                        const int radiusK = 2 * speed;  // 10
                        const double angleK = 1.0 * speed;  // 5 degree

                        for (int r = 10; r < 128;)
                        {
                            for (double a = 0.0; a < 360; a += angleK)
                            {
                                double b = a < 45 ? 315 + a : a - 45;  // lag on 45 degrees

                                double aR = a / 180.0 * Math.PI;
                                double bR = b / 180.0 * Math.PI;

                                double fbJ = FB - r * Math.Sin(aR);
                                double lrJ = LR + r * Math.Cos(aR);

                                double en1 = DR_MIN;
                                double en2 = DR_MIN;
                                double en3 = DR_MIN;
                                double en4 = DR_MIN;

                                if (0 <= b && b < 90)
                                {
                                    en4 += r * incK * Math.Cos(bR);
                                    en3 += r * incK * Math.Sin(bR);
                                }
                                else if (90 <= b && b < 180)
                                {
                                    en3 += r * incK * Math.Sin(bR);
                                    en2 -= r * incK * Math.Cos(bR);
                                }
                                else if (180 <= b && b < 270)
                                {
                                    en2 -= r * incK * Math.Cos(bR);
                                    en1 -= r * incK * Math.Sin(bR);
                                }
                                else
                                {
                                    en1 -= r * incK * Math.Sin(bR);
                                    en4 += r * incK * Math.Cos(bR);
                                }

                                WriteLine(stream, fbJ, lrJ, en1, en2, en3, en4);
                            }

                            if (r == 127)
                            {
                                break;
                            }

                            r = 128 - r > radiusK ? r + radiusK : 127;
                            //incK += 0.015 * speed / 5.0;
                        }

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("GenerateSamples error: {0}", ex.Message);
                    return false;
                }
            }

            return true;
        }

        private void WriteLine(StreamWriter stream, params object[] arg)
        {
//            stream.WriteLine("{0:000} {1:000} {2:000} {3:000} {4:000} {5:000} {6:000} {7:000} {8:000} {9:000} {10:000} {11:000}",
//                arg[0], arg[1], arg[2], arg[3], arg[4], arg[5], arg[6], arg[7], arg[8], arg[9], arg[10], arg[11]);
            stream.WriteLine("{0:000};{1:000};{2:000};{3:000};{4:000};{5:000}",
                Round(arg[0]), Round(arg[1]), Round(arg[2]), Round(arg[3]), Round(arg[4]), Round(arg[5]));
        }

        private int Round(object d)
        {
            return (int) Math.Round(Convert.ToDouble(d), 0);
        }

        private bool LoadSamples()
        {
            using (var stream = new StreamReader(SamplesFile))
            {
                try
                {
                    while (!stream.EndOfStream)
                    {
                        var array = stream.ReadLine().Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                        var inp = array.Take(InputCount).Select(x => double.Parse(x) / 255.0).ToArray();
                        var outp =
                            array.Skip(InputCount)
                                .Take(OutputCount)
                                .Select(x => Helper.MapJoystickValueToNetwork(byte.Parse(x)))
                                .ToArray();
                        _inputList.Add(inp);
                        _outputList.Add(outp);
                    }

                    _inputList.AddRange(_inputList.ToArray().Reverse());
                    _outputList.AddRange(_outputList.ToArray().Reverse());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("LoadSamples error: {0}", ex.Message);
                    return false;
                }
            }

            return true;
        }

        private bool SaveWeights(IEnumerable<IEnumerable<IEnumerable<string>>> weights)
        {
            using (var stream = new StreamWriter(WeightsFile))
            {
                try
                {
                    int num = 0;
                    stream.WriteLine("int Weights[] = {");

                    foreach (var layerWeights in weights)
                    {
                        stream.WriteLine("    // w{0}", num++);

                        foreach (var neuronWeights in layerWeights)
                        {
                            var row = string.Join(", ", neuronWeights);
                            stream.WriteLine("    {0},", row);
                        }
                    }

                    stream.WriteLine("};");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("GenerateSamples error: {0}", ex.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
