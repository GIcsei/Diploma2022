namespace PSI_Checker_2p0.Enums
{
    public enum States_Enum
    {
        MC_BRAKE0,     //0
        MC_BRAKE1,         //1
        MC_SHORT0 = 0,         //2
        MC_SHORT1 = 1,     //3
        MC_LONG0 = 0,          //4
        MC_LONG1 = 1,          //5
        MC_EMPTY
    }

    public enum SYNC_PULSE
    {
        PULSE0 = 0,
        PULSE1,
        NEGATIVE = -1,
        POSITIVE = 1
    }
}
