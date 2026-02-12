using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.CodeElements.Statements;
using CodeGenerator.Domain.ProgrammingLanguages;
using CodeGenerator.Domain.ProgrammingLanguages.Python;
using System.Text;

namespace CodeGenerator.Domain.ProgrammingLanguages.Python
{
    /// <summary>
    /// Code generator for Python programming language.
    /// Converts CodeElement structures to Python source code.
    /// </summary>
    public class PythonCodeGenerator : ProgrammingLanguageCodeGenerator
    {
        public override ProgrammingLanguage Language => PythonLanguage.Instance;

        private static readonly Dictionary<string, string> CSharpToPythonTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["void"] = "None",
            ["string"] = "str",
            ["String"] = "str",
            ["int"] = "int",
            ["Int32"] = "int",
            ["long"] = "int",
            ["Int64"] = "int",
            ["short"] = "int",
            ["Int16"] = "int",
            ["byte"] = "int",
            ["float"] = "float",
            ["Single"] = "float",
            ["double"] = "float",
            ["Double"] = "float",
            ["decimal"] = "float",
            ["Decimal"] = "float",
            ["bool"] = "bool",
            ["Boolean"] = "bool",
            ["object"] = "Any",
            ["Object"] = "Any",
            ["char"] = "str",
            ["DateTime"] = "datetime",
            ["DateOnly"] = "date",
            ["TimeOnly"] = "time",
            ["DateTimeOffset"] = "datetime",
            ["Guid"] = "UUID",
            ["Task"] = "Awaitable[None]",
            ["List"] = "list",
            ["Dictionary"] = "dict",
            ["IEnumerable"] = "Iterable",
            ["IList"] = "list",
            ["ICollection"] = "list",
            ["IDictionary"] = "dict",
            ["IReadOnlyList"] = "Sequence",
            ["IReadOnlyCollection"] = "Sequence",
            ["IReadOnlyDictionary"] = "Mapping",
        };

        #region Code File

