namespace NeuronWeightsGenerator
{
    public static class Helper
    {
        public const int DrMin = 85;
        public const int DrMax = 160;

        private static readonly double Diff;

        static Helper()
        {
            Diff = DrMax - DrMin;
        }

        public static double MapJoystickValueToNetwork(byte value)
        {
            return (value - DrMin) / Diff;
        }

        public static int MapNetworkValueToDriver(double value)
        {
            return (int) (value * Diff + DrMin);
        }
    }
}
