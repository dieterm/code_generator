using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.CodeElements.Statements;
using CodeGenerator.Domain.ProgrammingLanguages;
using CodeGenerator.Domain.ProgrammingLanguages.CSharp;
using CodeGenerator.Domain.ProgrammingLanguages.Python;
using System.Text;

namespace CodeGenerator.Domain.ProgrammingLanguages.CSharp
{
    /// <summary>
    /// Code generator for C# programming language.
    /// Converts CodeElement structures to C# source code.
    /// </summary>
    public class CSharpCodeGenerator : ProgrammingLanguageCodeGenerator
    {
        public override ProgrammingLanguage Language => CSharpLanguage.Instance;

        #region Code File

        public override string GenerateCodeFile(CodeFileElement file)
        {
            if(file.RawCode!=null) return file.RawCode;

            var sb = new StringBuilder();

            // File header
            if (!string.IsNullOrEmpty(file.FileHeader))
            {
                sb.Append(GenerateFileHeader(file.FileHeader));
            }

            // Nullable context
            if (file.NullableContext.HasValue)
            {
                sb.AppendLine($"#nullable {(file.NullableContext.Value ? "enable" : "disable")}");
                sb.AppendLine();
            }

            // Global usings
            foreach (var globalUsing in file.GlobalUsings)
            {
                sb.AppendLine($"global using {globalUsing.Namespace};");
            }
            if (file.GlobalUsings.Count > 0)
                sb.AppendLine();

            // Regular usings
            foreach (var use in file.Usings)
            {
                sb.AppendLine(GenerateUsing(use).TrimEnd());
            }
            if (file.Usings.Count > 0)
                sb.AppendLine();

            // Namespaces
            foreach (var ns in file.Namespaces)
            {
                sb.Append(GenerateNamespace(ns));
            }

            // Top-level types (outside namespace)
            foreach (var type in file.TopLevelTypes)
            {
                sb.Append(GenerateCodeElement(type));
                sb.AppendLine();
            }

            // Top-level statements
            foreach (var statement in file.TopLevelStatements)
            {
                sb.AppendLine(statement);
            }

            return sb.ToString();
        }

        private string GenerateFileHeader(string header)
        {
            var sb = new StringBuilder();
            var lines = header.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                sb.AppendLine($"// {line}");
            }
            sb.AppendLine();
            return sb.ToString();
        }

        #endregion

        #region Namespace

        public override string GenerateNamespace(NamespaceElement ns)
        {
            if (ns.RawCode != null) return ns.RawCode;
            var sb = new StringBuilder();

            // Usings within namespace
            foreach (var use in ns.Usings)
            {
                sb.AppendLine(GenerateUsing(use).TrimEnd());
            }
            if (ns.Usings.Count > 0)
                sb.AppendLine();

            if (ns.IsFileScoped)
            {
                // File-scoped namespace (C# 10+)
                sb.AppendLine($"namespace {ns.FullName};");
                sb.AppendLine();

                foreach (var type in ns.Types)
                {
                    sb.Append(GenerateCodeElement(type));
                    sb.AppendLine();
                }
            }
            else
            {
                // Block-scoped namespace
                sb.AppendLine($"namespace {ns.FullName}");
                sb.AppendLine("{");
                IncreaseIndent();

                foreach (var type in ns.Types)
                {
                    sb.Append(GenerateCodeElement(type));
                    sb.AppendLine();
                }

                DecreaseIndent();
                sb.AppendLine("}");
            }

            // Nested namespaces
            foreach (var nestedNs in ns.NestedNamespaces)
            {
                sb.Append(GenerateNamespace(nestedNs));
            }

            return sb.ToString();
        }

        #endregion

        #region Using

        public override string GenerateUsing(UsingElement use)
        {
            if (use.RawCode != null) return use.RawCode;
            var sb = new StringBuilder();

            if (use.IsGlobal)
                sb.Append("global ");

            sb.Append("using ");

            if (use.IsStatic)
                sb.Append("static ");

            if (!string.IsNullOrEmpty(use.Alias))
            {
                sb.Append($"{use.Alias} = ");
            }

            sb.Append(use.Namespace);
            sb.Append(';');

            return sb.ToString();
        }

        #endregion

        #region Class

