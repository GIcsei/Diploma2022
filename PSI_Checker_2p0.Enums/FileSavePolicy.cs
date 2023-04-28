namespace PSI_Checker_2p0.Enums
{
    /// <summary>
    /// Enum for file save policy, 
    /// which describes how the data will be saved.
    /// </summary>
    public enum FileSavePolicy
    {
        /// <summary>
        /// Depending on the implementation, the default policy will be used.
        /// </summary>
        Default,
        /// <summary>
        /// The file saver will fill the disk,
        /// until it has free space.
        /// </summary>
        FillDisk,
        /// <summary>
        /// Save into one file, overrides the previously written data
        /// </summary>
        OneFile,
        /// <summary>
        /// Writes into predefined number of files.
        /// If these files are filled,
        /// starts from the beginning with overwrite.
        /// </summary>
        Circular,
    }
}
