using System.ComponentModel;

namespace PSI_Checker_2p0.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum PSI5_DataRate
    {
        [Description("125 kbps")]
        Low = 125,
        [Description("189 kbps")]
        High = 189,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum PSI5_CheckSum
    {
        CRC = 0,
        Parity = 1,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum PSI5_Mode
    {
        [Description("10 bit")]
        Mode_10,
        [Description("16 bit")]
        Mode_16,
        [Description("10+10 bit")]
        Mode_10p10,
        [Description("16+4 bit")]
        Mode_16p4,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum PSI5_Sync
    {
        Async = 0,
        Sync = 1,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum PSI5_SyncDist
    {
        [Description("250 us")]
        Dist_250 = 250,
        [Description("350 us")]
        Dist_350 = 350,
        [Description("500 us")]
        Dist_500 = 500,
        [Description("750 us")]
        Dist_750 = 750,
        [Description("1000 us")]
        Dist_1000 = 1000,
    }
}
