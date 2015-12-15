using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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

        protected const int DR_MIN = 87;
        protected const int DR_MAX = 160;

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

            var network = new ActivationNetwork(new SigmoidFunction(1), InputCount, 8, 8, OutputCount);
            var teacher = new BackPropagationLearning(network)
            {
                Momentum = 0.1
            };

            Console.WriteLine("Start training the network.");

            int iteration = 0;
            const int iterations = 5000;
            double error = 1.0;

            while (iteration < iterations && error > 0.00005)
            {
                error = teacher.RunEpoch(_inputList.ToArray(), _outputList.ToArray()) / _inputList.Count;
                iteration++;
            }

            Console.WriteLine("Network successfully trained! Error = {0:0.######}, Iteration = {1}\n", error, iteration);

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

                        const double incK = 0.1;
                        const int radiusK = 10;
                        const double angleK = Math.PI / 180.0; // in radians.

                        for (int r = radiusK; r < 128;)
                        {
                            for (double a = 0.0; a < 2.0 * Math.PI; a += angleK)
                            {
                                double b = a - Math.PI / 4.0;

                                double fbJ = FB - r * Math.Sin(a);
                                double lrJ = LR + r * Math.Cos(a);

                                if (fbJ < FB)  // Forward 127-0
                                {
                                    if (lrJ < LR)  // Left 128-0
                                    {
                                        // main engine - 3.
                                        // additionals - 2 & 4.

                                        var mainEn = r * Math.Sin(b) * incK;
                                        var addEn = r * Math.Cos(b) * incK;

                                        var en2 = DR_MIN + (addEn < 0 ? Math.Abs(addEn) : (mainEn - addEn) / 2.0);
                                        var en4 = DR_MIN + (addEn > 0 ? addEn : (mainEn + addEn) / 2.0);
                                        mainEn += DR_MIN;

                                        WriteLine(stream, fbJ, lrJ, DR_MIN, en2, mainEn, en4);
                                    }
                                    else if (LR < lrJ)  // Right 128-255
                                    {
                                        // main engine - 4.
                                        // additionals - 1 & 3.

                                        var mainEn = r * Math.Cos(b) * incK;
                                        var addEn = r * Math.Sin(b) * incK;

                                        var en1 = DR_MIN + (addEn < 0 ? Math.Abs(addEn) : (mainEn - addEn) / 2.0);
                                        var en3 = DR_MIN + (addEn > 0 ? addEn : (mainEn + addEn) / 2.0);
                                        mainEn += DR_MIN;

                                        WriteLine(stream, fbJ, lrJ, en1, DR_MIN, en3, mainEn);
                                    }
                                    else  // Straight Forward
                                    {
                                        var en = DR_MIN + r * incK;

                                        WriteLine(stream, fbJ, lrJ, DR_MIN, DR_MIN, en, en);
                                    }
                                }
                                else if (fbJ > FB)  // Back 127-255
                                {
                                    if (lrJ < LR)  // Left 128-0
                                    {
                                        // main engine - 2.
                                        // additionals - 1 & 3.

                                        var mainEn = (-1.0) * r * Math.Cos(b) * incK;
                                        var addEn = r * Math.Sin(b) * incK;

                                        var en1 = DR_MIN + (addEn < 0 ? Math.Abs(addEn) : (mainEn - addEn) / 2.0);
                                        var en3 = DR_MIN + (addEn > 0 ? addEn : (mainEn + addEn) / 2.0);
                                        mainEn += DR_MIN;

                                        WriteLine(stream, fbJ, lrJ, en1, mainEn, en3, DR_MIN);
                                    }
                                    else if (LR < lrJ)  // Right 128-255
                                    {
                                        // main engine - 1.
                                        // additionals - 4 & 2.

                                        var mainEn = (-1) * r * Math.Sin(b) * incK;
                                        var addEn = r * Math.Cos(b) * incK;

                                        var en4 = DR_MIN + (addEn < 0 ? Math.Abs(addEn) : (mainEn - addEn) / 2.0);
                                        var en2 = DR_MIN + (addEn > 0 ? addEn : (mainEn + addEn) / 2.0);
                                        mainEn += DR_MIN;

                                        WriteLine(stream, fbJ, lrJ, mainEn, en2, DR_MIN, en4);
                                    }
                                    else  // Straight Back
                                    {
                                        var en = DR_MIN + r * incK;

                                        WriteLine(stream, fbJ, lrJ, en, en, DR_MIN, DR_MIN);
                                    }
                                }
                                else // Straight
                                {
                                    if (lrJ < LR)  // Straight Left 128-0
                                    {
                                        var en = DR_MIN + r * incK;

                                        WriteLine(stream, fbJ, lrJ, en, DR_MIN, DR_MIN, en);
                                    }
                                    else if (LR < lrJ)  // Straight Right 128-255
                                    {
                                        var en = DR_MIN + r * incK;

                                        WriteLine(stream, fbJ, lrJ, DR_MIN, en, en, DR_MIN);
                                    }
                                }
                            }

                            if (r == 127)
                            {
                                break;
                            }

                            r = 128 - r > radiusK ? r + radiusK : 127;
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
                arg[0], arg[1], arg[2], arg[3], arg[4], arg[5]);
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
