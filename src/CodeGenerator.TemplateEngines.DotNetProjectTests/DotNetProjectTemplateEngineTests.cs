using CodeGenerator.Domain.DotNet;
using CodeGenerator.TemplateEngines.DotNetProject;
using CodeGenerator.TemplateEngines.DotNetProject.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.TemplateEngines.DotNetProject.Tests
{
    [TestClass()]
    public class DotNetProjectTemplateEngineTests
    {
        private ILogger<DotNetProjectTemplateEngine> GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<DotNetProjectTemplateEngine>();
        }
        private DotNetProjectTemplateEngine GetEngine()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var engineLogger = loggerFactory.CreateLogger<DotNetProjectTemplateEngine>();
            var serviceLogger = loggerFactory.CreateLogger<DotNetProjectService>();
            var dotNetService = new DotNetProjectService(serviceLogger);
            var engine = new DotNetProjectTemplateEngine(dotNetService, engineLogger);
            return engine;
        }
        [TestMethod()]
        public async Task RenderAsyncTest()
        {
            var engine = GetEngine();
            var template = new DotNetProjectTemplate("dotnetconsoleapplication", DotNetProjectType.ConsoleApp, DotNetLanguages.CSharp, TargetFrameworks.Net8);
            var templateInstance = new DotNetProjectTemplateInstance(template, "TestConsoleApp");

            var result = await engine.RenderAsync(templateInstance, CancellationToken.None);

            result.Artifacts.ToList().ForEach(a => 
            {
                Console.WriteLine($"Artifact: {a.TreeNodeText} ({a.TreeNodeIcon.IconKey})");
                if (a is CodeGenerator.Core.Artifacts.FileSystem.FileArtifact fileArtifact)
                {
                    Console.WriteLine(fileArtifact.GetTextContext());
                }
            });

            Assert.IsTrue(result.Artifacts.Count() > 0, "No artifacts were generated.");
        }

        [TestMethod()]
        public async Task RenderAsyncClass1CsRemovedTest()
        {
            var engine = GetEngine();
            var template = new DotNetProjectTemplate("dotnetconsoleapplication", DotNetProjectType.ClassLib, DotNetLanguages.CSharp, TargetFrameworks.Net8);
            var templateInstance = new DotNetProjectTemplateInstance(template, "TestLibrary");

            var result = await engine.RenderAsync(templateInstance, CancellationToken.None);

            result.Artifacts.ToList().ForEach(a =>
            {
                Console.WriteLine($"Artifact: {a.TreeNodeText} ({a.TreeNodeIcon.IconKey})");
                if (a is CodeGenerator.Core.Artifacts.FileSystem.FileArtifact fileArtifact)
                {
                    Console.WriteLine(fileArtifact.GetTextContext());
                }
            });
            var class1CsArtifact = result.Artifacts.FirstOrDefault(a => a.TreeNodeText.Equals("Class1.cs", StringComparison.OrdinalIgnoreCase));
            Assert.IsNull(class1CsArtifact, "Class1.cs should have been removed from the generated artifacts.");
            Assert.IsTrue(result.Artifacts.Count() > 0, "No artifacts were generated.");
        }
    }
}