        public override string GenerateCodeFile(CodeFileElement file)
        {
            if (file.RawCode != null) return file.RawCode;

            var sb = new StringBuilder();

            // File header
            if (!string.IsNullOrEmpty(file.FileHeader))
            {
                sb.Append(GenerateFileHeader(file.FileHeader));
            }

            // Imports (usings)
            foreach (var use in file.Usings)
            {
                sb.AppendLine(GenerateUsing(use).TrimEnd());
            }
            if (file.Usings.Count > 0)
                sb.AppendLine();

            // Global usings
            foreach (var globalUsing in file.GlobalUsings)
            {
                sb.AppendLine(GenerateUsing(globalUsing).TrimEnd());
            }
            if (file.GlobalUsings.Count > 0)
                sb.AppendLine();

            // Namespaces — Python doesn't have namespaces, generate the types directly
            foreach (var ns in file.Namespaces)
            {
                sb.Append(GenerateNamespace(ns));
            }

            // Top-level types
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
            var lines = header.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
            foreach (var line in lines)
            {
                sb.AppendLine($"# {line}");
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

            // Python has no namespace concept — just output the types
            foreach (var type in ns.Types)
            {
                sb.Append(GenerateCodeElement(type));
                sb.AppendLine();
            }

            // Nested namespaces
            foreach (var nestedNs in ns.NestedNamespaces)
            {
                sb.Append(GenerateNamespace(nestedNs));
            }

            return sb.ToString();
        }

        #endregion

        #region Using / Import

        public override string GenerateUsing(UsingElement use)
        {
            if (use.RawCode != null) return use.RawCode;

            if (!string.IsNullOrEmpty(use.Alias))
                return $"import {use.Namespace} as {use.Alias}";

            if (use.IsStatic)
            {
                // "using static X.Y.Z" ? "from X.Y import Z"
                var lastDot = use.Namespace.LastIndexOf('.');
                if (lastDot > 0)
                    return $"from {use.Namespace[..lastDot]} import {use.Namespace[(lastDot + 1)..]}";
            }

            return $"import {use.Namespace}";
        }

        #endregion

        #region Class

        public override string GenerateClass(ClassElement cls)
        {
            if (cls.RawCode != null) return cls.RawCode;

            var sb = new StringBuilder();

            // Documentation (before decorators in Python)
            // Decorators (attributes)
            foreach (var attr in cls.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            // Class declaration
            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append("class ");
            declaration.Append(cls.Name);

            // Base types
            var bases = new List<string>();
            if (cls.BaseTypes.Count > 0)
            {
                bases.AddRange(cls.BaseTypes.Select(b => GenerateTypeReference(b)));
            }

            // Generic type parameters ? use Generic[] base
            if (cls.IsGeneric)
            {
                var genericParams = string.Join(", ", cls.GenericTypeParameters.Select(g => g.Name));
                bases.Insert(0, $"Generic[{genericParams}]");
            }

            if (bases.Count > 0)
            {
                declaration.Append('(');
                declaration.Append(string.Join(", ", bases));
                declaration.Append(')');
            }

            declaration.Append(':');
            sb.AppendLine(declaration.ToString());
            IncreaseIndent();

            // Docstring
            sb.Append(GenerateDocumentation(cls.Documentation));

            var hasMembers = false;

            // Class-level fields (class variables)
            foreach (var field in cls.Fields)
            {
                if (field.Modifiers.HasFlag(ElementModifiers.Static) || field.Modifiers.HasFlag(ElementModifiers.Const))
                {
                    sb.Append(GenerateField(field));
                    hasMembers = true;
                }
            }
            if (cls.Fields.Any(f => f.Modifiers.HasFlag(ElementModifiers.Static) || f.Modifiers.HasFlag(ElementModifiers.Const)))
                sb.AppendLine();

            // Constructor (__init__)
            if (cls.Constructors.Count > 0)
            {
                foreach (var ctor in cls.Constructors)
                {
                    sb.Append(GenerateConstructor(ctor));
                    sb.AppendLine();
                }
                hasMembers = true;
            }
            else if (cls.Fields.Any(f => !f.Modifiers.HasFlag(ElementModifiers.Static) && !f.Modifiers.HasFlag(ElementModifiers.Const))
                     || cls.Properties.Count > 0)
            {
                // Auto-generate __init__ from instance fields and properties
                sb.Append(GenerateAutoInit(cls));
                sb.AppendLine();
                hasMembers = true;
            }

            // Properties
            foreach (var prop in cls.Properties)
            {
                sb.Append(GenerateProperty(prop));
                sb.AppendLine();
                hasMembers = true;
            }

            // Methods
            foreach (var method in cls.Methods)
            {
                sb.Append(GenerateMethod(method));
                sb.AppendLine();
                hasMembers = true;
            }

            // Events ? generate as simple attributes
            foreach (var evt in cls.Events)
            {
                sb.Append(GenerateEvent(evt));
                hasMembers = true;
            }

            // Operators
            foreach (var op in cls.Operators)
            {
                sb.Append(GenerateOperator(op));
                sb.AppendLine();
                hasMembers = true;
            }

            // Nested types
            foreach (var nestedType in cls.NestedTypes)
            {
                sb.Append(GenerateCodeElement(nestedType));
                sb.AppendLine();
                hasMembers = true;
            }

            if (!hasMembers)
            {
                sb.AppendLine(Line("pass").TrimEnd());
            }

            DecreaseIndent();
            return sb.ToString();
        }

        private string GenerateAutoInit(ClassElement cls)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Line("def __init__(self) -> None:").TrimEnd());
            IncreaseIndent();

            var instanceFields = cls.Fields
                .Where(f => !f.Modifiers.HasFlag(ElementModifiers.Static) && !f.Modifiers.HasFlag(ElementModifiers.Const))
                .ToList();

            foreach (var field in instanceFields)
            {
                var typeHint = GenerateTypeReference(field.Type);
                var initValue = field.HasInitialValue ? field.InitialValue : GetDefaultValue(typeHint);
                sb.AppendLine(Line($"self.{field.Name}: {typeHint} = {initValue}").TrimEnd());
            }

            foreach (var prop in cls.Properties)
            {
                if (prop.IsAutoImplemented)
                {
                    var typeHint = GenerateTypeReference(prop.Type);
                    var initValue = prop.HasInitialValue ? prop.InitialValue : GetDefaultValue(typeHint);
                    sb.AppendLine(Line($"self._{ToSnakeCase(prop.Name)}: {typeHint} = {initValue}").TrimEnd());
                }
            }

            if (instanceFields.Count == 0 && !cls.Properties.Any(p => p.IsAutoImplemented))
            {
                sb.AppendLine(Line("pass").TrimEnd());
            }

            DecreaseIndent();
            return sb.ToString();
        }

        #endregion

        #region Interface

