using projet_easy_save_v2.src.viewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace projet_easy_save_v2.src.views
{
    /// <summary>
    /// Logique d'interaction pour InitiateBackupView.xaml
    /// </summary>
    public partial class InitiateBackupView : UserControl
    {
        private readonly InitiateBackupViewModel initiateBackupViewModel;
        private bool isPaused = false;

        public InitiateBackupView()
        {
            InitializeComponent();
            initiateBackupViewModel = new InitiateBackupViewModel();
            DataContext = initiateBackupViewModel;
            initiateBackupViewModel.InitializeViewModel();
        }

        private void AddBackupToQueueButton_Click(object sender, RoutedEventArgs e)
        {
            initiateBackupViewModel.AddBackupToQueue(BackupsListToQueue.SelectedItems);

        }
        private void RemoveBackupFromQueueButton_Click(object sender, RoutedEventArgs e)
        {
            initiateBackupViewModel.RemoveBackupToQueue(backupQueue.SelectedItems);
        }

        private void StartBackupButton_Click(object sender, RoutedEventArgs e)
        {
            if (initiateBackupViewModel.EnableButtons)
            {
                initiateBackupViewModel.cancelAllBackups(); // TODO cancel all backups
            } else
            {
                try
                {
                    initiateBackupViewModel.StartBackupStrategiesAsync();
                    MyScrollViewer.ScrollToBottom();
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("Backup queue is empty");
                }
                catch (OperationCanceledException)
                {
                    backupNameLabel.Content = "CANCELED";
                }
            }

        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPaused == true)
            {
                isPaused = false;
                pauseButton.Content = "Pause";
                initiateBackupViewModel.resumeBackup();
            }
            else
            {
                isPaused = true;
                pauseButton.Content = "Resume";
                initiateBackupViewModel.pauseBackup();
            }

        }
    }
}
