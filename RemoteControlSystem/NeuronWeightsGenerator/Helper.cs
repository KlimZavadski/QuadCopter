using System;

namespace NeuronWeightsGenerator
{
    public static class Helper
    {
        public delegate bool BoolAction();

        public static void ShowAlert(string message, Action action)
        {
            Console.WriteLine(message);
            if (Console.ReadLine() == "y")
            {
                action();
            }
        }

        public static bool ShowAlert(string message, BoolAction action)
        {
            Console.WriteLine(message);
            return Console.ReadLine() == "y" && action();
        }
    }
}
