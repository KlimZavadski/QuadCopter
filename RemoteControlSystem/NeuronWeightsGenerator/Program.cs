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
            Thread.CurrentThread.Join();
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

            while (iteration < iterations && error < 0.00005)
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

        #region Samples

        private bool GenerateSamples()
        {
            using (var stream = new StreamWriter(SamplesFile))
            {
                try
                {
                    const double incK = 0.1;

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

                    // Elementary
                    //
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

                    // Advanced
                    //
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
            stream.WriteLine("{0:000} {1:000} {2:000} {3:000} {4:000} {5:000}",
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
                        var array = stream.ReadLine().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
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

        #endregion
    }
}
