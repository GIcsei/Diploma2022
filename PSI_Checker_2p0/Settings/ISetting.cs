namespace PSI_Checker_2p0
{
    public interface ISetting<type> where type : class
    {
        string Name { get; }
        string Description { get; }
        type Value { get; }
    }
}
