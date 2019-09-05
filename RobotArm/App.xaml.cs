using System.Windows;

namespace Motion_and_vision
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainView = new MainWindow()
                           {
                               Title = "Inverse Kine",
                               WindowStartupLocation = WindowStartupLocation.CenterScreen
                           };

            mainView.Show();
        }

    }
}
