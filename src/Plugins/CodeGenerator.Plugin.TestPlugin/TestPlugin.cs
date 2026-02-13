using CodeGenerator.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Plugin.TestPlugin
{
    public class TestPlugin : IPlugin
    {
        public string Id => nameof(TestPlugin);

        public string Name => "Test Plugin";

        public Version Version => new Version(1, 0, 0);

        public string Description => "This is a test plugin.";

        public PluginScope Scope => PluginScope.Workspace;

        public void Dispose()
        {
            // Clean up any resources if necessary
        }

        public void Initialize(IPluginContext context)
        {
            context.RegisterWorkspaceSubscriber(new TestWorkspaceSubscriber(context.CreateLogger<TestWorkspaceSubscriber>()));
        }

        public void Shutdown()
        {
            // Perform any necessary cleanup on shutdown
        }
    }
}
