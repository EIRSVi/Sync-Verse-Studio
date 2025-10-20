namespace SyncVerseStudio
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Initialize QuestPDF License
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            
            Application.Run(new LoginForm());
        }
    }
}