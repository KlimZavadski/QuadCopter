using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AForge.Neuro;
using AForge.Neuro.Learning;

namespace NeuronWeightsGenerator
{
    internal class Program
    {
        public const string SamplesFile = "../../Samples.txt";
        public const string NetworkFile = "../../Network.txt";

        private const int _inputCount = 2;
        private const int _outputCount = 4;
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
            Helper.ShowAlert("Generate new samples before training? y/n\n", () => GenerateSamples());

            if (!LoadSamples())
            {
                return;
            }

            var network = new ActivationNetwork(new SigmoidFunction(1), _inputCount, 8, 8, _outputCount);
            var teacher = new BackPropagationLearning(network)
            {
                Momentum = 0.1
            };

            int iteration = 0;
            const int iterations = 5000;
            double error = 1.0;

            while (iteration < iterations && error > 0.0003)
            {
                error = teacher.RunEpoch(_inputList.ToArray(), _outputList.ToArray()) / _inputList.Count;
                iteration++;
            }

            Console.WriteLine("Network trained! error = {0}, iteration = {1}\n", error, iteration);

            Helper.ShowAlert("Do you want to save network to file? y/n", () => network.Save(NetworkFile));


//            var result1 = network.Compute(new[] { 0.313725, 0.498039, 0.501961, 0.501961 }).Select(MapNetworkValueToDriver).ToList();
//            var result2 = network.Compute(new[] { 0.498039, 0.705882, 0.501961, 0.501961 }).Select(MapNetworkValueToDriver).ToList();
//            var result3 = network.Compute(new[] { 0.498039, 0.498039, 0.313725, 0.501961 }).Select(MapNetworkValueToDriver).ToList();
//            var result4 = network.Compute(new[] { 0.498039, 0.498039, 0.501961, 0.705882 }).Select(MapNetworkValueToDriver).ToList();
//            var r1 = result1;
//            var r2 = result2;
//            var r3 = result3;
//            var r4 = result4;
            var result1 = network.Compute(new[] {0.313725, 0.501961}).Select(MapNetworkValueToDriver).ToList();
            var result2 = network.Compute(new[] {0.705882, 0.501961}).Select(MapNetworkValueToDriver).ToList();
            var result3 = network.Compute(new[] {0.498039, 0.313725}).Select(MapNetworkValueToDriver).ToList();
            var result4 = network.Compute(new[] {0.498039, 0.705882}).Select(MapNetworkValueToDriver).ToList();
            var r1 = result1;
            var r2 = result2;
            var r3 = result3;
            var r4 = result4;
        }

        private double MapDriverValueToNetwork(int value)
        {
            return (value - 87) / 75.0;
        }

        private int MapNetworkValueToDriver(double value)
        {
            return (int) (value * 75.0 + 87.0);
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
                        var inp = array.Take(_inputCount).Select(x => double.Parse(x) / 255.0).ToArray();
                        var outp =
                            array.Skip(_inputCount)
                                .Take(_outputCount)
                                .Select(x => MapDriverValueToNetwork(int.Parse(x)))
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
