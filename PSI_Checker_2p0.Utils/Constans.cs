namespace PSI_Checker_2p0.Utils
{
    public static class Constans
    {
        public const int LOW = 125;
        public const int HIGH = 189;

        public const int MAX_START_TIMES = 5; /*User defined values*/

        public const double DELTA_T = 3.5;
        public const double T_START = 45;
    }

    public static class CHECKSUM
    {
        public const int CRC = 0;
        public const int PARITY = 1;
    }

    public static class BIDIR
    {

    }

    public static class PSI5_DEFAULT
    {
        public const bool SYNC = true;
        public const int SYNC_DIST = 500;
        public const int BITRATE = Constans.HIGH;
        public const int BIT_LEN = 16;
        public const int CHECK_TYPE = CHECKSUM.CRC;
        public static readonly int[] START_TIMES = { 817, 3364, 6103 };
    }
}
