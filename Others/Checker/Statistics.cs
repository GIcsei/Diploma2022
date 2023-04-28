using System;
using System.Linq;

namespace PSI_Checker_2p0.MathFunctions
{
    public static class Statistics
    {
        public static double[] GetSubArray(double[] data, int startIndex, int numel)
        {
            double[] result = new double[numel];
            Array.Copy(data, startIndex, result, 0, numel);
            return result;
        }

        public static void MinMaxMean(double[] data, out double min, out double max, out double mean)
        {
            min = data.Min();
            max = data.Max();
            mean = Mean(data);
        }

        public static double Mean(double[] array)
        {
            double result = 0;
            for (int index = 0; index < array.Length; index++)
            {
                result += (array[index] - result * index) / (index + 1);
            }
            return result;
        }
    }
}
