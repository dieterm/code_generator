namespace CodeGenerator.Core.Copilot.Settings
{
    public class CopilotSettings
    {
        private static readonly CopilotSettings _instance = new CopilotSettings();

        public static CopilotSettings Instance => _instance;

        private CopilotSettings()
        {
        }

        /// <summary>
        /// GitHub Personal Access Token (PAT) with Copilot access.
        /// Required scope: copilot
        /// </summary>
        public string? GitHubToken { get; set; }
    }
}