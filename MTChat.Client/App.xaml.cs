using System;
using System.Windows;

namespace MTChat.Client
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            new UnityContainerBootstrapper().Run();
        }
    }
}
