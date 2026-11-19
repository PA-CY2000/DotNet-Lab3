using Ass3.Data;
using Ass3.Views;

namespace Ass3
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            try
            {
                DatabaseInitializer.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization failed:\n{ex.Message}\n\nCheck PostgreSQL and TAXPAYER_DB_CONNECTION.", "Startup error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new LoginForm());
        }
    }
}