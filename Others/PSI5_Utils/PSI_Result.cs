namespace PSI_Checker_2p0.PSI5_Utils
{
    public class PSI_Result : BaseDataContainer
    {
        public PSI_Result() : base()
        {
            Add(new PSI_Data());
            Add(new PSI_Sync());
            Add(new DataContainer("PSI_RSET"));
        }

        public override void CalculateRemainingFields()
        {
            isReadOnly = true;
            foreach (var elem in Data)
                elem.CalculateRestrictedFields();
        }
    }
}
