using Microsoft.Data.Sqlite;
using RaceTrade;
using RaceTrader;
using SQLitePCL;
using System;
using System.IO;
using System.Windows.Forms;

namespace WinFormsTraderApp
{
    static class Program
    {
        [STAThread]  // Required for Windows Forms applications
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Console.WriteLine("Creating required directories...");
                CreateRequiredDirectories();
                Console.WriteLine("Directories created successfully.");

                Console.WriteLine("Initializing database...");
                SQLiteHelper.InitializeDatabase();
                IMDBCache.Initialize();
                TVMazeCache.Initialize();
                Console.WriteLine("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during initialization: {ex.Message}");
                MessageBox.Show($"Error during initialization: {ex.Message}", "Initialization Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            RaceHelper.LoadAllSiteConfigs();
            Application.Run(new RaceTrade.MainApp());
        }

        private static void CreateRequiredDirectories()
        {
            string[] requiredDirectories = new string[]
            {
                "sites",
                "cbftp",
                "sections",
                "db",
                "pre_bots",
                "settings"
            };

            foreach (string dir in requiredDirectories)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    Console.WriteLine($"Created directory: {dir}");
                }
                else
                {
                    Console.WriteLine($"Directory already exists: {dir}");
                }
            }
        }
    }
}