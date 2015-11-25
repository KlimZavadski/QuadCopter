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
        private string _samples = "../../Samples.txt";
        private readonly List<double[]> _inputList = new List<double[]>();
        private readonly List<double[]> _outputList = new List<double[]>();

        private static void Main(string[] args)
        {
            new Program().Worker();
            Thread.CurrentThread.Join();
        }

        private void Worker()
        {
            Console.WriteLine("Generate samples? y/n");
            //string key = Console.ReadLine();
            //if (key == "y")
            {
                //GenerateSamples();
                //return;
            }

            if (!LoadSamples())
            {
                return;
            }

            ActivationNetwork network = new ActivationNetwork(new SigmoidFunction(1), 8, 8, 8, 4);
            BackPropagationLearning teacher = new BackPropagationLearning(network)
            {
                Momentum = 0.0
            };

            int iteration = 0;
            const int iterations = 10000;
            double error = 1;

            while (iteration < iterations && error > 0.0005)
            {
                error = teacher.RunEpoch(_inputList.ToArray(), _outputList.ToArray()) / _inputList.Count;
                iteration++;

                if (error < 0.0005)
                {
                    Console.WriteLine("Ok! {0}, iter = {1}", error, iteration);
                }
            }

            var result1 = network.Compute(new[] { 127.0, 128, 80, 128, 0, 0, 0, 0 }).Select(x => x * 75.0 + 87.0).ToList();
            var result2 = network.Compute(new[] { 127.0, 128, 180, 128, 0, 0, 0, 0 }).Select(x => x * 75.0 + 87.0).ToList();
            var result3 = network.Compute(new[] { 127.0, 128, 127, 80, 0, 0, 0, 0 }).Select(x => x * 75.0 + 87.0).ToList();
            var result4 = network.Compute(new[] { 127.0, 128, 127, 180, 0, 0, 0, 0 }).Select(x => x * 75.0 + 87.0).ToList();
            var r1 = result1;
            var r2 = result2;
            var r3 = result3;
            var r4 = result4;
        }

        private bool GenerateSamples()
        {
            using (var stream = new StreamWriter(_samples))
            {
                try
                {
                    const int dr = 87;
                    const int ud = 127;
                    const int rlr = 128;
                    const int fb = 127;
                    const int lr = 128;

                    const double incK = 0.1;

                    //  1   2
                    //    x
                    //  4   3
                    // Up/Down 0-127-255
                    // Rotate Left/Right 0-128-255
                    // Forward/Back 0-127-255
                    // Left/Right 0-128-255

                    // Forward 127-0
                    for (int value = fb; value >= 0; value--)
                    {
                        var engine = dr + (fb - value) * incK;
                        WriteLine(stream, ud, rlr, value, lr, 0, 0, 0, 0, dr, dr, engine, engine);
                    }

                    // Back 127-255
                    for (int value = fb; value <= 255; value++)
                    {
                        var engine = dr + (value - fb) * incK;
                        WriteLine(stream, ud, rlr, value, lr, 0, 0, 0, 0, engine, engine, dr, dr);
                    }

                    // Left 128-0
                    for (int value = lr; value >= 0; value--)
                    {
                        var engine = dr + (lr - value) * incK;
                        WriteLine(stream, ud, rlr, fb, value, 0, 0, 0, 0, dr, engine, engine, dr);
                    }

                    // Right 128-255
                    for (int value = lr; value <= 255; value++)
                    {
                        var engine = dr + (value - lr) * incK;
                        WriteLine(stream, ud, rlr, fb, value, 0, 0, 0, 0, engine, dr, dr, engine);
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
            stream.WriteLine("{0:000} {1:000} {2:000} {3:000} {4:000} {5:000} {6:000} {7:000} {8:000} {9:000} {10:000} {11:000}",
                arg[0], arg[1], arg[2], arg[3], arg[4], arg[5], arg[6], arg[7], arg[8], arg[9], arg[10], arg[11]);
        }

        private bool LoadSamples()
        {
            using (var stream = new StreamReader(_samples))
            {
                try
                {
                    while (!stream.EndOfStream)
                    {
                        var array = stream.ReadLine().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                        var inp = array.Take(8).Select(x => double.Parse(x) / 255.0).ToArray();
                        var outp = array.Skip(8).Take(4).Select(x => (double.Parse(x) - 87.0) / 75.0).ToArray();
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
    }
}
