using projet_easy_save_v2.src.viewModels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace projet_easy_save_v2.src.views
{
    /// <summary>
    /// Logique d'interaction pour MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
       private readonly BrushConverter bc = new BrushConverter();
        private readonly MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();
        public MainWindowView()
        {
            mainWindowViewModel = new MainWindowViewModel();
            DataContext = mainWindowViewModel;


            InitializeComponent();

            // Home menu is the default page
            _mainFrame.Navigate(new HomeMenuView());
            HomeButton.Background = (Brush)bc.ConvertFrom("#23233E");

        }


        private void HomeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            BackupListButton.Background = Brushes.Transparent;
            CreateBackupButton.Background = Brushes.Transparent;
            InitiateBackupButton.Background = Brushes.Transparent;
            SettingsButton.Background = Brushes.Transparent;
            HomeButton.Background = (Brush)bc.ConvertFrom("#23233E");
            _mainFrame.Navigate(new HomeMenuView());
        }
        private void BackupListItem_Click(object sender, RoutedEventArgs e)
        {
            HomeButton.Background = Brushes.Transparent;
            CreateBackupButton.Background = Brushes.Transparent;
            InitiateBackupButton.Background = Brushes.Transparent;
            SettingsButton.Background = Brushes.Transparent;
            BackupListButton.Background = (Brush)bc.ConvertFrom("#23233E");
            _mainFrame.Navigate(new BackupStrategiesListView());
        }
        private void CreateBackupItem_Click(object sender, RoutedEventArgs e)
        {
            HomeButton.Background = Brushes.Transparent;
            InitiateBackupButton.Background = Brushes.Transparent;
            SettingsButton.Background = Brushes.Transparent;
            BackupListButton.Background = Brushes.Transparent;
            CreateBackupButton.Background = (Brush)bc.ConvertFrom("#23233E");
            _mainFrame.Navigate(new CreateBackupStrategyView());
        }
        private void InitiateBackupItem_Click(object sender, RoutedEventArgs e)
        {
            HomeButton.Background = Brushes.Transparent;
            SettingsButton.Background = Brushes.Transparent;
            BackupListButton.Background = Brushes.Transparent;
            CreateBackupButton.Background = Brushes.Transparent;
            InitiateBackupButton.Background = (Brush)bc.ConvertFrom("#23233E");
            _mainFrame.Navigate(new InitiateBackupView());
        }
        private void SettingsItem_Click(object sender, RoutedEventArgs e)
        {
            HomeButton.Background = Brushes.Transparent;
            BackupListButton.Background = Brushes.Transparent;
            CreateBackupButton.Background = Brushes.Transparent;
            InitiateBackupButton.Background = Brushes.Transparent;
            SettingsButton.Background = (Brush)bc.ConvertFrom("#23233E");
            _mainFrame.Navigate(new ParamsView());
        }

        private void MinimizeAppButton_Down(object sender, MouseButtonEventArgs e)
        {
            // Minimizes application window.
            WindowState = WindowState.Minimized;
        }

        private void CloseAppButton_Down(object sender, MouseButtonEventArgs e)
        {
            // Shutdowns the application.
            Application.Current.Shutdown();
        }

        private void DraggableWindow_Down(object sender, MouseButtonEventArgs e)
        {
            // Sets window as draggable.
            DragMove();
        }
    }
}
