namespace PSI_Checker_2p0.Decoder
{
    internal interface IDigitalDecoder
    {
        int[] DigitalData { get; }

        int[] Decode();
    }

    internal interface IDigitizer
    {
        double[] AnalogData { get; }

        void Digitalize(double[] data);
    }
}
