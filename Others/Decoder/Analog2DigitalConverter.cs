using System;

namespace PSI_Checker_2p0.Decoder
{
    abstract class Analog2DigitalConverter : IDigitalDecoder, IDigitizer
    {
        protected TimestampData timestampData;
        public TimestampData TimestampData
        {
            get
            {
                if (!DecodingDone)
                {
                    return null;
                }
                return timestampData;
            }
        }

        public bool DecodingDone { get; protected set; } = false;
        public int SampleRate { get => (int)SamplingRate; }
        public double[] AnalogData { get; set; }
        public int[] DigitalData { get; set; }

        public abstract int[] Decode();
        public abstract void Digitalize(double[] data);

        protected double ComparisonLevel;
        protected uint SamplingRate;
        protected double DataRate;
        protected uint DataLength = 21;

        /// <summary>
        /// Shows how many samples equals with 1 bit.
        /// <see cref="DigitalData"/> must contains at least the number of elements
        /// equals to this field!
        /// </summary>
        protected readonly int SamplePerBitTime;

        public Analog2DigitalConverter(double ComparisonLevel, uint SamplingRate, double DataRate)
        {
            if (ComparisonLevel < 0 || DataRate < 0) throw new ArgumentOutOfRangeException();
            this.DataRate = DataRate;
            this.ComparisonLevel = ComparisonLevel;
            this.SamplingRate = SamplingRate;

            SamplePerBitTime = CalculateSamplePerBitTime();
        }

        private int CalculateSamplePerBitTime() => (int)Math.Floor(DataRate / SamplingRate);

        /// <summary>
        /// Calculates if the <see cref="DigitalData"/> can be decoded or not.
        /// <see cref="DigitalData"/> must contains at least <see cref="SamplePerBitTime"/> elements.
        /// </summary>
        /// <returns>Returns true if the <see cref="DigitalData"/> can be decoded. Else returns false.</returns>
        public abstract bool CanBeDecoded();
    }
}
