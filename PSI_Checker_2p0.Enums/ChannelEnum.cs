using System.ComponentModel;

namespace PSI_Checker_2p0.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ImpedanceEnum
    {
        [Description("1 MOhm")]
        Impedance_1MOhm = 1_000_000,
        [Description("50 Ohm")]
        Impedance_50Ohm = 50
    }

    public enum CouplingEnum
    {
        AC,
        DC,
    }
}