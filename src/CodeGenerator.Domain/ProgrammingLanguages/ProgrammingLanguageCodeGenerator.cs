using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Domain.ProgrammingLanguages
{
    /// <summary>
    /// Base class for programming language code generators.
    /// Converts CodeElement structures to source code strings for a specific programming language.
    /// </summary>
    public abstract class ProgrammingLanguageCodeGenerator
    {
        /// <summary>
        /// The programming language this generator targets
        /// </summary>
        public abstract ProgrammingLanguage Language { get; }

        /// <summary>
        /// Default indentation string (spaces or tabs)
        /// </summary>
        public string IndentString { get; set; } = "    ";

        /// <summary>
        /// Current indentation level
        /// </summary>
        protected int IndentLevel { get; set; } = 0;

        /// <summary>
        /// Line ending string
        /// </summary>
        public string LineEnding { get; set; } = Environment.NewLine;

        #region Main Generation Methods

        /// <summary>
        /// Generate code for any CodeElement
        /// </summary>
        public virtual string GenerateCodeElement(CodeElement element)
        {
            return element switch
            {
                CodeFileElement file => GenerateCodeFile(file),
                NamespaceElement ns => GenerateNamespace(ns),
                ClassElement cls => GenerateClass(cls),
                InterfaceElement iface => GenerateInterface(iface),
                StructElement strct => GenerateStruct(strct),
                EnumElement enm => GenerateEnum(enm),
                DelegateElement del => GenerateDelegate(del),
                FieldElement field => GenerateField(field),
                PropertyElement prop => GenerateProperty(prop),
                MethodElement method => GenerateMethod(method),
                ConstructorElement ctor => GenerateConstructor(ctor),
                EventElement evt => GenerateEvent(evt),
                IndexerElement indexer => GenerateIndexer(indexer),
                OperatorElement op => GenerateOperator(op),
                UsingElement use => GenerateUsing(use),
                AttributeElement attr => GenerateAttribute(attr),
                ParameterElement param => GenerateParameter(param),
                _ => throw new NotSupportedException($"Code element type '{element.GetType().Name}' is not supported.")
            };
        }

        #endregion

        #region Abstract Generation Methods

        /// <summary>
        /// Generate a complete code file
        /// </summary>
        public abstract string GenerateCodeFile(CodeFileElement file);

        /// <summary>
        /// Generate a namespace declaration
        /// </summary>
        public abstract string GenerateNamespace(NamespaceElement ns);

        /// <summary>
        /// Generate a using/import statement
        /// </summary>
        public abstract string GenerateUsing(UsingElement use);

        /// <summary>
        /// Generate a class declaration
        /// </summary>
        public abstract string GenerateClass(ClassElement cls);

        /// <summary>
        /// Generate an interface declaration
        /// </summary>
        public abstract string GenerateInterface(InterfaceElement iface);

        /// <summary>
        /// Generate a struct declaration
        /// </summary>
        public abstract string GenerateStruct(StructElement strct);

        /// <summary>
        /// Generate an enum declaration
        /// </summary>
        public abstract string GenerateEnum(EnumElement enm);

        /// <summary>
        /// Generate a delegate declaration
        /// </summary>
        public abstract string GenerateDelegate(DelegateElement del);

        /// <summary>
        /// Generate a field declaration
        /// </summary>
        public abstract string GenerateField(FieldElement field);

        /// <summary>
        /// Generate a property declaration
        /// </summary>
        public abstract string GenerateProperty(PropertyElement prop);

        /// <summary>
        /// Generate a method declaration
        /// </summary>
        public abstract string GenerateMethod(MethodElement method);

        /// <summary>
        /// Generate a constructor declaration
        /// </summary>
        public abstract string GenerateConstructor(ConstructorElement ctor);

        /// <summary>
        /// Generate an event declaration
        /// </summary>
        public abstract string GenerateEvent(EventElement evt);

        /// <summary>
        /// Generate an indexer declaration
        /// </summary>
        public abstract string GenerateIndexer(IndexerElement indexer);

        /// <summary>
        /// Generate an operator declaration
        /// </summary>
        public abstract string GenerateOperator(OperatorElement op);

        /// <summary>
        /// Generate an attribute
        /// </summary>
        public abstract string GenerateAttribute(AttributeElement attr);

        /// <summary>
        /// Generate a parameter
        /// </summary>
        public abstract string GenerateParameter(ParameterElement param);

        /// <summary>
        /// Generate a type reference
        /// </summary>
        public abstract string GenerateTypeReference(TypeReference typeRef);

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get the current indentation string
        /// </summary>
        protected string GetIndent()
        {
            return string.Concat(Enumerable.Repeat(IndentString, IndentLevel));
        }

        /// <summary>
        /// Get indentation at a specific level
        /// </summary>
        protected string GetIndent(int level)
        {
            return string.Concat(Enumerable.Repeat(IndentString, level));
        }

        /// <summary>
        /// Increase indentation level
        /// </summary>
        protected void IncreaseIndent()
        {
            IndentLevel++;
        }

        /// <summary>
        /// Decrease indentation level
        /// </summary>
        protected void DecreaseIndent()
        {
            if (IndentLevel > 0)
                IndentLevel--;
        }

        /// <summary>
        /// Write a line with current indentation
        /// </summary>
        protected string Line(string content = "")
        {
            if (string.IsNullOrEmpty(content))
                return LineEnding;
            return GetIndent() + content + LineEnding;
        }

        /// <summary>
        /// Generate access modifier string
        /// </summary>
        public abstract string GenerateAccessModifier(AccessModifier modifier);

        /// <summary>
        /// Generate element modifiers string
        /// </summary>
        public abstract string GenerateModifiers(ElementModifiers modifiers);

        /// <summary>
        /// Generate documentation comment
        /// </summary>
        public abstract string GenerateDocumentation(string? documentation);

        #endregion
    }
}
