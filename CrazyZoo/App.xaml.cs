using CrazyZoo.Classes;
using CrazyZoo.Generics;
using CrazyZoo.Interfaces;
using CrazyZoo.Properties;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;

namespace CrazyZoo
{
    public partial class App : Application
    {
        public static IServiceProvider Services;
        private string _connectionString;

        public App()
        {
            _connectionString = Resource1.dbConnectionString;

            var sc = new ServiceCollection();
            sc.AddTransient<ILogger, XmlLogger>();
            sc.AddTransient<ZooViewModel>();
            sc.AddTransient<MainWindow>();
            sc.AddSingleton<IRepository<Animal>>(sp => new DbRepository<Animal>(_connectionString));
            sc.AddSingleton<IRepository<Enclosure<Animal>>>(sp => new DbEnclosureRepository(_connectionString));
            Services = sc.BuildServiceProvider();

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            createDBifNotExists();
            clearDB();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.DataContext = Services.GetRequiredService<ZooViewModel>();
            mainWindow.Show();
        }

        private void createDBifNotExists()
        {
            using var conn = new SqlConnection(_connectionString);
            try
            {
                conn.Open();
            }
            catch (SqlException)
            {
                var csb = new SqlConnectionStringBuilder(_connectionString);
                string databaseName = csb.InitialCatalog;
                csb.InitialCatalog = "master";
                using var newConn = new SqlConnection(csb.ToString());
                newConn.Open();
                string sql = $@"
                    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{databaseName}')
                    CREATE DATABASE [{databaseName}];
                ";

                using var cmd = new SqlCommand(sql, newConn);
                cmd.ExecuteNonQuery();
            }
            using var finalConn = new SqlConnection(_connectionString);
            finalConn.Open();
        }

        private void clearDB()
        {

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using (var cmd = new SqlCommand(Resource1.dbDeleteAnimals, conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SqlCommand(Resource1.dbDeleteEnclosures, conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SqlCommand(Resource1.dbDropAnimalsIdCounter, conn))
            {
                cmd.ExecuteNonQuery();
            }
            using (var cmd = new SqlCommand(Resource1.dbDropEnclosuresIdCounter, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}
