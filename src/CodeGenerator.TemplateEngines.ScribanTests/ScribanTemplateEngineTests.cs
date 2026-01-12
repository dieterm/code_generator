using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeGenerator.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CodeGenerator.Core.Templates;
using CodeGenerator.TemplateEngines.Scriban;
using CodeGenerator.Core.Artifacts.FileSystem;

namespace CodeGenerator.Templates.Tests
{
    [TestClass()]
    public class ScribanTemplateEngineTests
    {
        private ILogger<ScribanTemplateEngine> GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<ScribanTemplateEngine>();
        }
        private ScribanTemplateEngine GetEngine()
        {
            var logger = GetLogger();
            // @"D:\Cloud\GitHub\code_generator\src\CodeGenerator.TemplateEngines.ScribanTests\"
            return new ScribanTemplateEngine(logger);

        }
        [TestMethod()]
        public void RenderAsyncTest()
        {
            var engine = GetEngine();
            var templateContent = "Hello, {{ name }}!";
            var model = new { name = "World" };
            var result = engine.RenderAsync(templateContent, model).Result;
            Assert.AreEqual("Hello, World!", result);
        }

        [TestMethod()]
        public void SupportsTemplateFileExtensionTest()
        {
            var engine = GetEngine();
            Assert.IsTrue(engine.SupportsTemplateFileExtension("scriban"));
            Assert.IsFalse(engine.SupportsTemplateFileExtension("tt"));
            Assert.IsFalse(engine.SupportsTemplateFileExtension("txt"));
        }

        [TestMethod()]
        public void SupportsTemplatePathTest()
        {
            var engine = GetEngine();
            Assert.IsTrue(engine.SupportsTemplatePath("template.scriban"));
            Assert.IsFalse(engine.SupportsTemplatePath("template.tt"));
            Assert.IsFalse(engine.SupportsTemplatePath("template.txt"));
        }

        [TestMethod()]
        public void SupportsTemplateTypeTest()
        {
            var engine = GetEngine();
            Assert.IsTrue(engine.SupportsTemplateType(TemplateType.Scriban));
            Assert.IsFalse(engine.SupportsTemplateType(TemplateType.T4));
            Assert.IsFalse(engine.SupportsTemplateType(TemplateType.DotNetProject));
        }

        [TestMethod()]
        public void SupportsTemplateTest()
        {
            var engine = GetEngine();
            var stringTemplate = new ScribanStringTemplate("template1","Hello, {{ name }}!");
            Assert.IsTrue(engine.SupportsTemplate(stringTemplate));
            var fileTemplate = new ScribanFileTemplate("template2","/path/to/template.scriban");
            Assert.IsTrue(engine.SupportsTemplate(fileTemplate));
        }

        [TestMethod()]
        public async Task RenderScribanStringTemplateInstanceTest()
        {
            var engine = GetEngine();
            var stringTemplate = new ScribanStringTemplate("template1", "Hello, {{ name }}!");
            var templateInstance = new ScribanTemplateInstance(stringTemplate);
            templateInstance.Parameters["name"] = "Tester";
            var result = await engine.RenderAsync(templateInstance, CancellationToken.None);
            Assert.AreEqual("Hello, Tester!", ((FileArtifact)result.Artifacts.Single()).GetTextContent());
        }

        [TestMethod()]
        public async Task RenderScribanFileTemplateInstanceTest()
        {
            var engine = GetEngine();
            var fileTemplate = new ScribanFileTemplate("template2", @"D:\Cloud\GitHub\code_generator\src\CodeGenerator.TemplateEngines.ScribanTests\HelloTemplate.scriban");
            var templateInstance = new ScribanTemplateInstance(fileTemplate);
            templateInstance.Parameters["name"] = "FileTester";
            var result = await engine.RenderAsync(templateInstance, CancellationToken.None);
            Assert.AreEqual("Hello, FileTester!", ((FileArtifact)result.Artifacts.Single()).GetTextContent());
        }

        [TestMethod()]
        public async Task RenderScribanFileTemplateInstanceHelperMethodsTest()
        {
            var engine = GetEngine();
            var fileTemplate = new ScribanFileTemplate("template2", @"D:\Cloud\GitHub\code_generator\src\CodeGenerator.TemplateEngines.ScribanTests\HelloTemplateHelperMethod.scriban");
            var templateInstance = new ScribanTemplateInstance(fileTemplate);
            templateInstance.Parameters["name"] = "FileTester";
            templateInstance.Parameters["countries"] = new List<string> { "Belgium", "France" };
            templateInstance.Functions["myhelper"] = new Func<string, string>(s => s.ToUpper());
            var result = await engine.RenderAsync(templateInstance, CancellationToken.None);
            var actualOutput = ((FileArtifact)result.Artifacts.Single()).GetTextContent();
            var expectedOutput = "Hello, FileTester!\r\nHello, file_tester!\r\nHello, FILETESTER!\r\n- Belgium\r\n- France";
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [TestMethod()]
        public async Task RenderScribanFileTemplateInstanceWithTemplateResolver()
        {
            var engine = GetEngine();
            var fileTemplate = new ScribanFileTemplate("template2", @"D:\Cloud\GitHub\code_generator\src\CodeGenerator.TemplateEngines.ScribanTests\ParentTemplate.scriban");
            var templateInstance = new ScribanTemplateInstance(fileTemplate);
            var result = await engine.RenderAsync(templateInstance, CancellationToken.None);
            var output = ((FileArtifact)result.Artifacts.Single()).GetTextContent();
            Assert.AreEqual("Hello, TESTER!", output);
        }
    }
}