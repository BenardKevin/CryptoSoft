using Ookii.Dialogs.Wpf;
using projet_easy_save_v2.src.viewModels;
using System.Windows;
using System.Windows.Input;

namespace projet_easy_save_v2.src.views
{
    /// <summary>
    /// Logique d'interaction pour ModifyBackupStrategyView.xaml
    /// </summary>
    public partial class ModifyBackupStrategyView : Window
    {
        private readonly ModifyBackupStrategyViewModel modifyBackupStrategyViewModel;

        public ModifyBackupStrategyView(object selectedItem)
        {
            InitializeComponent();
            modifyBackupStrategyViewModel = new ModifyBackupStrategyViewModel(selectedItem);
            DataContext = modifyBackupStrategyViewModel;

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            modifyBackupStrategyViewModel.UpdateBackupStrategy(newName.Text, newType.Text, newSource.Text, newTarget.Text);
            Close();
        }

        private void SourceButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == true)
            {
                newSource.Text = folderBrowserDialog.SelectedPath;
            }
        }
        private void TargetButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == true)
            {
                newTarget.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void DraggableWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

    }
}
