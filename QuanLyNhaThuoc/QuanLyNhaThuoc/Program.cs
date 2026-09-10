namespace QuanLyNhaThuoc
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
            // Create main form instance so we can apply a global UI theme before showing it
            var mainForm = new FormMain();
            UITheme.ApplyToForm(mainForm);
            Application.Run(mainForm);
        }
    }
}