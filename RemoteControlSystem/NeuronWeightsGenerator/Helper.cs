using System;

namespace NeuronWeightsGenerator
{
    public static class Helper
    {
        public static double MapDriverValueToNetwork(int value)
        {
            return (value - 87) / 75.0;
        }

        public static int MapNetworkValueToDriver(double value)
        {
            return (int) (value * 75.0 + 87.0);
        }
    }
}
