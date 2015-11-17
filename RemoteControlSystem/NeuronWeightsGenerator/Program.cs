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
        private readonly List<double[]> _inputList = new List<double[]>();
        private readonly List<double[]> _outputList = new List<double[]>();

        private static void Main(string[] args)
        {
            new Program().Worker();
            Thread.CurrentThread.Join();
        }

        private void Worker()
        {
            double[][] input = new double[4][]
            {
                new double[] {0, 0}, new double[] {0, 1},
                new double[] {1, 0}, new double[] {1, 1}
            };
            double[][] output = new double[4][]
            {
                new double[] {0}, new double[] {1},
                new double[] {1}, new double[] {0}
            };

            ActivationNetwork network = new ActivationNetwork(new SigmoidFunction(1), 8, 8, 8, 4);
            BackPropagationLearning teacher = new BackPropagationLearning(network)
            {
                Momentum = 0.1
            };

            int iteration = 0;
            const int iterations = 100000;
            double error;

            if (LoadSamples())
            {
                while (iteration < iterations)
                {
                    error = teacher.RunEpoch(_inputList.ToArray(), _outputList.ToArray()) / _inputList.Count;
                    iteration++;
                }
            }

            var result = network.Compute(new[] {46.0, 0, 0, 0, 0, 0, 0, 0}).Select(x => x * 75 + 85);
        }

        private bool LoadSamples()
        {
            using (var stream = new StreamReader("../../Samples.txt"))
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
                    Console.WriteLine("Error: {0}", ex.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
