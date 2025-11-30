using CrazyZoo.Classes;
using CrazyZoo.Generics;
using CrazyZoo.Interfaces;
using CrazyZoo.Properties;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.Models;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CrazyZoo
{
    public partial class App : System.Windows.Application
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
                string sql = string.Format("CREATE DATABASE [{0}];", databaseName);
                using var cmd = new SqlCommand(sql, newConn);
                cmd.ExecuteNonQuery();
            }
            using var finalConn = new SqlConnection(_connectionString);
            finalConn.Open();
            createTables(finalConn);
        }

        private void createTables(SqlConnection conn)
        {
            using (var cmd = new SqlCommand(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Enclosures' AND xtype='U')
            BEGIN
            CREATE TABLE [dbo].[Enclosures] (
                [Id]   INT            IDENTITY (1, 1) NOT NULL,
                [Name] NVARCHAR (100) NULL,
                PRIMARY KEY CLUSTERED ([Id] ASC)
            );
            END", conn))
            {
                cmd.ExecuteNonQuery();
            }
            using (var cmd = new SqlCommand(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Animals' AND xtype='U')
                BEGIN
                CREATE TABLE [dbo].[Animals](
                [Id]             INT            IDENTITY(1, 1) NOT NULL,
                [Name]           NVARCHAR(100) NULL,
                [Description]    NVARCHAR(100) NULL,
                [Age]            INT            NULL,
                [Type]           NVARCHAR(50)  NULL,
                [PreferableFood] NVARCHAR(100) NULL,
                [EnclosureId]    INT            NULL,
                PRIMARY KEY CLUSTERED([Id] ASC),
                CONSTRAINT[FK_Animals_Enclosures] FOREIGN KEY([EnclosureId]) REFERENCES[dbo].[Enclosures]([Id])
            );
            END", conn))
            {
                cmd.ExecuteNonQuery();
            }

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
