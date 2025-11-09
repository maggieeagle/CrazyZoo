using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using CrazyZoo.Interfaces;
using CrazyZoo.Classes;

namespace CrazyZoo
{
    public partial class App : Application
    {
        public static IServiceProvider Services;

        public App()
        {
            var sc = new ServiceCollection();
            sc.AddTransient<ILogger, XmlLogger>();
            sc.AddTransient<ZooViewModel>();
            sc.AddTransient<MainWindow>();
            Services = sc.BuildServiceProvider();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.DataContext = Services.GetRequiredService<ZooViewModel>();
            mainWindow.Show();
        }
    }
}
