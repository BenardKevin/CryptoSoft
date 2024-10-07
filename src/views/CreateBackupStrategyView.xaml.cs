using projet_easy_save_v2.src.viewModels;
using System.Windows;
using System.Windows.Controls;
using Ookii.Dialogs.Wpf;

namespace projet_easy_save_v2.src.views
{
    /// <summary>
    /// Logique d'interaction pour CreateBackupStrategyView.xaml
    /// </summary>
    public partial class CreateBackupStrategyView : UserControl
    {
        readonly CreateBackupStrategyViewModel createBackupStrategyViewModel;

        public CreateBackupStrategyView()
        {
            InitializeComponent();
            createBackupStrategyViewModel = new CreateBackupStrategyViewModel();
            DataContext = createBackupStrategyViewModel;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            createBackupStrategyViewModel.CreateBackupStrategy(newName.Text, newType.Text, newSource.Text, newTarget.Text);
            MessageBox.Show("BackupStrategy created successfully!");
        }

        private void SourceButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            if(folderBrowserDialog.ShowDialog() == true)
            {
                createBackupStrategyViewModel.SourceDirectory = folderBrowserDialog.SelectedPath;
            }
        }
        private void TargetButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == true)
            {
                createBackupStrategyViewModel.TargetDirectory = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