        public override string GenerateInterface(InterfaceElement iface)
        {
            if (iface.RawCode != null) return iface.RawCode;

            var sb = new StringBuilder();

            // Decorators
            foreach (var attr in iface.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            // Python uses ABC for interfaces
            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append("class ");
            declaration.Append(iface.Name);

            var bases = new List<string> { "ABC" };
            if (iface.BaseTypes.Count > 0)
                bases.AddRange(iface.BaseTypes.Select(b => GenerateTypeReference(b)));
            if (iface.IsGeneric)
            {
                var genericParams = string.Join(", ", iface.GenericTypeParameters.Select(g => g.Name));
                bases.Insert(0, $"Generic[{genericParams}]");
            }

            declaration.Append('(');
            declaration.Append(string.Join(", ", bases));
            declaration.Append("):");
            sb.AppendLine(declaration.ToString());

            IncreaseIndent();

            // Docstring
            sb.Append(GenerateDocumentation(iface.Documentation));

            var hasMembers = false;

            // Abstract properties
            foreach (var prop in iface.Properties)
            {
                sb.AppendLine(Line("@property").TrimEnd());
                sb.AppendLine(Line("@abstractmethod").TrimEnd());
                var typeHint = GenerateTypeReference(prop.Type);
                sb.AppendLine(Line($"def {ToSnakeCase(prop.Name)}(self) -> {typeHint}:").TrimEnd());
                IncreaseIndent();
                sb.Append(GenerateDocumentation(prop.Documentation));
                sb.AppendLine(Line("...").TrimEnd());
                DecreaseIndent();
                sb.AppendLine();

                if (prop.HasSetter)
                {
                    sb.AppendLine(Line($"@{ToSnakeCase(prop.Name)}.setter").TrimEnd());
                    sb.AppendLine(Line("@abstractmethod").TrimEnd());
                    sb.AppendLine(Line($"def {ToSnakeCase(prop.Name)}(self, value: {typeHint}) -> None:").TrimEnd());
                    IncreaseIndent();
                    sb.AppendLine(Line("...").TrimEnd());
                    DecreaseIndent();
                    sb.AppendLine();
                }

                hasMembers = true;
            }

            // Abstract methods
            foreach (var method in iface.Methods)
            {
                sb.AppendLine(Line("@abstractmethod").TrimEnd());
                sb.Append(GenerateMethodSignature(method, isAbstract: true));
                sb.AppendLine();
                hasMembers = true;
            }

            if (!hasMembers)
            {
                sb.AppendLine(Line("pass").TrimEnd());
            }

            DecreaseIndent();
            return sb.ToString();
        }

        #endregion

        #region Struct

        public override string GenerateStruct(StructElement strct)
        {
            if (strct.RawCode != null) return strct.RawCode;

            // Python has no struct — generate as a dataclass
            var sb = new StringBuilder();

            foreach (var attr in strct.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            sb.AppendLine(Line("@dataclass").TrimEnd());

            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append("class ");
            declaration.Append(strct.Name);

            var bases = new List<string>();
            if (strct.BaseTypes.Count > 0)
                bases.AddRange(strct.BaseTypes.Select(b => GenerateTypeReference(b)));
            if (bases.Count > 0)
            {
                declaration.Append('(');
                declaration.Append(string.Join(", ", bases));
                declaration.Append(')');
            }

            declaration.Append(':');
            sb.AppendLine(declaration.ToString());
            IncreaseIndent();

            sb.Append(GenerateDocumentation(strct.Documentation));

            var hasMembers = false;

            // Fields as dataclass fields
            foreach (var field in strct.Fields)
            {
                var typeHint = GenerateTypeReference(field.Type);
                if (field.HasInitialValue)
                    sb.AppendLine(Line($"{field.Name}: {typeHint} = {field.InitialValue}").TrimEnd());
                else
                    sb.AppendLine(Line($"{field.Name}: {typeHint}").TrimEnd());
                hasMembers = true;
            }

            // Properties as dataclass fields
            foreach (var prop in strct.Properties)
            {
                var typeHint = GenerateTypeReference(prop.Type);
                if (prop.HasInitialValue)
                    sb.AppendLine(Line($"{ToSnakeCase(prop.Name)}: {typeHint} = {prop.InitialValue}").TrimEnd());
                else
                    sb.AppendLine(Line($"{ToSnakeCase(prop.Name)}: {typeHint}").TrimEnd());
                hasMembers = true;
            }

            // Methods
            foreach (var method in strct.Methods)
            {
                sb.AppendLine();
                sb.Append(GenerateMethod(method));
                hasMembers = true;
            }

            if (!hasMembers)
            {
                sb.AppendLine(Line("pass").TrimEnd());
            }

            DecreaseIndent();
            return sb.ToString();
        }

        #endregion

        #region Enum

        public override string GenerateEnum(EnumElement enm)
        {
            if (enm.RawCode != null) return enm.RawCode;

            var sb = new StringBuilder();

            foreach (var attr in enm.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append("class ");
            declaration.Append(enm.Name);

            if (enm.IsFlags)
                declaration.Append("(Flag):");
            else
                declaration.Append("(Enum):");

            sb.AppendLine(declaration.ToString());
            IncreaseIndent();

            sb.Append(GenerateDocumentation(enm.Documentation));

            if (enm.Members.Count == 0)
            {
                sb.AppendLine(Line("pass").TrimEnd());
            }
            else
            {
                for (int i = 0; i < enm.Members.Count; i++)
                {
                    var member = enm.Members[i];
                    sb.Append(GenerateDocumentation(member.Documentation));

                    if (member.HasExplicitValue)
                        sb.AppendLine(Line($"{member.Name} = {member.Value}").TrimEnd());
                    else
                        sb.AppendLine(Line($"{member.Name} = auto()").TrimEnd());
                }
            }

            DecreaseIndent();
            return sb.ToString();
        }

        #endregion

        #region Delegate

        public override string GenerateDelegate(DelegateElement del)
        {
            if (del.RawCode != null) return del.RawCode;

            // Python has no delegates — generate as a type alias using Callable
            var sb = new StringBuilder();

            sb.Append(GenerateDocumentation(del.Documentation));

            var paramTypes = del.Parameters.Select(p => GenerateTypeReference(p.Type)).ToList();
            var returnType = GenerateTypeReference(del.ReturnType);
            var paramsStr = paramTypes.Count > 0 ? string.Join(", ", paramTypes) : "";

            sb.AppendLine(Line($"{del.Name} = Callable[[{paramsStr}], {returnType}]").TrimEnd());

            return sb.ToString();
        }

        #endregion

        #region Field

        public override string GenerateField(FieldElement field)
        {
            if (field.RawCode != null) return field.RawCode;

            var sb = new StringBuilder();

            sb.Append(GenerateDocumentation(field.Documentation));

            var typeHint = GenerateTypeReference(field.Type);

            if (field.Modifiers.HasFlag(ElementModifiers.Const))
            {
                // Constants are module-level UPPER_CASE
                var name = field.Name?.ToUpper() ?? "CONSTANT";
                if (field.HasInitialValue)
                    sb.AppendLine(Line($"{name}: {typeHint} = {field.InitialValue}").TrimEnd());
                else
                    sb.AppendLine(Line($"{name}: {typeHint}").TrimEnd());
            }
            else if (field.Modifiers.HasFlag(ElementModifiers.Static))
            {
                // Class-level variable
                if (field.HasInitialValue)
                    sb.AppendLine(Line($"{field.Name}: ClassVar[{typeHint}] = {field.InitialValue}").TrimEnd());
                else
                    sb.AppendLine(Line($"{field.Name}: ClassVar[{typeHint}]").TrimEnd());
            }
            else
            {
                // Instance variable (normally in __init__)
                if (field.HasInitialValue)
                    sb.AppendLine(Line($"self.{field.Name}: {typeHint} = {field.InitialValue}").TrimEnd());
                else
                    sb.AppendLine(Line($"self.{field.Name}: {typeHint}").TrimEnd());
            }

            return sb.ToString();
        }

        #endregion

        #region Property

        public override string GenerateProperty(PropertyElement prop)
        {
            if (prop.RawCode != null) return prop.RawCode;

            var sb = new StringBuilder();
            var typeHint = GenerateTypeReference(prop.Type);
            var snakeName = ToSnakeCase(prop.Name);

            // Decorators (attributes)
            foreach (var attr in prop.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            // Getter
            if (prop.HasGetter)
            {
                sb.AppendLine(Line("@property").TrimEnd());
                sb.AppendLine(Line($"def {snakeName}(self) -> {typeHint}:").TrimEnd());
                IncreaseIndent();
                sb.Append(GenerateDocumentation(prop.Documentation));

                if (prop.IsExpressionBodied && !string.IsNullOrEmpty(prop.ExpressionBody))
                {
                    sb.AppendLine(Line($"return {prop.ExpressionBody}").TrimEnd());
                }
                else if (prop.GetterBody.HasStatements)
                {
                    GenerateStatements(sb, prop.GetterBody.Statements);
                }
                else
                {
                    sb.AppendLine(Line($"return self._{snakeName}").TrimEnd());
                }

                DecreaseIndent();
                sb.AppendLine();
            }

            // Setter
            if (prop.HasSetter && !prop.IsExpressionBodied)
            {
                sb.AppendLine(Line($"@{snakeName}.setter").TrimEnd());
                sb.AppendLine(Line($"def {snakeName}(self, value: {typeHint}) -> None:").TrimEnd());
                IncreaseIndent();

                if (prop.SetterBody.HasStatements)
                {
                    GenerateStatements(sb, prop.SetterBody.Statements);
                }
                else
                {
                    sb.AppendLine(Line($"self._{snakeName} = value").TrimEnd());
                }

                DecreaseIndent();
                sb.AppendLine();
            }

            return sb.ToString();
        }

        #endregion

        #region Method

        public override string GenerateMethod(MethodElement method)
        {
            if (method.RawCode != null) return method.RawCode;

            return GenerateMethodSignature(method, isAbstract: method.Modifiers.HasFlag(ElementModifiers.Abstract));
        }

        private string GenerateMethodSignature(MethodElement method, bool isAbstract)
        {
            var sb = new StringBuilder();

            // Decorators (attributes)
            foreach (var attr in method.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            if (method.Modifiers.HasFlag(ElementModifiers.Static))
            {
                sb.AppendLine(Line("@staticmethod").TrimEnd());
            }
            else if (isAbstract)
            {
                sb.AppendLine(Line("@abstractmethod").TrimEnd());
            }

            // Method signature
            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append("def ");
            declaration.Append(ToSnakeCase(method.Name));
            declaration.Append('(');

            var paramList = new List<string>();
            if (!method.Modifiers.HasFlag(ElementModifiers.Static))
                paramList.Add("self");

            foreach (var param in method.Parameters)
            {
                paramList.Add(GenerateParameter(param));
            }

            declaration.Append(string.Join(", ", paramList));
            declaration.Append(')');

            // Return type hint
            var returnType = GenerateTypeReference(method.ReturnType);
            declaration.Append($" -> {returnType}:");

            sb.AppendLine(declaration.ToString());
            IncreaseIndent();

            // Docstring
            sb.Append(GenerateDocumentation(method.Documentation));

            if (isAbstract || !method.HasBody)
            {
                sb.AppendLine(Line("...").TrimEnd());
            }
            else if (method.IsExpressionBodied && !string.IsNullOrEmpty(method.ExpressionBody))
            {
                sb.AppendLine(Line($"return {method.ExpressionBody}").TrimEnd());
            }
            else if (method.Body.HasStatements)
            {
                GenerateStatements(sb, method.Body.Statements);
            }
            else
            {
                sb.AppendLine(Line("pass").TrimEnd());
            }

            DecreaseIndent();
            return sb.ToString();
        }

        #endregion

        #region Constructor

        public override string GenerateConstructor(ConstructorElement ctor)
        {
            if (ctor.RawCode != null) return ctor.RawCode;

            var sb = new StringBuilder();

            // Decorators (attributes)
            foreach (var attr in ctor.Attributes)
            {
                sb.AppendLine(Line(GenerateAttribute(attr)).TrimEnd());
            }

            var declaration = new StringBuilder();
            declaration.Append(GetIndent());
            declaration.Append("def __init__(self");

            foreach (var param in ctor.Parameters)
            {
                declaration.Append(", ");
                declaration.Append(GenerateParameter(param));
            }

            declaration.Append(") -> None:");
            sb.AppendLine(declaration.ToString());

            IncreaseIndent();

            sb.Append(GenerateDocumentation(ctor.Documentation));

            // Base call
            if (ctor.BaseCall != null)
            {
                var args = string.Join(", ", ctor.BaseCall.Arguments);
                sb.AppendLine(Line($"super().__init__({args})").TrimEnd());
            }

            if (ctor.Body.HasStatements)
            {
                GenerateStatements(sb, ctor.Body.Statements);
            }
            else if (ctor.BaseCall == null)
            {
                sb.AppendLine(Line("pass").TrimEnd());
            }

            DecreaseIndent();
            return sb.ToString();
        }

        #endregion

        #region Event

        public override string GenerateEvent(EventElement evt)
        {
            if (evt.RawCode != null) return evt.RawCode;

            // Python has no events — generate as a list of callbacks
            var sb = new StringBuilder();
            sb.Append(GenerateDocumentation(evt.Documentation));
            var typeHint = GenerateTypeReference(evt.Type);
            sb.AppendLine(Line($"# Event: {evt.Name}").TrimEnd());
            sb.AppendLine(Line($"{ToSnakeCase(evt.Name)}: list[Callable] = []").TrimEnd());
            return sb.ToString();
        }

        #endregion

        #region Indexer

        public override string GenerateIndexer(IndexerElement indexer)
        {
            if (indexer.RawCode != null) return indexer.RawCode;

            var sb = new StringBuilder();
            var typeHint = GenerateTypeReference(indexer.Type);

            // __getitem__
            if (indexer.HasGetter)
            {
                var paramStr = string.Join(", ", indexer.Parameters.Select(p => GenerateParameter(p)));
                sb.AppendLine(Line($"def __getitem__(self, {paramStr}) -> {typeHint}:").TrimEnd());
                IncreaseIndent();
                sb.Append(GenerateDocumentation(indexer.Documentation));
                if (indexer.GetterBody.HasStatements)
                    GenerateStatements(sb, indexer.GetterBody.Statements);
                else
                    sb.AppendLine(Line("...").TrimEnd());
                DecreaseIndent();
                sb.AppendLine();
            }

            // __setitem__
            if (indexer.HasSetter)
            {
                var paramStr = string.Join(", ", indexer.Parameters.Select(p => GenerateParameter(p)));
                sb.AppendLine(Line($"def __setitem__(self, {paramStr}, value: {typeHint}) -> None:").TrimEnd());
                IncreaseIndent();
                if (indexer.SetterBody.HasStatements)
                    GenerateStatements(sb, indexer.SetterBody.Statements);
                else
                    sb.AppendLine(Line("...").TrimEnd());
                DecreaseIndent();
                sb.AppendLine();
            }

            return sb.ToString();
        }

        #endregion

        #region Operator

        public override string GenerateOperator(OperatorElement op)
        {
            if (op.RawCode != null) return op.RawCode;

            var sb = new StringBuilder();
            var dunderName = GetPythonOperatorDunder(op.OperatorType);
            var returnType = GenerateTypeReference(op.ReturnType);

            sb.Append(GenerateDocumentation(op.Documentation));

            var paramStr = string.Join(", ", op.Parameters.Select(p => GenerateParameter(p)));
            if (!string.IsNullOrEmpty(paramStr))
                paramStr = ", " + paramStr;

            sb.AppendLine(Line($"def {dunderName}(self{paramStr}) -> {returnType}:").TrimEnd());
            IncreaseIndent();

            if (op.Body.HasStatements)
                GenerateStatements(sb, op.Body.Statements);
            else
                sb.AppendLine(Line("...").TrimEnd());

            DecreaseIndent();
            return sb.ToString();
        }

        private static string GetPythonOperatorDunder(OperatorType operatorType)
        {
            return operatorType switch
            {
                OperatorType.UnaryPlus => "__pos__",
                OperatorType.UnaryMinus => "__neg__",
                OperatorType.LogicalNot => "__not__",
                OperatorType.BitwiseNot => "__invert__",
                OperatorType.Addition => "__add__",
                OperatorType.Subtraction => "__sub__",
                OperatorType.Multiplication => "__mul__",
                OperatorType.Division => "__truediv__",
                OperatorType.Modulus => "__mod__",
                OperatorType.BitwiseAnd => "__and__",
                OperatorType.BitwiseOr => "__or__",
                OperatorType.BitwiseXor => "__xor__",
                OperatorType.LeftShift => "__lshift__",
                OperatorType.RightShift => "__rshift__",
                OperatorType.Equality => "__eq__",
                OperatorType.Inequality => "__ne__",
                OperatorType.LessThan => "__lt__",
                OperatorType.GreaterThan => "__gt__",
                OperatorType.LessThanOrEqual => "__le__",
                OperatorType.GreaterThanOrEqual => "__ge__",
                OperatorType.Implicit => "__init__",
                OperatorType.Explicit => "__init__",
                _ => $"__operator_{operatorType.ToString().ToLower()}__"
            };
        }

        #endregion

        #region Attribute / Decorator

        public override string GenerateAttribute(AttributeElement attr)
        {
            if (attr.RawCode != null) return attr.RawCode;

            var sb = new StringBuilder();
            sb.Append('@');
            sb.Append(attr.AttributeName);

            var allArgs = new List<string>();
            allArgs.AddRange(attr.Arguments);
            allArgs.AddRange(attr.NamedArguments.Select(kv => $"{kv.Key}={kv.Value}"));

            if (allArgs.Count > 0)
            {
                sb.Append('(');
                sb.Append(string.Join(", ", allArgs));
                sb.Append(')');
            }

            return sb.ToString();
        }

        #endregion

        #region Parameter

        public override string GenerateParameter(ParameterElement param)
        {
            if (param.RawCode != null) return param.RawCode;

            var sb = new StringBuilder();

            // *args
            if (param.Modifier == ParameterModifier.VarArgs || param.Modifier == ParameterModifier.Params)
            {
                sb.Append('*');
                sb.Append(param.Name);
                sb.Append(": ");
                sb.Append(GenerateTypeReference(param.Type));
            }
            // **kwargs
            else if (param.Modifier == ParameterModifier.KeywordArgs)
            {
                sb.Append("**");
                sb.Append(param.Name);
                sb.Append(": ");
                sb.Append(GenerateTypeReference(param.Type));
            }
            else
            {
                sb.Append(param.Name);
                sb.Append(": ");
                sb.Append(GenerateTypeReference(param.Type));
            }

            if (param.HasDefaultValue)
            {
                sb.Append(" = ");
                sb.Append(MapDefaultValue(param.DefaultValue!));
            }

            return sb.ToString();
        }

        private static string MapDefaultValue(string defaultValue)
        {
            return defaultValue switch
            {
                "null" => "None",
                "true" => "True",
                "false" => "False",
                _ => defaultValue
            };
        }

        #endregion

        #region Type Reference

        public override string GenerateTypeReference(TypeReference typeRef)
        {
            var sb = new StringBuilder();

            var typeName = MapTypeName(typeRef.TypeName);

            // Generic arguments
            if (typeRef.GenericArguments.Count > 0)
            {
                sb.Append(typeName);
                sb.Append('[');
                sb.Append(string.Join(", ", typeRef.GenericArguments.Select(g => GenerateTypeReference(g))));
                sb.Append(']');
            }
            else
            {
                sb.Append(typeName);
            }

            // Array ? list[T]
            if (typeRef.IsArray)
            {
                var inner = sb.ToString();
                sb.Clear();
                sb.Append($"list[{inner}]");
            }

            // Nullable ? Optional[T] or T | None
            if (typeRef.IsNullable)
            {
                var inner = sb.ToString();
                sb.Clear();
                sb.Append($"{inner} | None");
            }

            return sb.ToString();
        }

        private static string MapTypeName(string typeName)
        {
            if (CSharpToPythonTypeMap.TryGetValue(typeName, out var pythonType))
                return pythonType;
            return typeName;
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
                AppendBody(sb, statement.RawCode);
                return;
            }

            switch (statement)
            {
                case CompositeStatement composite:
                    GenerateStatements(sb, composite.Statements);
                    break;

                case RawStatementElement raw:
                    AppendBody(sb, raw.Code);
                    break;

                case CommentStatement comment:
                    sb.AppendLine(Line($"# {comment.Text}").TrimEnd());
                    break;

                case AssignmentStatement assignment:
                    sb.AppendLine(Line($"{assignment.Left} = {assignment.Right}").TrimEnd());
                    break;

                case ReturnStatementElement returnStmt:
                    if (string.IsNullOrEmpty(returnStmt.Expression))
                        sb.AppendLine(Line("return").TrimEnd());
                    else
                        sb.AppendLine(Line($"return {returnStmt.Expression}").TrimEnd());
                    break;

                case ThrowStatementElement throwStmt:
                    if (string.IsNullOrEmpty(throwStmt.Expression))
                        sb.AppendLine(Line("raise").TrimEnd());
                    else
                        sb.AppendLine(Line($"raise {throwStmt.Expression}").TrimEnd());
                    break;

                case IfStatementElement ifStmt:
                    sb.AppendLine(Line($"if {ifStmt.Condition}:").TrimEnd());
                    IncreaseIndent();
                    if (ifStmt.ThenStatements.Statements.Count > 0)
                        GenerateStatements(sb, ifStmt.ThenStatements.Statements);
                    else
                        sb.AppendLine(Line("pass").TrimEnd());
                    DecreaseIndent();

                    foreach (var elseIf in ifStmt.ElseIfBranches)
                    {
                        sb.AppendLine(Line($"elif {elseIf.Condition}:").TrimEnd());
                        IncreaseIndent();
                        if (elseIf.Statements.Statements.Count > 0)
                            GenerateStatements(sb, elseIf.Statements.Statements);
                        else
                            sb.AppendLine(Line("pass").TrimEnd());
                        DecreaseIndent();
                    }

                    if (ifStmt.ElseStatements.Statements.Count > 0)
                    {
                        sb.AppendLine(Line("else:").TrimEnd());
                        IncreaseIndent();
                        GenerateStatements(sb, ifStmt.ElseStatements.Statements);
                        DecreaseIndent();
                    }
                    break;

                case ForStatementElement forStmt:
                    // Python has no C-style for — map to a range-based for
                    sb.AppendLine(Line($"# for ({forStmt.Initializer}; {forStmt.Condition}; {forStmt.Incrementer})").TrimEnd());
                    sb.AppendLine(Line($"while {forStmt.Condition ?? "True"}:").TrimEnd());
                    IncreaseIndent();
                    if (forStmt.Body.Statements.Count > 0)
                        GenerateStatements(sb, forStmt.Body.Statements);
                    else
                        sb.AppendLine(Line("pass").TrimEnd());
                    if (!string.IsNullOrEmpty(forStmt.Incrementer))
                        sb.AppendLine(Line(forStmt.Incrementer).TrimEnd());
                    DecreaseIndent();
                    break;

                case ForEachStatementElement forEachStmt:
                    sb.AppendLine(Line($"for {forEachStmt.VariableName} in {forEachStmt.Collection}:").TrimEnd());
                    IncreaseIndent();
                    if (forEachStmt.Body.Statements.Count > 0)
                        GenerateStatements(sb, forEachStmt.Body.Statements);
                    else
                        sb.AppendLine(Line("pass").TrimEnd());
                    DecreaseIndent();
                    break;

                case WhileStatementElement whileStmt:
                    sb.AppendLine(Line($"while {whileStmt.Condition}:").TrimEnd());
                    IncreaseIndent();
                    if (whileStmt.Body.Statements.Count > 0)
                        GenerateStatements(sb, whileStmt.Body.Statements);
                    else
                        sb.AppendLine(Line("pass").TrimEnd());
                    DecreaseIndent();
                    break;

                case SwitchStatementElement switchStmt:
                    // Python 3.10+ match statement
                    sb.AppendLine(Line($"match {switchStmt.Expression}:").TrimEnd());
                    IncreaseIndent();
                    foreach (var caseBlock in switchStmt.Cases)
                    {
                        var caseLabel = caseBlock.Labels.Count > 0
                            ? string.Join(" | ", caseBlock.Labels)
                            : caseBlock.Pattern ?? "_";
                        sb.AppendLine(Line($"case {caseLabel}:").TrimEnd());
                        IncreaseIndent();
                        if (caseBlock.Statements.Statements.Count > 0)
                            GenerateStatements(sb, caseBlock.Statements.Statements);
                        else
                            sb.AppendLine(Line("pass").TrimEnd());
                        DecreaseIndent();
                    }
                    if (switchStmt.DefaultStatements.Statements.Count > 0)
                    {
                        sb.AppendLine(Line("case _:").TrimEnd());
                        IncreaseIndent();
                        GenerateStatements(sb, switchStmt.DefaultStatements.Statements);
                        DecreaseIndent();
                    }
                    DecreaseIndent();
                    break;

                case TryCatchStatementElement tryCatch:
                    sb.AppendLine(Line("try:").TrimEnd());
                    IncreaseIndent();
                    if (tryCatch.TryStatements.Statements.Count > 0)
                        GenerateStatements(sb, tryCatch.TryStatements.Statements);
                    else
                        sb.AppendLine(Line("pass").TrimEnd());
                    DecreaseIndent();

                    foreach (var catchBlock in tryCatch.CatchBlocks)
                    {
                        var exceptLine = new StringBuilder("except");
                        if (catchBlock.ExceptionType != null)
                        {
                            exceptLine.Append($" {GenerateTypeReference(catchBlock.ExceptionType)}");
                            if (!string.IsNullOrEmpty(catchBlock.ExceptionVariable))
                                exceptLine.Append($" as {catchBlock.ExceptionVariable}");
                        }
                        exceptLine.Append(':');
                        sb.AppendLine(Line(exceptLine.ToString()).TrimEnd());
                        IncreaseIndent();
                        if (catchBlock.Statements.Statements.Count > 0)
                            GenerateStatements(sb, catchBlock.Statements.Statements);
                        else
                            sb.AppendLine(Line("pass").TrimEnd());
                        DecreaseIndent();
                    }

                    if (tryCatch.HasFinally)
                    {
                        sb.AppendLine(Line("finally:").TrimEnd());
                        IncreaseIndent();
                        GenerateStatements(sb, tryCatch.FinallyStatements.Statements);
                        DecreaseIndent();
                    }
                    break;

                case UsingStatementElement usingStmt:
                    // Python 'with' statement
                    sb.AppendLine(Line($"with {usingStmt.Resource}:").TrimEnd());
                    IncreaseIndent();
                    if (usingStmt.Body.Statements.Count > 0)
                        GenerateStatements(sb, usingStmt.Body.Statements);
                    else
                        sb.AppendLine(Line("pass").TrimEnd());
                    DecreaseIndent();
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
            // Python has no access modifiers — convention is _ prefix for private
            return string.Empty;
        }

        public override string GenerateModifiers(ElementModifiers modifiers)
        {
            // Python has no keyword modifiers — handled via decorators
            return string.Empty;
        }

        public override string GenerateDocumentation(string? documentation)
        {
            if (string.IsNullOrEmpty(documentation))
                return string.Empty;

            var sb = new StringBuilder();
            var lines = documentation.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);

            if (lines.Length == 1)
            {
                sb.AppendLine(Line($"\"\"\"{documentation}\"\"\"").TrimEnd());
            }
            else
            {
                sb.AppendLine(Line("\"\"\"").TrimEnd());
                foreach (var line in lines)
                {
                    sb.AppendLine(Line(line).TrimEnd());
                }
                sb.AppendLine(Line("\"\"\"").TrimEnd());
            }

            return sb.ToString();
        }

        private void AppendBody(StringBuilder sb, string body)
        {
            var lines = body.Split('\n');
            foreach (var bodyLine in lines)
            {
                var trimmedLine = bodyLine.TrimEnd('\r');
                if (!string.IsNullOrWhiteSpace(trimmedLine))
                    sb.AppendLine(Line(trimmedLine.TrimStart()).TrimEnd());
                else
                    sb.AppendLine();
            }
        }

        private static string GetDefaultValue(string typeHint)
        {
            return typeHint switch
            {
                "int" => "0",
                "float" => "0.0",
                "bool" => "False",
                "str" => "\"\"",
                _ => "None"
            };
        }

        private static string ToSnakeCase(string? name)
        {
            if (string.IsNullOrEmpty(name)) return "unnamed";

            var sb = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                var c = name[i];
                if (char.IsUpper(c))
                {
                    if (i > 0 && !char.IsUpper(name[i - 1]))
                        sb.Append('_');
                    else if (i > 0 && i < name.Length - 1 && char.IsUpper(name[i - 1]) && !char.IsUpper(name[i + 1]))
                        sb.Append('_');
                    sb.Append(char.ToLower(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        #endregion
    }
}
