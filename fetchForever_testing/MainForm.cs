//=====================================================================================================
//
// Title:
//        Fetch Forever
//
// Description:
//      The application demonstrates how to fetch data records continuously from a NI-SCOPE device.
//      It uses a Memory Optimized asynchronous version of Fetch. The program continues fetching 
//      until the stop button is pressed or an exception is thrown.
//
//==================================================================================================

using System;
using System.Windows.Forms;
using System.Data;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.ModularInstruments.SystemServices.DeviceServices;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace NationalInstruments.Examples.FetchForever
{
    public partial class MainForm : Form
    {
        NIScope scopeSession;
        bool stop;
        List<string> collector = new List<string> { "without_predefined_collector", "with_predefined_collector" };
        List<string> collector2 = new List<string> { "with_every_data","with_minimum_data"};
        List<string> collector3 = new List<string> { "without_timeout", "with_timeout", "right_now" };
        const int stepSize = 50000;
        const int maxTimeout = 50;
        const int MaxSampleLength = 250000000;
        DataTable table = new DataTable("Results");
        const int limit = 2000000;
        public MainForm()
        {
            InitializeComponent();
            LoadScopeDeviceNames();
            ChangeControlState(true);
        }

        #region Mainform initial configuration
        void LoadScopeDeviceNames()
        {
            using (ModularInstrumentsSystem scopeDevices = new ModularInstrumentsSystem("NI-Scope"))
            {
                foreach (DeviceInfo device in scopeDevices.DeviceCollection)
                {
                    resourceNameComboBox.Items.Add(device.Name);
                }
            }
            if (resourceNameComboBox.Items.Count > 0)
            {
                resourceNameComboBox.SelectedIndex = 0;
            }
        }
        #endregion

        #region Mainform configuration values
        string ResourceName
        {
            get
            {
                return this.resourceNameComboBox.Text;
            }
        }

        string ChannelName
        {
            get
            {
                int val;
                //Strip off the device name from the fully qualified channel name (i.e. convert Dev1/0 to 0)
                string unqualifiedChannelName = Regex.Replace(this.channelNameTextBox.Text, "[^,]+/", "");
                //Check the unqualified channel name to verify it is a single channel with only digits
                if (!Int32.TryParse(unqualifiedChannelName, out val))
                {
                    throw new ArgumentException("The channel name specified is either invalid or multiple channels are specified.\n\r\n\rThis example supports only 1 channel.");
                }
                return this.channelNameTextBox.Text;
            }
        }

        double SampleRateMin
        {
            get
            {
                return decimal.ToDouble(this.minSampleRateNumeric.Value);
            }
        }

        double Range
        {
            get
            {
                return decimal.ToDouble(this.verticalRangeNumeric.Value);
            }
        }

        long PointsToFetchMax
        {
            get
            {
                return decimal.ToInt64(this.maxPointsFetchedNumeric.Value);
            }
        }

        string TotalPointsFetched
        {
            set
            {
                this.totalPointsFetchedTextBox.Text = value;
            }
        }

        string LastFetchPoints
        {
            set
            {
                this.lastFetchedPointsTextBox.Text = value;
            }
        }
        #endregion

        void acquireButton_Click(object sender, System.EventArgs e)
        {
            FetchForever();
        }

        void FetchForever()
        {
            stop = false;
            ChangeControlState(false);
            InitTable();
            // Configure the vertical parameters.
            double offset = 0.0;
            ScopeVerticalCoupling coupling = ScopeVerticalCoupling.DC;
            double probeAttenuation = 1.0;
            double referencePosition = 0.0;
            int numberOfRecords = 1;
            bool enforceRealtime = true;
            DisplayMessage("Acquisition is in progress...\r\n");

            int timeout=0, collectData;
            bool isCrashed = false;
            for (int i = 0; i < collector.Count; i++) {
                
                DisplayMessage($"Start of {collector[i]}\r\n");
                for (int j=0; j< collector2.Count; j++) // 0: -1, 1: MinLength
                {
                    DisplayMessage($"Start of {collector2[j]}\r\n");
                    for (int k=0; k < collector3.Count; k++) // 0: -1, 1: maxTimeout, 2: 0 
                    {
                        DisplayMessage($"Start of {collector3[k]}\r\n");
                        if (k == 0) timeout = -1;
                        else if (k == 1) timeout = maxTimeout;
                        else if (k == 2) timeout = 0;
                        var watch = new System.Diagnostics.Stopwatch();
                        int recordLengthMin = stepSize;
                        TimeSpan timeoutTicks = new TimeSpan(timeout);
                        long elapsedMs;
                        AnalogWaveformCollection<double> waveformsIn = null, waveformsOut = null;
                        while (recordLengthMin <= MaxSampleLength)
                        {
                            int counter = 0;
                            if (j == 0) collectData = -1;
                            else collectData = recordLengthMin;
                            string unhandledMessgae = "";
                            try
                            {
                                InitializeSession();
                                scopeSession.Channels[ChannelName].Configure(Range, offset, coupling, probeAttenuation, true);

                                // Configure the horizontal parameters.
                                scopeSession.Timing.ConfigureTiming(MaxSampleLength, recordLengthMin, referencePosition, numberOfRecords, enforceRealtime);

                                // Configure software trigger, but never send the trigger.
                                // This starts an infinite acquisition, until you call niScope_Abort or niScope_close.
                                scopeSession.Trigger.ConfigureTriggerSoftware(PrecisionTimeSpan.Zero, PrecisionTimeSpan.Zero);
                                scopeSession.Measurement.FetchRelativeTo = ScopeFetchRelativeTo.ReadPointer;
                                var iter = recordLengthMin;
                                scopeSession.Measurement.Initiate();
                                watch.Restart();
                                try
                                {
                                    while (counter++ != limit)
                                    {
                                        // the code that you want to measure comes here
                                        if(i == 0)
                                            waveformsOut = scopeSession.Channels[ChannelName].Measurement.FetchDouble(PrecisionTimeSpan.FromTimeSpan(timeoutTicks),
                                            collectData, waveformsIn);
                                        else
                                            waveformsOut = scopeSession.Channels[ChannelName].Measurement.FetchDouble(PrecisionTimeSpan.FromTimeSpan(timeoutTicks),
                                            collectData, waveformsOut);
                                    }
                                    DisplayMessage("Acquisition successful!!!");
                                }
                                catch (Exception) {
                                    isCrashed = true;
                                }
                                finally
                                {
                                    watch.Stop();
                                    scopeSession.Measurement.Abort();
                                }
                            }
                            catch (Exception ex)
                            {
                                unhandledMessgae = ex.Message;
                            }
                            finally
                            {
                                elapsedMs = watch.ElapsedMilliseconds;
                                PlotWaveforms(elapsedMs, recordLengthMin, isCrashed, unhandledMessgae);
                                CloseSession();
                                recordLengthMin += stepSize;
                            }
                        }
                        SaveResults(i,j,k);
                        ClearWaveforms();
                    }
                }
            }
            ChangeControlState(true);
        }

        void ClearWaveforms()
        {
            waveformDataGridView.Columns.Clear();
            table.Rows.Clear();
        }

        void SaveResults(int i, int j, int k)
        {
            var dest = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), String.Concat(collector[i], "_", collector2[j], "_", collector3[k],".xml"));
            table.AcceptChanges();
            table.WriteXml(dest.ToString());
        }

        void InitTable()
        {
            table.Columns.Add("RecordLengthMin", typeof(int));
            table.Columns.Add("IsCrashed", typeof(bool));
            table.Columns.Add("Runtime", typeof(long));
            table.Columns.Add("Message", typeof(string));
        }
        void PlotWaveforms(long runtime, int recordLengthMin, bool isCrashed, string unhandledMessage)
        {
                table.Rows.Add(recordLengthMin, isCrashed, runtime, unhandledMessage);
        }

        void UpdateOutputResults(double totalPointsFetched, long actualNumberOfSamples)
        {
            TotalPointsFetched = totalPointsFetched.ToString();
            LastFetchPoints = actualNumberOfSamples.ToString();
        }
        void UpdateOutputResults(long totalPointsFetched, long actualNumberOfSamples)
        {
            TotalPointsFetched = totalPointsFetched.ToString();
            LastFetchPoints = actualNumberOfSamples.ToString();
        }

        #region NoChangeRequired

        void mainForm_Closing(object sender, FormClosingEventArgs e)
        {
            CloseSession();
        }

        void stopButton_Click(object sender, System.EventArgs e)
        {
            if (!stop)
            {
                stop = true;
                DisplayMessage("Stop in progress...Fetched points are being plotted...");
            }
        }

        void DisplayMessage(string message)
        {
            messageTextBox.Text += message;
            this.Refresh();
        }

        void InitializeSession()
        {
            scopeSession = new NIScope(ResourceName, false, false);
            scopeSession.DriverOperation.Warning += new EventHandler<ScopeWarningEventArgs>(DriverOperation_Warning);
        }

        void DriverOperation_Warning(object sender, ScopeWarningEventArgs e)
        {
            messageTextBox.Clear();
            MessageBox.Show(e.Text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        void ChangeControlState(bool isEnabled)
        {
            generalGroupBox.Enabled = isEnabled;
            acquireButton.Enabled = isEnabled;
            stopButton.Enabled = !isEnabled;
            if (!isEnabled)
            {
                ClearWaveforms();
            }
            this.Refresh();
        }

        void CloseSession()
        {
            if (scopeSession != null)
            {
                try
                {
                    scopeSession.Close();
                    scopeSession = null;
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                    Application.Exit();
                }
            }
        }

        void ShowError(Exception ex)
        {
            messageTextBox.Clear();
            MessageBox.Show(ex.Message, ex.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion
    }
}
