namespace CodeGenerator.Core.Generators
{
    public abstract class GeneratorContextEventArgs : EventArgs
    {
        public GenerationResult Result { get; }
        protected GeneratorContextEventArgs(GenerationResult result)
        {
            Result = result;
        }
    }
}
