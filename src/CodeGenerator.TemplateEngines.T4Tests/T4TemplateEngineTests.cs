using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.TemplateEngines.T4;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.TemplateEngines.T4.Tests
{
    [TestClass()]
    public class T4TemplateEngineTests
    {
        private ILogger<T4TemplateEngine> GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<T4TemplateEngine>();
        }

        private T4TemplateEngine GetEngine()
        {
            var logger = GetLogger();
            var templateRootFolder = @"D:\Cloud\GitHub\code_generator\src\CodeGenerator.TemplateEngines.T4Tests\";
            return new T4TemplateEngine(logger, templateRootFolder);
        }

        [TestMethod()]
        public async Task RenderAsyncTest()
        {
            var cancellationToken = CancellationToken.None;
            // Gebruik via T4TemplateInstance
            var templatePath = @"D:\Cloud\GitHub\code_generator\src\CodeGenerator.TemplateEngines.T4Tests\Template.tt";
            var template = new T4FileTemplate("myTemplate", templatePath);
            var instance = new T4TemplateInstance(template)
            {
                OutputFileName = "Output.cs"
            };
            instance.Parameters["EntityName"] = "Customer";
            instance.Parameters["Namespace"] = "MyApp.Domain";
            var engine = GetEngine();
            var result = await engine.RenderAsync(instance, cancellationToken);

            if (result.Succeeded)
            {
                // result.Artifacts bevat de gegenereerde bestanden
                var output = ((FileArtifact)result.Artifacts.Single()).GetTextContext();
                Assert.IsTrue(output!.Contains("namespace MyApp.Domain"), "Output should contain namespace");
                Assert.IsTrue(output!.Contains("public class Customer"), "Output should contain class name");
                Assert.IsTrue(output!.Contains("public string Name"), "Output should contain Name property from class feature");
                Assert.IsTrue(output!.Contains("public bool Validate()"), "Output should contain Validate method from class feature");
            }
            else
            {
                Assert.Fail("Template rendering failed: " + string.Join(", ", result.Errors));
            }
        }

        [TestMethod()]
        public async Task RenderAsyncWithIncludeTest()
        {
            var cancellationToken = CancellationToken.None;
            // Test template met include directive
            var templatePath = @"D:\Cloud\GitHub\code_generator\src\CodeGenerator.TemplateEngines.T4Tests\TemplateWithInclude.tt";
            var template = new T4FileTemplate("templateWithInclude", templatePath);
            var instance = new T4TemplateInstance(template)
            {
                OutputFileName = "OutputWithInclude.cs"
            };
            instance.Parameters["EntityName"] = "Order";
            instance.Parameters["Namespace"] = "MyApp.Sales";
            var engine = GetEngine();
            var result = await engine.RenderAsync(instance, cancellationToken);

            if (result.Succeeded)
            {
                // result.Artifacts bevat de gegenereerde bestanden
                var output = ((FileArtifact)result.Artifacts.Single()).GetTextContext();
                Assert.IsTrue(output!.Contains("namespace MyApp.Sales"), "Output should contain namespace");
                Assert.IsTrue(output!.Contains("public class Order"), "Output should contain class name");
                Assert.IsTrue(output!.Contains("Properties from ChildTemplate.tt"), "Output should contain content from included child template");
                Assert.IsTrue(output!.Contains("public string Name"), "Output should contain Name property from child template");
                Assert.IsTrue(output!.Contains("public bool Validate()"), "Output should contain Validate method from child template");
            }
            else
            {
                Assert.Fail("Template rendering failed: " + string.Join(", ", result.Errors));
            }
        }
    }
}