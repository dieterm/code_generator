using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Windows.Forms;
using System.Diagnostics;

namespace ProjectXYZ.Presentation.WinForms
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

            if (!ConfigureSyncfusionFramework())
            {
                return;
            }

            Application.Run(new MainView());
        }

        private static bool ConfigureSyncfusionFramework()
        {
            var currentFolder = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var licensesPath = Path.Combine(currentFolder!, "SyncfusionLicenseKey.txt");
            Debug.WriteLine(licensesPath);
            if (!File.Exists(licensesPath))
            {
                // Show input prompt to enter license key
                string licenseKey = Microsoft.VisualBasic.Interaction.InputBox("No licence key found at " + licensesPath + ".\r\nEnter Syncfusion License Key:", "License Key Required", "");
                if (string.IsNullOrWhiteSpace(licenseKey))
                {
                    MessageBox.Show("A valid Syncfusion license key is required to run this application.", "License Key Missing", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return false;
                }
                File.WriteAllText(licensesPath, licenseKey);
            }

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(File.ReadAllText(licensesPath));
            SkinManager.LoadAssembly(typeof(Syncfusion.WinForms.Themes.Office2019Theme).Assembly);
            return true;
        }
    }
}