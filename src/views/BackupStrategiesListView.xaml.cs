using projet_easy_save_v2.src.viewModels;
using System.Windows;
using System.Windows.Controls;

namespace projet_easy_save_v2.src.views
{
    /// <summary>
    /// Logique d'interaction pour BackupStrategiesListView.xaml
    /// </summary>
    public partial class BackupStrategiesListView : UserControl
    {
        private readonly BackupStrategiesListViewModel backupStrategiesListViewModel;

        public BackupStrategiesListView()
        {
            InitializeComponent();
            backupStrategiesListViewModel = new BackupStrategiesListViewModel();
            DataContext = backupStrategiesListViewModel;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                backupStrategiesListViewModel.DeleteBackupStrategy(backupsList.SelectedItem);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (backupsList.SelectedItem != null)
            {
                ModifyBackupStrategyView modifyBackupStrategyView = new ModifyBackupStrategyView(backupsList.SelectedItem);
                modifyBackupStrategyView.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a backup strategy to modify", "Warning", MessageBoxButton.OK);
            }
        }

    }
}
