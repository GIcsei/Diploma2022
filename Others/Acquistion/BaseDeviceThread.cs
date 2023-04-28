using PSI_Checker_2p0.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.Acquistion
{
    public abstract class BaseDeviceThread : NotifyBase, IDeviceThread
    {
        protected Thread deviceThread;
        private bool isRunning;
        private readonly string threadName;

        public bool IsRunning
        {
            get => isRunning;
            protected set => SetProperty(ref isRunning, value, nameof(IsRunning));
        }

        protected BaseDeviceThread(string threadName)
        {
            isRunning = false;
            this.threadName = threadName;
        }

        private void CreateThread()
        {
            deviceThread = new Thread(new ThreadStart(ThreadTask))
            {
                Name = threadName,
                IsBackground = true,
                Priority = ThreadPriority.Highest,
            };
        }

        public async Task StartThread()
        {
            await Task.Run(() =>
            {
                InitValues();
                CreateThread();
                IsRunning = true;
                deviceThread.Start();
            });

        }
        protected abstract void InitValues();

        public async Task StopThread()
        {
            await Task.Run(async () =>
            {
                await ClearUp();
                IsRunning = false;
            });
        }

        protected abstract Task ClearUp();
        protected abstract void ThreadTask();
    }
}
