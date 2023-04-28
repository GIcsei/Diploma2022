namespace PSI_Checker_2p0.Utils
{
    public static class GigabyteToByteConverter
    {
        private const long GB = 1073741824;

        public static long GBToByte(int ConvertFromGb) => ConvertFromGb * GB;
    }
}
