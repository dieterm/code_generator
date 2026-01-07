namespace CodeGenerator.Domain.Tests
{
    [TestClass]
    public sealed class Test1
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // This method is called once for the test assembly, before any tests are run.
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // This method is called once for the test assembly, after all tests are run.
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            // This method is called once for the test class, before any tests of the class are run.
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // This method is called once for the test class, after all tests of the class are run.
        }

        [TestInitialize]
        public void TestInit()
        {
            // This method is called before each test method.
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // This method is called after each test method.
        }

        [TestMethod]
        public void TestFileArtifact()
        {
            // Arrange
            var file = new FileArtifact();
            // Act
            file.FileName = "example";
            file.Extension = ".txt";
            file.Size = 1024;
            
            // Assert
            Assert.AreEqual("example", file.FileName);
            Assert.AreEqual(".txt", file.Extension);
            Assert.AreEqual(1024, file.Size);
            Assert.IsTrue(file.CanPreview);
            var filePreview = file.CreatePreview();
            Assert.IsInstanceOfType(filePreview, typeof(string));
            Assert.AreEqual("Hello world", filePreview);
            
        }


        [TestMethod]
        public void TestMessagebus()
        {
            // Arrange
            var messageBus = new TestMessageBus();
            var genA = new GeneratorA(messageBus);
            var genB = new GeneratorB(messageBus);
            // Act
            var artifact = genA.PublishEvent();
            // Assert
            Assert.IsNotNull(artifact);
            Assert.IsTrue(artifact.CanPreview);
            var preview = artifact.CreatePreview();
            Assert.IsInstanceOfType(preview, typeof(string));
            Assert.AreEqual("New preview content from GeneratorB", preview);
        }
    }
}
