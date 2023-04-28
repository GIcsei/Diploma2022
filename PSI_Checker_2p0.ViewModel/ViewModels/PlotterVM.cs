using PSI_Checker_2p0.Acquistion;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.ViewModel.ViewModels
{
    public abstract class PlotterVM : BaseVM
    {
        #region Privates
        private const int DivisionValue = 1000;
        protected ScopeDeviceThread deviceThread;
        private readonly List<double[]> ThreadResults = new List<double[]> { new double[DivisionValue], new double[DivisionValue] };
        private readonly object threadLocker = new object();
        #endregion

        /// <summary>
        /// Flag to show if there is any ongoing measurement.
        /// </summary>
        private bool ongoingMeas;
        public bool OngoingMeas
        {
            get => ongoingMeas;
            set
            {
                SetProperty(ref ongoingMeas, value, nameof(OngoingMeas));
            }
        }

        private System.Timers.Timer RenderTimer;
        protected PlotterVM()
        {
            InitPlotControl();
            InitTimer();
            InitThread();
        }


        /// <summary>
        /// The main container of the <see cref="ScottPlot"/> plot.
        /// Since the package does not support MVVM,
        /// the instance of the plot must be controlled
        /// from the VievModel.
        /// </summary>
        private static WpfPlot plotControl;
        public WpfPlot PlotControl
        {
            get => plotControl;
            set => SetProperty(ref plotControl, value, nameof(PlotControl));
        }

        /// <summary>
        /// Initalizes the Timer.
        /// Helps to render the <see cref="PlotControl"/> with 50FPS.
        /// </summary>
        private void InitTimer()
        {
            RenderTimer = new System.Timers.Timer(20)
            {
                Enabled = false,
                AutoReset = true
            };
            RenderTimer.Elapsed += Render50FPS;
        }

        /// <summary>
        /// Initialize the <see cref="PlotControl"/> and set the default plots.
        /// The number of plots depends on the <see cref="UsedChannels"/>.
        /// If the <see cref="PlotControl"/> is already initialized, no action is performed.
        /// </summary>
        private void InitPlotControl()
        {
            if (!(PlotControl is null))
            {
                PlotControl.Plot.Clear();
                return;
            }
            PlotControl = new WpfPlot();
            PlotControl.Plot.SetAxisLimits(0, 1000);
            PlotControl.Configuration.DoubleClickBenchmark = false;
            PlotControl.Configuration.UseRenderQueue = true;
            PlotControl.Configuration.Quality = ScottPlot.Control.QualityMode.High;
            PlotControl.Refresh();
        }

        /// <summary>
        /// Sent a render request for the <see cref="PlotControl"/> in every 20ms.
        /// The render quality is low, so the plot can be rendered as quickly as possible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Render50FPS(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.InvokeAsync(
                () => PlotControl.RenderRequest(RenderType.LowQuality));
        }

        /// <summary>
        /// Initialize the <see cref="deviceThread"/>.
        /// </summary>
        private void InitThread()
        {
            deviceThread = DIService.Instance.Get<ScopeDeviceThread>();
            if (deviceThread.IsRunning)
            {
                for (int i = 0; i < deviceThread.ReadValues.Count; i++)
                {
                    ThreadResults.Add(new double[DivisionValue]);
                    PlotControl.Plot.AddSignal(ThreadResults[i]);
                }
                RenderTimer.Enabled = true;
                OngoingMeas = true;
            }
            deviceThread.PropertyChanged += new PropertyChangedEventHandler(ThreadChange);
        }

        #region Thread handling
        /// <summary>
        /// Handles the <see cref="deviceThread"/> PropertyChange events.
        /// If the thread has read new values,
        /// it updates the <see cref="ThreadResults"/> array.
        /// </summary>
        /// <param name="sender">The instance of the <see cref="ScopeDeviceThread"/> class.</param>
        /// <param name="e">The property changed in <see cref="deviceThread"/>.</param>
        private void ThreadChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(deviceThread.ReadValues))
            {
                lock (threadLocker)
                {
                    int nStep = deviceThread.ReadValues[0].Length / DivisionValue;
                    int listElem = 0;
                    foreach (var elem in deviceThread.ReadValues)
                    {
                        int index = 0;
                        for (int i = 0; i < elem.Length; i += nStep, index++)
                        {
                            ThreadResults[listElem][index] = elem[i];
                        }
                        listElem++;
                    }
                }
            }
            if (e.PropertyName == nameof(deviceThread.IsRunning))
            {
                lock (threadLocker)
                {
                    RenderTimer.Enabled = deviceThread.IsRunning;
                    OngoingMeas = deviceThread.IsRunning;
                }
            }
        }


        protected async Task<bool> ChangeThreadState()
        {
            if (deviceThread.IsRunning)
            {
                await deviceThread.StopThread();
                return false;
            }
            PlotControl.Plot.Clear();
            ThreadResults.Clear();
            for (int i = 0; i < deviceThread.Scope.ChannelNum; i++)
            {
                ThreadResults.Add(new double[DivisionValue]);
                PlotControl.Plot.AddSignal(ThreadResults[i]);
            }
            await deviceThread.StartThread();
            return true;
        }

        public async Task ShowAll(IEnumerable<double[]> list)
        {
            await Task.Run(() =>
            {
                PlotControl.Plot.Clear();
                var index = 0;
                foreach (var channel in list)
                {
                    var sig = PlotControl.Plot.AddSignal(channel);
                    index++;
                    if (index % 2 == 1)
                    {
                        sig.YAxisIndex = PlotControl.Plot.YAxis2.AxisIndex;

                    }

                }
            });
            PlotControl.Refresh();
        }
        #endregion
    }
}
