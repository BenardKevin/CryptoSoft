using System.Threading;
using System.Windows;

namespace EasySave_v3.src.main
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        static Mutex m = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string mutexName = "EasySavev3.exe";
            try
            {
                // Initializes a new instance of the Mutex class with a Boolean value that indicates 
                // whether the calling thread should have initial ownership of the mutex, a string that is the name of the mutex, 
                // and a Boolean value that, when the method returns, indicates whether the calling thread was granted initial ownership of the mutex.

                m = new Mutex(true, mutexName, out bool createdNew);

                if (!createdNew)
                {
                    MessageBox.Show("Application is already running !");
                    Current.Shutdown(); // Exit the application
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (m != null)
            {
                m.Dispose();
            }
            base.OnExit(e);
        }
    }
}
