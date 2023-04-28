using System;

namespace PSI_Checker_2p0.Protocol
{
    public abstract class BaseProtocol : IProtocol
    {
        public abstract bool CheckData(long data);

        public abstract int DecodeCommand(byte data);

        public abstract byte EncodeCommand(int data);

        private int bitRate;
        private int checkSum;
        private double bitTime;
        protected int dataLength;
        public int BitRate
        {
            get => bitRate;
            set
            {
                if (value < 0) throw new ArgumentException();
                bitRate = value;
                bitTime = 1000.0 / value;
            }
        }
        public int CheckSum
        {
            get => checkSum;
            set => checkSum = value;
        }

        public double BitTime
        {
            get => bitTime;
            protected set => bitTime = value;
        }

        public int DataLength
        {
            get => dataLength;
            private set => dataLength = value;
        }

        public string Name
        {
            get;
            protected set;
        }
    }
}
