namespace PSI_Checker_2p0.Protocol
{
    public interface IProtocol
    {
        string Name { get; }

        /// <summary>
        /// Creates the protocol descriptor.
        /// </summary>
        /// <returns></returns>
        string ToString();

        /// <summary>
        /// Check if the received data is correct or not.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool CheckData(long data);

        /// <summary>
        /// Wraps the sensor data into a command.
        /// </summary>
        /// <param name="data">The raw data to send to the sensor.</param>
        /// <returns></returns>
        byte EncodeCommand(int data);

        /// <summary>
        /// Decodes sensor command and get the relevant sensor data.
        /// </summary>
        /// <param name="data">The uncoded data received from the sensor.</param>
        /// <returns></returns>
        int DecodeCommand(byte data);
    }
}