using projet_easy_save_v2.src.viewModels;
using System.Windows;
using System.Windows.Controls;

namespace projet_easy_save_v2.src.views
{
    /// <summary>
    /// Logique d'interaction pour ParamsView.xaml
    /// </summary>
    public partial class ParamsView : UserControl
    {
        private readonly ParamsViewModel paramsViewModel;
        public ParamsView()
        {
            InitializeComponent();
            paramsViewModel = new ParamsViewModel();
            DataContext = paramsViewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (paramsViewModel.SaveParams(LanguageComboBox.Text, EncryptionToggler.IsChecked, FileExtensionToCrypt.Items))
            {
                MessageBox.Show("Application must restart to change language");
                // Shutdowns the application.
                Application.Current.Shutdown();
            } else
            {
                MessageBox.Show("Saved Successfully");
            }

        }

        private void AddFileExtension(object sender, RoutedEventArgs e)
        {
            try
            {
                paramsViewModel.AddFileExtension(newFileExtension.Text);
            }
            catch
            {
                // TODO: handle error directly on textbox.
                MessageBox.Show("File extension cannot be null");
            }

            newFileExtension.Text = "";
        }

        private void RemoveFileExtension(object sender, RoutedEventArgs e)
        {
            try
            {
                paramsViewModel.RemoveFileExtension(FileExtensionToCrypt.SelectedItem);
            } catch
            {
                MessageBox.Show("Please select an item berore removing");
            }
        }

        private void FileEncryption_Checked(object sender, RoutedEventArgs e)
        {
            fileExtensionsList.IsEnabled = true;
        }

        private void FileEncryption_Unchecked(object sender, RoutedEventArgs e)
        {
            fileExtensionsList.IsEnabled = false;
        }
    }
}