        public override string GenerateClass(ClassElement cls)
        {
            if (cls.RawCode != null) return cls.RawCode;
            var sb = new StringBuilder();

            // Documentation
            sb.Append(GenerateDocumentation(cls.Documentation));

            // Attributes
            foreach (var attr in cls.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            // Class declaration
            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append(GenerateAccessModifier(cls.AccessModifier));
            declaration.Append(GenerateModifiers(cls.Modifiers));

            if (cls.IsRecord)
                declaration.Append("record ");
            else
                declaration.Append("class ");

            declaration.Append(cls.Name);

            // Generic type parameters
            if (cls.IsGeneric)
            {
                declaration.Append('<');
                declaration.Append(string.Join(", ", cls.GenericTypeParameters.Select(g => GenerateGenericTypeParameter(g))));
                declaration.Append('>');
            }

            // Primary constructor parameters (for records or C# 12)
            if (cls.PrimaryConstructorParameters.Count > 0)
            {
                declaration.Append('(');
                declaration.Append(string.Join(", ", cls.PrimaryConstructorParameters.Select(p => GenerateParameter(p))));
                declaration.Append(')');
            }

            // Base types
            if (cls.BaseTypes.Count > 0)
            {
                declaration.Append(" : ");
                declaration.Append(string.Join(", ", cls.BaseTypes.Select(b => GenerateTypeReference(b))));
            }

            sb.AppendLine(declaration.ToString());

            // Generic constraints
            foreach (var constraint in cls.GenericConstraints)
            {
                sb.AppendLine(Line($"    where {GenerateGenericConstraint(constraint)}").TrimEnd());
            }

            // Class body
            sb.AppendLine(Line("{").TrimEnd());
            IncreaseIndent();

            // Fields
            foreach (var field in cls.Fields)
            {
                sb.Append(GenerateField(field));
            }
            if (cls.Fields.Count > 0 && (cls.Properties.Count > 0 || cls.Constructors.Count > 0 || cls.Methods.Count > 0))
                sb.AppendLine();

            // Properties
            foreach (var prop in cls.Properties)
            {
                sb.Append(GenerateProperty(prop));
            }
            if (cls.Properties.Count > 0 && (cls.Constructors.Count > 0 || cls.Methods.Count > 0))
                sb.AppendLine();

            // Events
            foreach (var evt in cls.Events)
            {
                sb.Append(GenerateEvent(evt));
            }
            if (cls.Events.Count > 0 && (cls.Constructors.Count > 0 || cls.Methods.Count > 0))
                sb.AppendLine();

            // Constructors
            foreach (var ctor in cls.Constructors)
            {
                sb.Append(GenerateConstructor(ctor, cls.Name));
                sb.AppendLine();
            }

            // Finalizer
            if (cls.Finalizer != null)
            {
                sb.Append(GenerateFinalizer(cls.Finalizer, cls.Name));
                sb.AppendLine();
            }

            // Indexers
            foreach (var indexer in cls.Indexers)
            {
                sb.Append(GenerateIndexer(indexer));
                sb.AppendLine();
            }

            // Operators
            foreach (var op in cls.Operators)
            {
                sb.Append(GenerateOperator(op));
                sb.AppendLine();
            }

            // Methods
            foreach (var method in cls.Methods)
            {
                sb.Append(GenerateMethod(method));
                sb.AppendLine();
            }

            // Nested types
            foreach (var nestedType in cls.NestedTypes)
            {
                sb.Append(GenerateCodeElement(nestedType));
                sb.AppendLine();
            }

            DecreaseIndent();
            sb.AppendLine(Line("}").TrimEnd());

            return sb.ToString();
        }

        #endregion

        #region Interface

        public override string GenerateInterface(InterfaceElement iface)
        {
            if (iface.RawCode != null) return iface.RawCode;

            var sb = new StringBuilder();

            // Documentation
            sb.Append(GenerateDocumentation(iface.Documentation));

            // Attributes
            foreach (var attr in iface.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            // Interface declaration
            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append(GenerateAccessModifier(iface.AccessModifier));
            declaration.Append("interface ");
            declaration.Append(iface.Name);

            // Generic type parameters
            if (iface.IsGeneric)
            {
                declaration.Append('<');
                declaration.Append(string.Join(", ", iface.GenericTypeParameters.Select(g => GenerateGenericTypeParameter(g))));
                declaration.Append('>');
            }

            // Base types
            if (iface.BaseTypes.Count > 0)
            {
                declaration.Append(" : ");
                declaration.Append(string.Join(", ", iface.BaseTypes.Select(b => GenerateTypeReference(b))));
            }

            sb.AppendLine(declaration.ToString());

            // Generic constraints
            foreach (var constraint in iface.GenericConstraints)
            {
                sb.AppendLine(Line($"    where {GenerateGenericConstraint(constraint)}").TrimEnd());
            }

            // Interface body
            sb.AppendLine(Line("{").TrimEnd());
            IncreaseIndent();

            // Properties
            foreach (var prop in iface.Properties)
            {
                sb.Append(GenerateInterfaceProperty(prop));
            }

            // Events
            foreach (var evt in iface.Events)
            {
                sb.Append(GenerateInterfaceEvent(evt));
            }

            // Methods
            foreach (var method in iface.Methods)
            {
                sb.Append(GenerateInterfaceMethod(method));
            }

            // Indexers
            foreach (var indexer in iface.Indexers)
            {
                sb.Append(GenerateInterfaceIndexer(indexer));
            }

            DecreaseIndent();
            sb.AppendLine(Line("}").TrimEnd());

            return sb.ToString();
        }

        private string GenerateInterfaceProperty(PropertyElement prop)
        {
            var sb = new StringBuilder();
            sb.Append(GenerateDocumentation(prop.Documentation));

            var line = new StringBuilder();
            line.Append(GetIndent());
            line.Append(GenerateTypeReference(prop.Type));
            line.Append(' ');
            line.Append(prop.Name);
            line.Append(" { ");

            if (prop.HasGetter) line.Append("get; ");
            if (prop.HasSetter)
            {
                if (prop.IsInitOnly)
                    line.Append("init; ");
                else
                    line.Append("set; ");
            }
            line.Append('}');

            sb.AppendLine(line.ToString());
            return sb.ToString();
        }

        private string GenerateInterfaceMethod(MethodElement method)
        {
            var sb = new StringBuilder();
            sb.Append(GenerateDocumentation(method.Documentation));

            var line = new StringBuilder();
            line.Append(GetIndent());
            line.Append(GenerateTypeReference(method.ReturnType));
            line.Append(' ');
            line.Append(method.Name);

            if (method.IsGeneric)
            {
                line.Append('<');
                line.Append(string.Join(", ", method.GenericTypeParameters.Select(g => GenerateGenericTypeParameter(g))));
                line.Append('>');
            }

            line.Append('(');
            line.Append(string.Join(", ", method.Parameters.Select(p => GenerateParameter(p))));
            line.Append(')');

            // Generic constraints
            if (method.GenericConstraints.Count > 0)
            {
                foreach (var constraint in method.GenericConstraints)
                {
                    line.Append($" where {GenerateGenericConstraint(constraint)}");
                }
            }

            line.Append(';');
            sb.AppendLine(line.ToString());
            return sb.ToString();
        }

        private string GenerateInterfaceEvent(EventElement evt)
        {
            var sb = new StringBuilder();
            sb.Append(GenerateDocumentation(evt.Documentation));
            sb.AppendLine(Line($"event {GenerateTypeReference(evt.Type)} {evt.Name};").TrimEnd());
            return sb.ToString();
        }

        private string GenerateInterfaceIndexer(IndexerElement indexer)
        {
            var sb = new StringBuilder();
            sb.Append(GenerateDocumentation(indexer.Documentation));

            var line = new StringBuilder();
            line.Append(GetIndent());
            line.Append(GenerateTypeReference(indexer.Type));
            line.Append(" this[");
            line.Append(string.Join(", ", indexer.Parameters.Select(p => GenerateParameter(p))));
            line.Append("] { ");

            if (indexer.HasGetter) line.Append("get; ");
            if (indexer.HasSetter) line.Append("set; ");
            line.Append('}');

            sb.AppendLine(line.ToString());
            return sb.ToString();
        }

        #endregion

        #region Struct

        public override string GenerateStruct(StructElement strct)
        {
            if (strct.RawCode != null) return strct.RawCode;
            var sb = new StringBuilder();

            // Documentation
            sb.Append(GenerateDocumentation(strct.Documentation));

            // Attributes
            foreach (var attr in strct.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            // Struct declaration
            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append(GenerateAccessModifier(strct.AccessModifier));

            if (strct.IsReadonly)
                declaration.Append("readonly ");
            if (strct.IsRef)
                declaration.Append("ref ");

            if (strct.IsRecord)
                declaration.Append("record struct ");
            else
                declaration.Append("struct ");

            declaration.Append(strct.Name);

            // Generic type parameters
            if (strct.IsGeneric)
            {
                declaration.Append('<');
                declaration.Append(string.Join(", ", strct.GenericTypeParameters.Select(g => GenerateGenericTypeParameter(g))));
                declaration.Append('>');
            }

            // Primary constructor parameters
            if (strct.PrimaryConstructorParameters.Count > 0)
            {
                declaration.Append('(');
                declaration.Append(string.Join(", ", strct.PrimaryConstructorParameters.Select(p => GenerateParameter(p))));
                declaration.Append(')');
            }

            // Base types (interfaces)
            if (strct.BaseTypes.Count > 0)
            {
                declaration.Append(" : ");
                declaration.Append(string.Join(", ", strct.BaseTypes.Select(b => GenerateTypeReference(b))));
            }

            sb.AppendLine(declaration.ToString());

            // Struct body
            sb.AppendLine(Line("{").TrimEnd());
            IncreaseIndent();

            // Fields
            foreach (var field in strct.Fields)
            {
                sb.Append(GenerateField(field));
            }

            // Properties
            foreach (var prop in strct.Properties)
            {
                sb.Append(GenerateProperty(prop));
            }

            // Constructors
            foreach (var ctor in strct.Constructors)
            {
                sb.Append(GenerateConstructor(ctor, strct.Name));
                sb.AppendLine();
            }

            // Methods
            foreach (var method in strct.Methods)
            {
                sb.Append(GenerateMethod(method));
                sb.AppendLine();
            }

            DecreaseIndent();
            sb.AppendLine(Line("}").TrimEnd());

            return sb.ToString();
        }

        #endregion

        #region Enum

        public override string GenerateEnum(EnumElement enm)
        {
            if (enm.RawCode != null) return enm.RawCode;
            var sb = new StringBuilder();

            // Documentation
            sb.Append(GenerateDocumentation(enm.Documentation));

            // Attributes
            foreach (var attr in enm.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            if (enm.IsFlags)
            {
                sb.AppendLine(Line("[Flags]").TrimEnd());
            }

            // Enum declaration
            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append(GenerateAccessModifier(enm.AccessModifier));
            declaration.Append("enum ");
            declaration.Append(enm.Name);

            if (enm.UnderlyingType != null)
            {
                declaration.Append(" : ");
                declaration.Append(GenerateTypeReference(enm.UnderlyingType));
            }

            sb.AppendLine(declaration.ToString());
            sb.AppendLine(Line("{").TrimEnd());
            IncreaseIndent();

            // Enum members
            for (int i = 0; i < enm.Members.Count; i++)
            {
                var member = enm.Members[i];
                sb.Append(GenerateDocumentation(member.Documentation));

                var memberLine = new StringBuilder();
                memberLine.Append(GetIndent());
                memberLine.Append(member.Name);

                if (member.HasExplicitValue)
                {
                    memberLine.Append(" = ");
                    memberLine.Append(member.Value);
                }

                if (i < enm.Members.Count - 1)
                    memberLine.Append(',');

                sb.AppendLine(memberLine.ToString());
            }

            DecreaseIndent();
            sb.AppendLine(Line("}").TrimEnd());

            return sb.ToString();
        }

        #endregion

        #region Delegate

        public override string GenerateDelegate(DelegateElement del)
        {
            if (del.RawCode != null) return del.RawCode;
            var sb = new StringBuilder();

            // Documentation
            sb.Append(GenerateDocumentation(del.Documentation));

            // Attributes
            foreach (var attr in del.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append(GenerateAccessModifier(del.AccessModifier));
            declaration.Append("delegate ");
            declaration.Append(GenerateTypeReference(del.ReturnType));
            declaration.Append(' ');
            declaration.Append(del.Name);

            // Generic type parameters
            if (del.IsGeneric)
            {
                declaration.Append('<');
                declaration.Append(string.Join(", ", del.GenericTypeParameters.Select(g => GenerateGenericTypeParameter(g))));
                declaration.Append('>');
            }

            declaration.Append('(');
            declaration.Append(string.Join(", ", del.Parameters.Select(p => GenerateParameter(p))));
            declaration.Append(");");

            sb.AppendLine(declaration.ToString());

            return sb.ToString();
        }

        #endregion

        #region Field

        public override string GenerateField(FieldElement field)
        {
            if (field.RawCode != null) return field.RawCode;
            var sb = new StringBuilder();

            // Documentation
            sb.Append(GenerateDocumentation(field.Documentation));

            // Attributes
            foreach (var attr in field.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            var line = new StringBuilder();
            line.Append(GetIndent());
            line.Append(GenerateAccessModifier(field.AccessModifier));
            line.Append(GenerateModifiers(field.Modifiers));
            line.Append(GenerateTypeReference(field.Type));
            line.Append(' ');
            line.Append(field.Name);

            if (field.HasInitialValue)
            {
                line.Append(" = ");
                line.Append(field.InitialValue);
            }

            line.Append(';');
            sb.AppendLine(line.ToString());

            return sb.ToString();
        }

        #endregion

        #region Property

        public override string GenerateProperty(PropertyElement prop)
        {
            if (prop.RawCode != null) return prop.RawCode;
            var sb = new StringBuilder();

            // Documentation
            sb.Append(GenerateDocumentation(prop.Documentation));

            // Attributes
            foreach (var attr in prop.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            // Expression-bodied property
            if (prop.IsExpressionBodied && !string.IsNullOrEmpty(prop.ExpressionBody))
            {
                var line = new StringBuilder();
                line.Append(GetIndent());
                line.Append(GenerateAccessModifier(prop.AccessModifier));
                line.Append(GenerateModifiers(prop.Modifiers));
                line.Append(GenerateTypeReference(prop.Type));
                line.Append(' ');
                line.Append(prop.Name);
                line.Append(" => ");
                line.Append(prop.ExpressionBody);
                line.Append(';');
                sb.AppendLine(line.ToString());
                return sb.ToString();
            }

            // Auto-implemented or full property
            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append(GenerateAccessModifier(prop.AccessModifier));
            declaration.Append(GenerateModifiers(prop.Modifiers));
            declaration.Append(GenerateTypeReference(prop.Type));
            declaration.Append(' ');
            declaration.Append(prop.Name);

            if (prop.IsAutoImplemented)
            {
                // Auto-property on single line
                declaration.Append(" { ");

                if (prop.HasGetter)
                {
                    if (prop.GetterAccessModifier.HasValue && prop.GetterAccessModifier.Value != prop.AccessModifier)
                    {
                        declaration.Append(GenerateAccessModifier(prop.GetterAccessModifier.Value).TrimEnd());
                        declaration.Append(' ');
                    }
                    declaration.Append("get; ");
                }

                if (prop.HasSetter)
                {
                    if (prop.SetterAccessModifier.HasValue && prop.SetterAccessModifier.Value != prop.AccessModifier)
                    {
                        declaration.Append(GenerateAccessModifier(prop.SetterAccessModifier.Value).TrimEnd());
                        declaration.Append(' ');
                    }
                    if (prop.IsInitOnly)
                        declaration.Append("init; ");
                    else
                        declaration.Append("set; ");
                }

                declaration.Append('}');

                if (prop.HasInitialValue)
                {
                    declaration.Append(" = ");
                    declaration.Append(prop.InitialValue);
                    declaration.Append(';');
                }

                sb.AppendLine(declaration.ToString());
            }
            else
            {
                // Full property with body
                sb.AppendLine(declaration.ToString());
                sb.AppendLine(Line("{").TrimEnd());
                IncreaseIndent();

                if (prop.HasGetter)
                {
                    var getterLine = new StringBuilder();
                    getterLine.Append(GetIndent());
                    if (prop.GetterAccessModifier.HasValue && prop.GetterAccessModifier.Value != prop.AccessModifier)
                    {
                        getterLine.Append(GenerateAccessModifier(prop.GetterAccessModifier.Value).TrimEnd());
                        getterLine.Append(' ');
                    }
                    getterLine.Append("get");

                    if (prop.GetterBody.HasStatements)
                    {
                        sb.AppendLine(getterLine.ToString());
                        sb.AppendLine(Line("{").TrimEnd());
                        IncreaseIndent();
                        GenerateStatements(sb, prop.GetterBody.Statements);
                        DecreaseIndent();
                        sb.AppendLine(Line("}").TrimEnd());
                    }
                    else
                    {
                        getterLine.Append(';');
                        sb.AppendLine(getterLine.ToString());
                    }
                }

                if (prop.HasSetter)
                {
                    var setterLine = new StringBuilder();
                    setterLine.Append(GetIndent());
                    if (prop.SetterAccessModifier.HasValue && prop.SetterAccessModifier.Value != prop.AccessModifier)
                    {
                        setterLine.Append(GenerateAccessModifier(prop.SetterAccessModifier.Value).TrimEnd());
                        setterLine.Append(' ');
                    }
                    setterLine.Append(prop.IsInitOnly ? "init" : "set");

                    if (prop.SetterBody.HasStatements)
                    {
                        sb.AppendLine(setterLine.ToString());
                        sb.AppendLine(Line("{").TrimEnd());
                        IncreaseIndent();
                        GenerateStatements(sb, prop.SetterBody.Statements);
                        DecreaseIndent();
                        sb.AppendLine(Line("}").TrimEnd());
                    }
                    else
                    {
                        setterLine.Append(';');
                        sb.AppendLine(setterLine.ToString());
                    }
                }

                DecreaseIndent();
                sb.AppendLine(Line("}").TrimEnd());
            }

            return sb.ToString();
        }

        #endregion

        #region Method

        public override string GenerateMethod(MethodElement method)
        {
            if (method.RawCode != null) return method.RawCode;

            var sb = new StringBuilder();

            // Documentation
            sb.Append(GenerateDocumentation(method.Documentation));

            // Attributes
            foreach (var attr in method.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append(GenerateAccessModifier(method.AccessModifier));
            declaration.Append(GenerateModifiers(method.Modifiers));
            declaration.Append(GenerateTypeReference(method.ReturnType));
            declaration.Append(' ');
            declaration.Append(method.Name);

            // Generic type parameters
            if (method.IsGeneric)
            {
                declaration.Append('<');
                declaration.Append(string.Join(", ", method.GenericTypeParameters.Select(g => GenerateGenericTypeParameter(g))));
                declaration.Append('>');
            }

            declaration.Append('(');

            // Parameters (with 'this' for extension methods)
            var parameters = method.Parameters.Select((p, i) =>
            {
                if (i == 0 && method.IsExtensionMethod)
                {
                    return "this " + GenerateParameter(p);
                }
                return GenerateParameter(p);
            });
            declaration.Append(string.Join(", ", parameters));
            declaration.Append(')');

            // Expression-bodied method
            if (method.IsExpressionBodied && !string.IsNullOrEmpty(method.ExpressionBody))
            {
                // Generic constraints before body
                foreach (var constraint in method.GenericConstraints)
                {
                    sb.AppendLine(declaration.ToString());
                    declaration.Clear();
                    declaration.Append(GetIndent());
                    declaration.Append($"    where {GenerateGenericConstraint(constraint)}");
                }

                declaration.Append(" => ");
                declaration.Append(method.ExpressionBody);
                declaration.Append(';');
                sb.AppendLine(declaration.ToString());
                return sb.ToString();
            }

            // Abstract method (no body)
            if (method.Modifiers.HasFlag(ElementModifiers.Abstract) || !method.HasBody)
            {
                foreach (var constraint in method.GenericConstraints)
                {
                    sb.AppendLine(declaration.ToString());
                    declaration.Clear();
                    declaration.Append(GetIndent());
                    declaration.Append($"    where {GenerateGenericConstraint(constraint)}");
                }
                declaration.Append(';');
                sb.AppendLine(declaration.ToString());
                return sb.ToString();
            }

            // Generic constraints
            if (method.GenericConstraints.Count > 0)
            {
                sb.AppendLine(declaration.ToString());
                foreach (var constraint in method.GenericConstraints)
                {
                    sb.AppendLine(Line($"    where {GenerateGenericConstraint(constraint)}").TrimEnd());
                }
                sb.AppendLine(Line("{").TrimEnd());
            }
            else
            {
                sb.AppendLine(declaration.ToString());
                sb.AppendLine(Line("{").TrimEnd());
            }

            // Method body
            IncreaseIndent();
            GenerateStatements(sb, method.Body.Statements);
            DecreaseIndent();
            sb.AppendLine(Line("}").TrimEnd());

            return sb.ToString();
        }

        #endregion

        #region Constructor

        public override string GenerateConstructor(ConstructorElement ctor)
        {
            return GenerateConstructor(ctor, "ClassName");
        }

        public string GenerateConstructor(ConstructorElement ctor, string? className)
        {
            if (ctor.RawCode != null) return ctor.RawCode;

            var sb = new StringBuilder();

            // Documentation
            sb.Append(GenerateDocumentation(ctor.Documentation));

            // Attributes
            foreach (var attr in ctor.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            var declaration = new StringBuilder();
            declaration.Append(GetIndent());

            if (ctor.IsStatic)
            {
                declaration.Append("static ");
            }
            else
            {
                declaration.Append(GenerateAccessModifier(ctor.AccessModifier));
            }

            declaration.Append(className ?? ctor.Name ?? "Constructor");
            declaration.Append('(');
            declaration.Append(string.Join(", ", ctor.Parameters.Select(p => GenerateParameter(p))));
            declaration.Append(')');

            // Base or this call
            if (ctor.BaseCall != null)
            {
                declaration.Append(" : base(");
                declaration.Append(string.Join(", ", ctor.BaseCall.Arguments));
                declaration.Append(')');
            }
            else if (ctor.ThisCall != null)
            {
                declaration.Append(" : this(");
                declaration.Append(string.Join(", ", ctor.ThisCall.Arguments));
                declaration.Append(')');
            }

            sb.AppendLine(declaration.ToString());
            sb.AppendLine(Line("{").TrimEnd());

            // Constructor body
            IncreaseIndent();
            GenerateStatements(sb, ctor.Body.Statements);
            DecreaseIndent();
            sb.AppendLine(Line("}").TrimEnd());

            return sb.ToString();
        }

        private string GenerateFinalizer(FinalizerElement finalizer, string? className)
        {
            var sb = new StringBuilder();

            sb.Append(GenerateDocumentation(finalizer.Documentation));
            sb.AppendLine(Line($"~{className ?? "Class"}()").TrimEnd());
            sb.AppendLine(Line("{").TrimEnd());

            IncreaseIndent();
            if (!string.IsNullOrEmpty(finalizer.Body))
            {
                foreach (var bodyLine in finalizer.Body.Split('\n'))
                {
                    var trimmedLine = bodyLine.TrimEnd('\r');
                    if (!string.IsNullOrWhiteSpace(trimmedLine))
                        sb.AppendLine(Line(trimmedLine.TrimStart()).TrimEnd());
                    else
                        sb.AppendLine();
                }
            }
            DecreaseIndent();
            sb.AppendLine(Line("}").TrimEnd());

            return sb.ToString();
        }

        #endregion

        #region Event

        public override string GenerateEvent(EventElement evt)
        {
            if (evt.RawCode != null) return evt.RawCode;

            var sb = new StringBuilder();

            // Documentation
            sb.Append(GenerateDocumentation(evt.Documentation));

            // Attributes
            foreach (var attr in evt.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append(GenerateAccessModifier(evt.AccessModifier));
            declaration.Append(GenerateModifiers(evt.Modifiers));
            declaration.Append("event ");
            declaration.Append(GenerateTypeReference(evt.Type));
            declaration.Append(' ');
            declaration.Append(evt.Name);

            if (evt.IsFieldLike)
            {
                declaration.Append(';');
                sb.AppendLine(declaration.ToString());
            }
            else
            {
                sb.AppendLine(declaration.ToString());
                sb.AppendLine(Line("{").TrimEnd());
                IncreaseIndent();

                if (!string.IsNullOrEmpty(evt.AddAccessorBody))
                {
                    sb.AppendLine(Line("add").TrimEnd());
                    sb.AppendLine(Line("{").TrimEnd());
                    IncreaseIndent();
                    foreach (var bodyLine in evt.AddAccessorBody.Split('\n'))
                    {
                        sb.AppendLine(Line(bodyLine.Trim()).TrimEnd());
                    }
                    DecreaseIndent();
                    sb.AppendLine(Line("}").TrimEnd());
                }

                if (!string.IsNullOrEmpty(evt.RemoveAccessorBody))
                {
                    sb.AppendLine(Line("remove").TrimEnd());
                    sb.AppendLine(Line("{").TrimEnd());
                    IncreaseIndent();
                    foreach (var bodyLine in evt.RemoveAccessorBody.Split('\n'))
                    {
                        sb.AppendLine(Line(bodyLine.Trim()).TrimEnd());
                    }
                    DecreaseIndent();
                    sb.AppendLine(Line("}").TrimEnd());
                }

                DecreaseIndent();
                sb.AppendLine(Line("}").TrimEnd());
            }

            return sb.ToString();
        }

        #endregion

        #region Indexer

        public override string GenerateIndexer(IndexerElement indexer)
        {
            if (indexer.RawCode != null) return indexer.RawCode;

            var sb = new StringBuilder();

            // Documentation
            sb.Append(GenerateDocumentation(indexer.Documentation));

            // Attributes
            foreach (var attr in indexer.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append(GenerateAccessModifier(indexer.AccessModifier));
            declaration.Append(GenerateModifiers(indexer.Modifiers));
            declaration.Append(GenerateTypeReference(indexer.Type));
            declaration.Append(" this[");
            declaration.Append(string.Join(", ", indexer.Parameters.Select(p => GenerateParameter(p))));
            declaration.Append(']');

            sb.AppendLine(declaration.ToString());
            sb.AppendLine(Line("{").TrimEnd());
            IncreaseIndent();

            if (indexer.HasGetter)
            {
                var getterLine = new StringBuilder();
                getterLine.Append(GetIndent());
                if (indexer.GetterAccessModifier.HasValue && indexer.GetterAccessModifier.Value != indexer.AccessModifier)
                {
                    getterLine.Append(GenerateAccessModifier(indexer.GetterAccessModifier.Value).TrimEnd());
                    getterLine.Append(' ');
                }
                getterLine.Append("get");

                if (indexer.GetterBody.HasStatements)
                {
                    sb.AppendLine(getterLine.ToString());
                    sb.AppendLine(Line("{").TrimEnd());
                    IncreaseIndent();
                    GenerateStatements(sb, indexer.GetterBody.Statements);
                    DecreaseIndent();
                    sb.AppendLine(Line("}").TrimEnd());
                }
                else
                {
                    getterLine.Append(';');
                    sb.AppendLine(getterLine.ToString());
                }
            }

            if (indexer.HasSetter)
            {
                var setterLine = new StringBuilder();
                setterLine.Append(GetIndent());
                if (indexer.SetterAccessModifier.HasValue && indexer.SetterAccessModifier.Value != indexer.AccessModifier)
                {
                    setterLine.Append(GenerateAccessModifier(indexer.SetterAccessModifier.Value).TrimEnd());
                    setterLine.Append(' ');
                }
                setterLine.Append("set");

                if (indexer.SetterBody.HasStatements)
                {
                    sb.AppendLine(setterLine.ToString());
                    sb.AppendLine(Line("{").TrimEnd());
                    IncreaseIndent();
                    GenerateStatements(sb, indexer.SetterBody.Statements);
                    DecreaseIndent();
                    sb.AppendLine(Line("}").TrimEnd());
                }
                else
                {
                    setterLine.Append(';');
                    sb.AppendLine(setterLine.ToString());
                }
            }

            DecreaseIndent();
            sb.AppendLine(Line("}").TrimEnd());

            return sb.ToString();
        }

        #endregion

        #region Operator

        public override string GenerateOperator(OperatorElement op)
        {
            if (op.RawCode != null) return op.RawCode;

            var sb = new StringBuilder();

            // Documentation
            sb.Append(GenerateDocumentation(op.Documentation));

            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append("public static ");

            if (op.OperatorType == OperatorType.Implicit)
            {
                declaration.Append("implicit operator ");
                declaration.Append(GenerateTypeReference(op.ReturnType));
            }
            else if (op.OperatorType == OperatorType.Explicit)
            {
                declaration.Append("explicit operator ");
                declaration.Append(GenerateTypeReference(op.ReturnType));
            }
            else
            {
                declaration.Append(GenerateTypeReference(op.ReturnType));
                declaration.Append(" operator ");
                declaration.Append(GetOperatorSymbol(op.OperatorType));
            }

            declaration.Append('(');
            declaration.Append(string.Join(", ", op.Parameters.Select(p => GenerateParameter(p))));
            declaration.Append(')');

            sb.AppendLine(declaration.ToString());
            sb.AppendLine(Line("{").TrimEnd());

            IncreaseIndent();
            GenerateStatements(sb, op.Body.Statements);
            DecreaseIndent();
            sb.AppendLine(Line("}").TrimEnd());

            return sb.ToString();
        }

        private string GetOperatorSymbol(OperatorType operatorType)
        {
            return operatorType switch
            {
                OperatorType.UnaryPlus => "+",
                OperatorType.UnaryMinus => "-",
                OperatorType.LogicalNot => "!",
                OperatorType.BitwiseNot => "~",
                OperatorType.Increment => "++",
                OperatorType.Decrement => "--",
                OperatorType.True => "true",
                OperatorType.False => "false",
                OperatorType.Addition => "+",
                OperatorType.Subtraction => "-",
                OperatorType.Multiplication => "*",
                OperatorType.Division => "/",
                OperatorType.Modulus => "%",
                OperatorType.BitwiseAnd => "&",
                OperatorType.BitwiseOr => "|",
                OperatorType.BitwiseXor => "^",
                OperatorType.LeftShift => "<<",
                OperatorType.RightShift => ">>",
                OperatorType.UnsignedRightShift => ">>>",
                OperatorType.Equality => "==",
                OperatorType.Inequality => "!=",
                OperatorType.LessThan => "<",
                OperatorType.GreaterThan => ">",
                OperatorType.LessThanOrEqual => "<=",
                OperatorType.GreaterThanOrEqual => ">=",
                _ => throw new NotSupportedException($"Operator type '{operatorType}' is not supported.")
            };
        }

        #endregion

        #region Attribute

        public override string GenerateAttribute(AttributeElement attr)
        {
            if (attr.RawCode != null) return attr.RawCode;

            var sb = new StringBuilder();
            sb.Append('[');

            // Attribute target
            if (attr.Target != AttributeTarget.Default)
            {
                sb.Append(attr.Target.ToString().ToLower());
                sb.Append(": ");
            }

            sb.Append(attr.AttributeName);

            // Arguments
            var allArgs = new List<string>();
            allArgs.AddRange(attr.Arguments);
            allArgs.AddRange(attr.NamedArguments.Select(kv => $"{kv.Key} = {kv.Value}"));

            if (allArgs.Count > 0)
            {
                sb.Append('(');
                sb.Append(string.Join(", ", allArgs));
                sb.Append(')');
            }

            sb.Append(']');
            return sb.ToString();
        }

        #endregion

        #region Parameter

        public override string GenerateParameter(ParameterElement param)
        {
            if (param.RawCode != null) return param.RawCode;

            var sb = new StringBuilder();

            // Attributes
            foreach (var attr in param.Attributes)
            {
                sb.Append(GenerateAttribute(attr));
                sb.Append(' ');
            }

            // Modifier
            switch (param.Modifier)
            {
                case ParameterModifier.Ref:
                    sb.Append("ref ");
                    break;
                case ParameterModifier.Out:
                    sb.Append("out ");
                    break;
                case ParameterModifier.In:
                    sb.Append("in ");
                    break;
                case ParameterModifier.Params:
                    sb.Append("params ");
                    break;
            }

            sb.Append(GenerateTypeReference(param.Type));
            sb.Append(' ');
            sb.Append(param.Name);

            if (param.HasDefaultValue)
            {
                sb.Append(" = ");
                sb.Append(param.DefaultValue);
            }

            return sb.ToString();
        }

        #endregion

        #region Type Reference

        public override string GenerateTypeReference(TypeReference typeRef)
        {
            var sb = new StringBuilder();

            sb.Append(typeRef.TypeName);

            // Generic arguments
            if (typeRef.GenericArguments.Count > 0)
            {
                sb.Append('<');
                sb.Append(string.Join(", ", typeRef.GenericArguments.Select(g => GenerateTypeReference(g))));
                sb.Append('>');
            }

            // Nullable
            if (typeRef.IsNullable)
            {
                sb.Append('?');
            }

            // Array
            if (typeRef.IsArray)
            {
                sb.Append('[');
                if (typeRef.ArrayRank > 1)
                {
                    sb.Append(new string(',', typeRef.ArrayRank - 1));
                }
                sb.Append(']');
            }

            return sb.ToString();
        }

        #endregion

        #region Statement Generation

        /// <summary>
        /// Generates a list of statements into the StringBuilder
        /// </summary>
        private void GenerateStatements(StringBuilder sb, List<StatementElement> statements)
        {
            foreach (var statement in statements)
            {
                GenerateStatement(sb, statement);
            }
        }

        /// <summary>
        /// Generates a single statement into the StringBuilder
        /// </summary>
        private void GenerateStatement(StringBuilder sb, StatementElement statement)
        {
            if (statement.RawCode != null)
            {
                foreach (var rawLine in statement.RawCode.Split('\n'))
                {
                    var trimmed = rawLine.TrimEnd('\r');
                    if (!string.IsNullOrWhiteSpace(trimmed))
                        sb.AppendLine(Line(trimmed.TrimStart()).TrimEnd());
                    else
                        sb.AppendLine();
                }
                return;
            }

            switch (statement)
            {
                case CompositeStatement composite:
                    GenerateStatements(sb, composite.Statements);
                    break;

                case RawStatementElement raw:
                    foreach (var rawLine in raw.Code.Split('\n'))
                    {
                        var trimmed = rawLine.TrimEnd('\r');
                        if (!string.IsNullOrWhiteSpace(trimmed))
                            sb.AppendLine(Line(trimmed.TrimStart()).TrimEnd());
                        else
                            sb.AppendLine();
                    }
                    break;

                case CommentStatement comment:
                    sb.AppendLine(Line($"// {comment.Text}").TrimEnd());
                    break;

                case AssignmentStatement assignment:
                    sb.AppendLine(Line($"{assignment.Left} = {assignment.Right};").TrimEnd());
                    break;

                case ReturnStatementElement returnStmt:
                    if (string.IsNullOrEmpty(returnStmt.Expression))
                        sb.AppendLine(Line("return;").TrimEnd());
                    else
                        sb.AppendLine(Line($"return {returnStmt.Expression};").TrimEnd());
                    break;

                case ThrowStatementElement throwStmt:
                    if (string.IsNullOrEmpty(throwStmt.Expression))
                        sb.AppendLine(Line("throw;").TrimEnd());
                    else
                        sb.AppendLine(Line($"throw {throwStmt.Expression};").TrimEnd());
                    break;

                case IfStatementElement ifStmt:
                    sb.AppendLine(Line($"if ({ifStmt.Condition})").TrimEnd());
                    sb.AppendLine(Line("{").TrimEnd());
                    IncreaseIndent();
                    GenerateStatements(sb, ifStmt.ThenStatements);
                    DecreaseIndent();
                    sb.AppendLine(Line("}").TrimEnd());

                    foreach (var elseIf in ifStmt.ElseIfBranches)
                    {
                        sb.AppendLine(Line($"else if ({elseIf.Condition})").TrimEnd());
                        sb.AppendLine(Line("{").TrimEnd());
                        IncreaseIndent();
                        GenerateStatements(sb, elseIf.Statements);
                        DecreaseIndent();
                        sb.AppendLine(Line("}").TrimEnd());
                    }

                    if (ifStmt.ElseStatements.Count > 0)
                    {
                        sb.AppendLine(Line("else").TrimEnd());
                        sb.AppendLine(Line("{").TrimEnd());
                        IncreaseIndent();
                        GenerateStatements(sb, ifStmt.ElseStatements);
                        DecreaseIndent();
                        sb.AppendLine(Line("}").TrimEnd());
                    }
                    break;

                case ForStatementElement forStmt:
                    sb.AppendLine(Line($"for ({forStmt.Initializer}; {forStmt.Condition}; {forStmt.Incrementer})").TrimEnd());
                    sb.AppendLine(Line("{").TrimEnd());
                    IncreaseIndent();
                    GenerateStatements(sb, forStmt.Body);
                    DecreaseIndent();
                    sb.AppendLine(Line("}").TrimEnd());
                    break;

                case ForEachStatementElement forEachStmt:
                    var varType = forEachStmt.VariableType != null
                        ? GenerateTypeReference(forEachStmt.VariableType)
                        : "var";
                    sb.AppendLine(Line($"foreach ({varType} {forEachStmt.VariableName} in {forEachStmt.Collection})").TrimEnd());
                    sb.AppendLine(Line("{").TrimEnd());
                    IncreaseIndent();
                    GenerateStatements(sb, forEachStmt.Body);
                    DecreaseIndent();
                    sb.AppendLine(Line("}").TrimEnd());
                    break;

                case WhileStatementElement whileStmt:
                    sb.AppendLine(Line($"while ({whileStmt.Condition})").TrimEnd());
                    sb.AppendLine(Line("{").TrimEnd());
                    IncreaseIndent();
                    GenerateStatements(sb, whileStmt.Body);
                    DecreaseIndent();
                    sb.AppendLine(Line("}").TrimEnd());
                    break;

                case SwitchStatementElement switchStmt:
                    sb.AppendLine(Line($"switch ({switchStmt.Expression})").TrimEnd());
                    sb.AppendLine(Line("{").TrimEnd());
                    IncreaseIndent();
                    foreach (var caseBlock in switchStmt.Cases)
                    {
                        foreach (var label in caseBlock.Labels)
                        {
                            sb.AppendLine(Line($"case {label}:").TrimEnd());
                        }
                        if (!string.IsNullOrEmpty(caseBlock.Pattern))
                        {
                            var whenClause = !string.IsNullOrEmpty(caseBlock.WhenClause) ? $" when {caseBlock.WhenClause}" : "";
                            sb.AppendLine(Line($"case {caseBlock.Pattern}{whenClause}:").TrimEnd());
                        }
                        IncreaseIndent();
                        GenerateStatements(sb, caseBlock.Statements);
                        DecreaseIndent();
                    }
                    if (switchStmt.DefaultStatements.Count > 0)
                    {
                        sb.AppendLine(Line("default:").TrimEnd());
                        IncreaseIndent();
                        GenerateStatements(sb, switchStmt.DefaultStatements);
                        DecreaseIndent();
                    }
                    DecreaseIndent();
                    sb.AppendLine(Line("}").TrimEnd());
                    break;

                case TryCatchStatementElement tryCatch:
                    sb.AppendLine(Line("try").TrimEnd());
                    sb.AppendLine(Line("{").TrimEnd());
                    IncreaseIndent();
                    GenerateStatements(sb, tryCatch.TryStatements);
                    DecreaseIndent();
                    sb.AppendLine(Line("}").TrimEnd());

                    foreach (var catchBlock in tryCatch.CatchBlocks)
                    {
                        var catchLine = new StringBuilder("catch");
                        if (catchBlock.ExceptionType != null)
                        {
                            catchLine.Append($" ({GenerateTypeReference(catchBlock.ExceptionType)}");
                            if (!string.IsNullOrEmpty(catchBlock.ExceptionVariable))
                                catchLine.Append($" {catchBlock.ExceptionVariable}");
                            catchLine.Append(')');
                        }
                        if (!string.IsNullOrEmpty(catchBlock.WhenFilter))
                        {
                            catchLine.Append($" when ({catchBlock.WhenFilter})");
                        }
                        sb.AppendLine(Line(catchLine.ToString()).TrimEnd());
                        sb.AppendLine(Line("{").TrimEnd());
                        IncreaseIndent();
                        GenerateStatements(sb, catchBlock.Statements);
                        DecreaseIndent();
                        sb.AppendLine(Line("}").TrimEnd());
                    }

                    if (tryCatch.HasFinally)
                    {
                        sb.AppendLine(Line("finally").TrimEnd());
                        sb.AppendLine(Line("{").TrimEnd());
                        IncreaseIndent();
                        GenerateStatements(sb, tryCatch.FinallyStatements);
                        DecreaseIndent();
                        sb.AppendLine(Line("}").TrimEnd());
                    }
                    break;

                case UsingStatementElement usingStmt:
                    if (usingStmt.IsDeclaration)
                    {
                        sb.AppendLine(Line($"using {usingStmt.Resource};").TrimEnd());
                        GenerateStatements(sb, usingStmt.Body);
                    }
                    else
                    {
                        sb.AppendLine(Line($"using ({usingStmt.Resource})").TrimEnd());
                        sb.AppendLine(Line("{").TrimEnd());
                        IncreaseIndent();
                        GenerateStatements(sb, usingStmt.Body);
                        DecreaseIndent();
                        sb.AppendLine(Line("}").TrimEnd());
                    }
                    break;

                default:
                    // Unknown statement type — skip
                    break;
            }
        }

        #endregion

        #region Helper Methods

        public override string GenerateAccessModifier(AccessModifier modifier)
        {
            return modifier switch
            {
                AccessModifier.Public => "public ",
                AccessModifier.Private => "private ",
                AccessModifier.Protected => "protected ",
                AccessModifier.Internal => "internal ",
                AccessModifier.ProtectedInternal => "protected internal ",
                AccessModifier.PrivateProtected => "private protected ",
                AccessModifier.Default => "",
                _ => ""
            };
        }

        public override string GenerateModifiers(ElementModifiers modifiers)
        {
            var sb = new StringBuilder();

            if (modifiers.HasFlag(ElementModifiers.New)) sb.Append("new ");
            if (modifiers.HasFlag(ElementModifiers.Static)) sb.Append("static ");
            if (modifiers.HasFlag(ElementModifiers.Abstract)) sb.Append("abstract ");
            if (modifiers.HasFlag(ElementModifiers.Virtual)) sb.Append("virtual ");
            if (modifiers.HasFlag(ElementModifiers.Override)) sb.Append("override ");
            if (modifiers.HasFlag(ElementModifiers.Sealed)) sb.Append("sealed ");
            if (modifiers.HasFlag(ElementModifiers.Readonly)) sb.Append("readonly ");
            if (modifiers.HasFlag(ElementModifiers.Const)) sb.Append("const ");
            if (modifiers.HasFlag(ElementModifiers.Async)) sb.Append("async ");
            if (modifiers.HasFlag(ElementModifiers.Partial)) sb.Append("partial ");
            if (modifiers.HasFlag(ElementModifiers.Extern)) sb.Append("extern ");
            if (modifiers.HasFlag(ElementModifiers.Volatile)) sb.Append("volatile ");
            if (modifiers.HasFlag(ElementModifiers.Required)) sb.Append("required ");

            return sb.ToString();
        }

        public override string GenerateDocumentation(string? documentation)
        {
            if (string.IsNullOrEmpty(documentation))
                return string.Empty;

            // escape xml-special characters
            documentation = System.Security.SecurityElement.Escape(documentation);

            var sb = new StringBuilder();
            var lines = documentation.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            if (lines.Length == 1)
            {
                sb.AppendLine(Line($"/// <summary>{documentation}</summary>").TrimEnd());
            }
            else
            {
                sb.AppendLine(Line("/// <summary>").TrimEnd());
                foreach (var line in lines)
                {
                    sb.AppendLine(Line($"/// {line}").TrimEnd());
                }
                sb.AppendLine(Line("/// </summary>").TrimEnd());
            }

            return sb.ToString();
        }

        private string GenerateGenericTypeParameter(GenericTypeParameterElement param)
        {
            var sb = new StringBuilder();

            switch (param.Variance)
            {
                case GenericVariance.Covariant:
                    sb.Append("out ");
                    break;
                case GenericVariance.Contravariant:
                    sb.Append("in ");
                    break;
            }

            sb.Append(param.Name);
            return sb.ToString();
        }

        private string GenerateGenericConstraint(GenericConstraintElement constraint)
        {
            var parts = new List<string>();

            // Special constraints first
            if (constraint.ConstraintKind.HasFlag(GenericConstraintKind.Class))
                parts.Add("class");
            if (constraint.ConstraintKind.HasFlag(GenericConstraintKind.Struct))
                parts.Add("struct");
            if (constraint.ConstraintKind.HasFlag(GenericConstraintKind.Unmanaged))
                parts.Add("unmanaged");
            if (constraint.ConstraintKind.HasFlag(GenericConstraintKind.NotNull))
                parts.Add("notnull");
            if (constraint.ConstraintKind.HasFlag(GenericConstraintKind.Default))
                parts.Add("default");

            // Type constraints
            foreach (var typeConstraint in constraint.ConstraintTypes)
            {
                parts.Add(GenerateTypeReference(typeConstraint));
            }

            // new() constraint last
            if (constraint.ConstraintKind.HasFlag(GenericConstraintKind.New))
                parts.Add("new()");

            return $"{constraint.TypeParameterName} : {string.Join(", ", parts)}";
        }

        #endregion
    }
}
