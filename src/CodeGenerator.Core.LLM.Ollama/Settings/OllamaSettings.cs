namespace CodeGenerator.Core.LLM.Ollama.Settings
{
    /// <summary>
    /// Configuration settings for the Ollama LLM provider
    /// </summary>
    public class OllamaSettings
    {
        /// <summary>Base URL of the Ollama API (default: http://localhost:11434)</summary>
        public string BaseUrl { get; set; } = "http://localhost:11434";

        /// <summary>Default model to use (e.g. "llama3.2", "mistral", "codellama")</summary>
        public string DefaultModel { get; set; } = "qwen2.5-coder:7b";
    }
}
