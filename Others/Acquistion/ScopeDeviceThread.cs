using PSI_Checker_2p0.Devices;
using PSI_Checker_2p0.FileHandler.FileSaver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.Acquistion
{
    public class ScopeDeviceThread : BaseDeviceThread, IScopeThread
    {
        private bool _disposed;
        private IScope scope;
        public IScope Scope
        {
            get => scope;
        }

        private List<double[]> readValues;

        public List<double[]> ReadValues
        {
            get => readValues;
        }

        private BaseScopeFileHandler fileSaver = ScopeFileSaverBuilder.CreateFileSaver(
                    Enums.FileSavePolicy.OneFile
                );
        public BaseScopeFileHandler FileSaver
        {
            get => fileSaver;
            set => fileSaver = value;
        }

        public ScopeDeviceThread() : base("ScopeThread")
        {
        }

        public async Task AddScope(IScope scope)
        {
            await Task.Run(() =>
            {
                if (!IsRunning)
                {
                    this.scope = scope;
                }
            });
        }

        protected override void InitValues()
        {
            readValues = new List<double[]>();
            for (int numel = 0; numel < scope.ChannelNum; numel++)
            {
                readValues.Add(new double[scope.RecordLength]);
            }
        }

        protected override void ThreadTask()
        {
            scope.Open();
            scope.Init();
            _disposed = false;
            var temp = scope.RecordLength;
            try
            {
                while (!_disposed)
                {
                    int indexer = 0;
                    // Get the results
                    foreach (var value in scope.ReadMultipleValues())
                    {
                        value.CopyTo(ReadValues[indexer], 0);
                        fileSaver.SaveFile(ReadValues[indexer], indexer);
                        indexer++;
                    }
                    this.OnPropertyChanged(nameof(ReadValues));
                }
                scope.RecordLength = -1;
                int index = 0;
                // Get the results
                foreach (var value in scope.ReadMultipleValues())
                {
                    ReadValues[index] = value;
                    fileSaver.SaveFile(value, index);
                    index++;
                }
                this.OnPropertyChanged(nameof(ReadValues));
            }
            catch
            {
                scope.Close();
            }
            finally
            {
                IsRunning = false;
                scope.RecordLength = temp;
                readValues = new List<double[]>();
            }
        }

        protected override async Task ClearUp()
        {
            _disposed = true;
            if (IsRunning)
                await scope.Close();
            deviceThread.Join();
            //fileSaver.ClearAll();
        }
    }
}
