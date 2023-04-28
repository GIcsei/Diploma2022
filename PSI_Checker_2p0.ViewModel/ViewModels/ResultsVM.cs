using PSI_Checker_2p0.Checker;
using PSI_Checker_2p0.FileHandler.FileLoader;
using PSI_Checker_2p0.Utils;
using ScottPlot;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSI_Checker_2p0.ViewModel.ViewModels
{
    public class ResultsVM : BaseVM
    {
        private DataTable tdmsTable;
        public DataTable TdmsTable
        {
            get => tdmsTable;
            set => SetProperty(ref tdmsTable, value, nameof(TdmsTable));
        }

        public int MaxProgress { get; } = 100;
        private readonly BaseResultHandler checker = new TDMSHandler();
        private readonly BaseFileLoader fileLoader = new TdmsFileLoader();

        private string _dirPath;
        public string DirPath
        {
            get => _dirPath;
            set => SetProperty(ref _dirPath, value, nameof(DirPath));
        }

        private bool canBeClicked = true;
        public bool CanBeClicked
        {
            get => canBeClicked;
            private set => SetProperty(ref canBeClicked, value, nameof(CanBeClicked));
        }

        private string selectedSource;
        public string SelectedSource
        {
            get => selectedSource;
            set => SetProperty(ref selectedSource, value, nameof(SelectedSource));
        }

        private int progress;
        public int Progress
        {
            get => progress;
            set => SetProperty(ref progress, value, nameof(Progress));
        }

        private double[] selectedValues = new double[1000];
        public double[] SelectedValues
        {
            get => selectedValues;
            set => SetProperty(ref selectedValues, value, nameof(SelectedValues));
        }


        private ObservableCollection<string> result;
        public ObservableCollection<string> Result
        {
            get => result;
            set => SetProperty(ref result, value, nameof(Result));
        }

        public ResultsVM()
        {
            PropertyChanged += GetTdms;
            ShowPlot = new RelayCommand(ChangePlotSetup);
            PlotShower.Plot.AddSignal(SelectedValues);
            PlotShower.Configuration.DoubleClickBenchmark = false;
            PlotShower.Configuration.UseRenderQueue = false;
            PlotShower.Configuration.Quality = ScottPlot.Control.QualityMode.High;
            PlotShower.Refresh();
        }

        private async void GetTdms(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DirPath))
            {
                CanBeClicked = false;
                checker.SaveLocation = DirPath;
                var task = new BackgroundWorker { WorkerReportsProgress = true };
                task.DoWork += TDMSTask;
                task.ProgressChanged += Report_Progress;
                task.RunWorkerCompleted += TDMSCompleted;
                task.RunWorkerAsync();
            }
            if (e.PropertyName == nameof(SelectedSource))
            {
                if (SelectedSource is null)
                    return;
                var result = await LoadTdmsAsync();
                TdmsTable = result;
            }
        }

        #region BackgroundWorker

        private void TDMSCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Result = e.Result as ObservableCollection<string>;
            TableItems = Result;
            OnPropertyChanged(nameof(Result));
            CanBeClicked = true;
        }

        private void Report_Progress(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        private void TDMSTask(object sender, DoWorkEventArgs e)
        {
            result = new ObservableCollection<string>();
            double progress = 0;
            var res = fileLoader.NumberOfFiles(DirPath);
            double progressStep = (double)MaxProgress / res;
            var task = sender as BackgroundWorker;
            task.ReportProgress(0);
            foreach (var item in fileLoader.LoadAllFiles(DirPath))
            {
                result.Add(item);
                task.ReportProgress((int)(progress += progressStep));
            }
            task.ReportProgress(MaxProgress);
            e.Result = result;
        }

        #endregion

        private WpfPlot plotShower = new WpfPlot();

        public WpfPlot PlotShower
        {
            get => plotShower;
            set => SetProperty(ref plotShower, value, nameof(PlotShower));
        }

        public ICommand ShowPlot { get; }

        private void ChangePlotSetup()
        {
            PlotShower.Refresh();
        }

        private System.Collections.IEnumerable tableItems;

        public System.Collections.IEnumerable TableItems { get => tableItems; set => SetProperty(ref tableItems, value); }

        private int selectedIndex;

        public int SelectedIndex
        {
            get => selectedIndex;
            set => SetProperty(ref selectedIndex, value, nameof(SelectedIndex));
        }

        private async Task<DataTable> LoadTdmsAsync()
        {
            var result = await Task.Factory.StartNew(() =>
            {
                BaseResultHandler handler = new TDMSHandler();
                var elem = handler.LoadFromFile(SelectedSource).Result;
                var dataTable = new DataTable();
                bool Init = true;
                foreach (var field in elem.GetFields())
                {
                    dataTable.Columns.Add(field.Name);
                    if (Init || field.Values.Count > dataTable.Rows.Count)
                    {
                        for (int i = dataTable.Rows.Count; i < field.Values.Count; i++)
                        {
                            dataTable.Rows.Add(dataTable.NewRow());
                        }
                        Init = false;
                    }

                    for (var index = 0; index < field.Values.Count; index++)
                    {
                        dataTable.Rows[index][field.Name] = field.Values[index];

                    }
                }
                return dataTable;
            });
            return result;
        }
    }
}
