using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Controls;

namespace PSI_Checker_2p0.Views
{
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : UserControl
    {
        private OpenFileDialog tdmsOpenFileDialog = new OpenFileDialog();
        public Results()
        {
            InitializeComponent();
        }

        private void OnTdmsFileBrowseButtonClicked(object sender, EventArgs e)
        {

            tdmsOpenFileDialog.InitialDirectory = GetInitialDirectory();
            tdmsOpenFileDialog.ValidateNames = false;
            tdmsOpenFileDialog.CheckFileExists = false;
            tdmsOpenFileDialog.CheckPathExists = true;
            tdmsOpenFileDialog.FileName = "Choose the intended folder!";
            if (tdmsOpenFileDialog.ShowDialog() == true)
            {
                DirPath.Text = System.IO.Path.GetDirectoryName(tdmsOpenFileDialog.FileName);
            }
        }

        private string GetInitialDirectory()
        {
            string initialDirectory = @"..\..";
            DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            if (currentDirectory.Parent != null)
            {
                DirectoryInfo parentDirectory = currentDirectory.Parent;
                if (parentDirectory.Parent != null)
                {
                    initialDirectory = parentDirectory.Parent.FullName;
                }
            }
            return initialDirectory;
        }
    }
}
