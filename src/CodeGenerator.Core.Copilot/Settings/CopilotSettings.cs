namespace CodeGenerator.Core.Copilot.Settings
{
    public class CopilotSettings
    {
        private static readonly CopilotSettings _instance = new CopilotSettings();

        public static CopilotSettings Instance => _instance;

        private CopilotSettings()
        {
        }

        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}