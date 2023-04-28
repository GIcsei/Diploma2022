using NationalInstruments.Tdms;
using PSI_Checker_2p0.PSI5_Utils;
using System;
using System.IO;
using System.Linq;

namespace PSI_Checker_2p0.Checker
{
    public class TDMSHandler : BaseResultHandler
    {
        // TODO 
        // Szigorúan user felelősség, hogy jó legyen a format!!!! --> Error message
        // Possible check: property info-ban Checker név legyen --> Fix value
        // Nem általános, egyedi infó needed!

        public override bool SaveToFile()
        {
            bool returnValue = false;
            if (String.IsNullOrEmpty(FullPath))
            {
                throw new ArgumentNullException("Directory patternFilePath was not set accordingly!\n");
            }
            using (var tdmsDest = new TdmsFile(FullPath, new TdmsFileOptions(TdmsFileFormat.Version20, TdmsFileAccess.ReadWrite)))
            {
                tdmsDest.Open();
                PopulateTDMSFile(tdmsDest);
                returnValue = true;
            }
            return returnValue;
        }

        private void PopulateTDMSFile(TdmsFile destination)
        {
            if (!destination.IsOpen)
                throw new FileLoadException();
            // Get every GroupName
            foreach (var groupName in dataContainer.FieldNames)
            {
                var group = destination.AddChannelGroup(groupName);
                // Get every ChannelName
                foreach (var channelName in dataContainer[groupName].GetElementsList())
                {
                    var channel = group.AddChannel(channelName.Name, TdmsDataType.Double);
                    channel.AppendData(channelName.Values);
                }
            }
        }

        public override IResultData<BaseDataContainer> LoadFromFile(string filePath)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("Directory patternFilePath was not set accordingly!\n");
            }
            using (var tdmsDest = new TdmsFile(@filePath, new TdmsFileOptions(TdmsFileFormat.Version20, TdmsFileAccess.Read)))
            {
                dataContainer = new BaseDataContainer
                {
                    Name = tdmsDest.Name
                };
                tdmsDest.Open();
                string key;
                foreach (TdmsChannelGroup group in tdmsDest.GetChannelGroups().Cast<TdmsChannelGroup>())
                {
                    var groupElem = new DataContainer(group.Name);
                    foreach (TdmsChannel channel in group.GetChannels().Cast<TdmsChannel>())
                    {
                        key = channel.Name;
                        try
                        {
                            groupElem.AddList(key, channel.GetData<double>().ToList());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                    dataContainer.Add(groupElem);
                }
            }
            return this;
        }
    }
}
