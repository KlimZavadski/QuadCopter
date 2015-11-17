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
            string key = Console.ReadLine();

            if (key == "y")
            {
                
            }

            if (!LoadSamples())
            {
                return;
            }

            ActivationNetwork network = new ActivationNetwork(new SigmoidFunction(1), 8, 8, 8, 4);
            BackPropagationLearning teacher = new BackPropagationLearning(network)
            {
                Momentum = 0.1
            };

            int iteration = 0;
            const int iterations = 100000;
            double error;

            while (iteration < iterations)
            {
                error = teacher.RunEpoch(_inputList.ToArray(), _outputList.ToArray()) / _inputList.Count;
                iteration++;
            }

            var result = network.Compute(new[] { 46.0, 0, 0, 0, 0, 0, 0, 0 }).Select(x => x * 75 + 85);
        }

        private bool GenerateSamples()
        {
            using (var stream = new StreamWriter(_samples))
            {
                try
                {
                    int dr = 87;
                    int ud = 127;
                    int rlr = 128;
                    int fb = 127;
                    int lr = 128;

                    // UpDown, RotateLeftRight, ForwardBack, LeftRight.
//                    stream.WriteLine("{0:3}{1:3}{2:3}{3:3}{4:3}{5:3}{6:3}{7:3}{8:3}{9:3}{10:3}{11:3}",
//                        ud, );
//
//                    for (int i = 0; i < 25; i++)
//                    {
//
//                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("GenerateSamples error: {0}", ex.Message);
                    return false;
                }
            }

            return true;
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
                        var outp = array.Skip(8).Take(4).Select(x => (double.Parse(x) - 85.0) / 75.0).ToArray();
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

        private bool SaveSamples()
        {
            return true;
        }
    }
}
