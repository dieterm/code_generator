using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.CodeElements.Statements;
using CodeGenerator.Domain.ProgrammingLanguages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace CodeGenerator.Domain.ProgrammingLanguages.CSharp
{
    /// <summary>
    /// Code parser for C# programming language.
    /// Parses C# source code into CodeElement structures using Roslyn.
    /// </summary>
    public class CSharpCodeParser : ProgrammingLanguageCodeParser
    {
        public override ProgrammingLanguage Language => CSharpLanguage.Instance;

        public override CodeFileElement ParseCodeFile(string sourceCode, string fileName)
        {
            var tree = CSharpSyntaxTree.ParseText(sourceCode);
            var root = tree.GetCompilationUnitRoot();

            var codeFile = new CodeFileElement(fileName, CSharpLanguage.Instance);

            // File header (leading trivia before first token)
            codeFile.FileHeader = ExtractFileHeader(root);

            // Nullable directive
            foreach (var directive in root.GetFirstToken().LeadingTrivia)
            {
                if (directive.IsKind(SyntaxKind.NullableDirectiveTrivia))
                {
                    var text = directive.ToString();
                    codeFile.NullableContext = text.Contains("enable");
                }
            }

            // Usings
            foreach (var usingDirective in root.Usings)
            {
                var usingElement = ParseUsing(usingDirective);
                if (usingElement.IsGlobal)
                    codeFile.GlobalUsings.Add(usingElement);
                else
                    codeFile.Usings.Add(usingElement);
            }

            // Assembly/module attributes
            foreach (var attrList in root.AttributeLists)
            {
                var target = attrList.Target?.Identifier.Text.ToLower();
                if (target == "assembly" || target == "module")
                {
                    foreach (var attr in attrList.Attributes)
                    {
                        var attrElement = ParseAttribute(attr);
                        attrElement.Target = target == "assembly" ? AttributeTarget.Assembly : AttributeTarget.Module;
                        codeFile.FileAttributes.Add(attrElement);
                    }
                }
            }

            // Namespaces and top-level types
            foreach (var member in root.Members)
            {
                switch (member)
                {
                    case BaseNamespaceDeclarationSyntax nsSyntax:
                        codeFile.Namespaces.Add(ParseNamespace(nsSyntax));
                        break;
                    case TypeDeclarationSyntax typeSyntax:
                        codeFile.TopLevelTypes.Add(ParseType(typeSyntax));
                        break;
                    case EnumDeclarationSyntax enumSyntax:
                        codeFile.TopLevelTypes.Add(ParseEnum(enumSyntax));
                        break;
                    case DelegateDeclarationSyntax delSyntax:
                        codeFile.TopLevelTypes.Add(ParseDelegate(delSyntax));
                        break;
                    case GlobalStatementSyntax globalStmt:
                        codeFile.TopLevelStatements.Add(globalStmt.Statement.ToString().Trim());
                        break;
                }
            }

            return codeFile;
        }

        #region File Header

        private string? ExtractFileHeader(CompilationUnitSyntax root)
        {
            var firstToken = root.GetFirstToken(includeZeroWidth: true);
            var leadingTrivia = firstToken.LeadingTrivia;

            var headerLines = new List<string>();
            foreach (var trivia in leadingTrivia)
            {
                if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                {
                    var text = trivia.ToString();
                    if (text.StartsWith("// "))
                        headerLines.Add(text.Substring(3));
                    else if (text.StartsWith("//"))
                        headerLines.Add(text.Substring(2));
                }
                else if (trivia.IsKind(SyntaxKind.EndOfLineTrivia) && headerLines.Count > 0)
                {
                    // Continue collecting
                }
                else if (!trivia.IsKind(SyntaxKind.WhitespaceTrivia) && !trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    break;
                }
            }

            if (headerLines.Count == 0) return null;

            // Only treat as file header if it appears before any usings/namespace/type
            var result = string.Join(Environment.NewLine, headerLines);
            return string.IsNullOrWhiteSpace(result) ? null : result;
        }

        #endregion

        #region Using

        private UsingElement ParseUsing(UsingDirectiveSyntax usingDirective)
        {
            var element = new UsingElement();

            element.IsGlobal = usingDirective.GlobalKeyword != default;
            element.IsStatic = usingDirective.StaticKeyword != default;

            if (usingDirective.Alias != null)
            {
                element.Alias = usingDirective.Alias.Name.ToString();
                element.Namespace = usingDirective.NamespaceOrType.ToString();
            }
            else
            {
                element.Namespace = usingDirective.NamespaceOrType.ToString();
            }

            return element;
        }

        #endregion

        #region Namespace

        private NamespaceElement ParseNamespace(BaseNamespaceDeclarationSyntax nsSyntax)
        {
            var ns = new NamespaceElement(nsSyntax.Name.ToString());
            ns.IsFileScoped = nsSyntax is FileScopedNamespaceDeclarationSyntax;

            // Usings within namespace
            foreach (var usingDirective in nsSyntax.Usings)
            {
                ns.Usings.Add(ParseUsing(usingDirective));
            }

            // Members
            foreach (var member in nsSyntax.Members)
            {
                switch (member)
                {
                    case BaseNamespaceDeclarationSyntax nestedNs:
                        ns.NestedNamespaces.Add(ParseNamespace(nestedNs));
                        break;
                    case TypeDeclarationSyntax typeSyntax:
                        ns.Types.Add(ParseType(typeSyntax));
                        break;
                    case EnumDeclarationSyntax enumSyntax:
                        ns.Types.Add(ParseEnum(enumSyntax));
                        break;
                    case DelegateDeclarationSyntax delSyntax:
                        ns.Types.Add(ParseDelegate(delSyntax));
                        break;
                }
            }

            ParseCodeElementBase(ns, nsSyntax);
            return ns;
        }

        #endregion

        #region Types

        private TypeElement ParseType(TypeDeclarationSyntax typeSyntax)
        {
            return typeSyntax switch
            {
                ClassDeclarationSyntax cls => ParseClass(cls),
                InterfaceDeclarationSyntax iface => ParseInterface(iface),
                StructDeclarationSyntax str => ParseStruct(str),
                RecordDeclarationSyntax rec => ParseRecord(rec),
                _ => ParseClassFallback(typeSyntax)
            };
        }

        private ClassElement ParseClass(ClassDeclarationSyntax cls)
        {
            var element = new ClassElement(cls.Identifier.Text);
            ParseCodeElementBase(element, cls);
            ParseAccessAndModifiers(element, cls.Modifiers);
            ParseTypeElementBase(element, cls);

            // Members
            foreach (var member in cls.Members)
                ParseTypeMember(element, member);

            return element;
        }

        private ClassElement ParseClassFallback(TypeDeclarationSyntax typeSyntax)
        {
            var element = new ClassElement(typeSyntax.Identifier.Text);
            ParseCodeElementBase(element, typeSyntax);
            ParseAccessAndModifiers(element, typeSyntax.Modifiers);
            ParseTypeElementBase(element, typeSyntax);

            foreach (var member in typeSyntax.Members)
                ParseTypeMember(element, member);

            return element;
        }

        private TypeElement ParseRecord(RecordDeclarationSyntax rec)
        {
            if (rec.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword))
            {
                var element = new StructElement(rec.Identifier.Text);
                element.IsRecord = true;
                ParseCodeElementBase(element, rec);
                ParseAccessAndModifiers(element, rec.Modifiers);
                ParseTypeElementBase(element, rec);

                if (rec.ParameterList != null)
                {
                    foreach (var param in rec.ParameterList.Parameters)
                        element.PrimaryConstructorParameters.Add(ParseParameter(param));
                }

                foreach (var member in rec.Members)
                    ParseStructMember(element, member);

                return element;
            }
            else
            {
                var element = new ClassElement(rec.Identifier.Text);
                element.IsRecord = true;
                ParseCodeElementBase(element, rec);
                ParseAccessAndModifiers(element, rec.Modifiers);
                ParseTypeElementBase(element, rec);

                if (rec.ParameterList != null)
                {
                    foreach (var param in rec.ParameterList.Parameters)
                        element.PrimaryConstructorParameters.Add(ParseParameter(param));
                }

                foreach (var member in rec.Members)
                    ParseTypeMember(element, member);

                return element;
            }
        }

        private InterfaceElement ParseInterface(InterfaceDeclarationSyntax iface)
        {
            var element = new InterfaceElement(iface.Identifier.Text);
            ParseCodeElementBase(element, iface);
            ParseAccessAndModifiers(element, iface.Modifiers);
            ParseTypeElementBase(element, iface);

            foreach (var member in iface.Members)
            {
                switch (member)
                {
                    case PropertyDeclarationSyntax prop:
                        element.Properties.Add(ParseProperty(prop));
                        break;
                    case MethodDeclarationSyntax method:
                        element.Methods.Add(ParseMethod(method));
                        break;
                    case EventDeclarationSyntax evt:
                        element.Events.Add(ParseEvent(evt));
                        break;
                    case EventFieldDeclarationSyntax evtField:
                        foreach (var v in evtField.Declaration.Variables)
                            element.Events.Add(ParseEventField(evtField, v));
                        break;
                    case IndexerDeclarationSyntax indexer:
                        element.Indexers.Add(ParseIndexer(indexer));
                        break;
                    case TypeDeclarationSyntax nestedType:
                        element.NestedTypes.Add(ParseType(nestedType));
                        break;
                    case EnumDeclarationSyntax nestedEnum:
                        element.NestedTypes.Add(ParseEnum(nestedEnum));
                        break;
                    case DelegateDeclarationSyntax nestedDel:
                        element.NestedTypes.Add(ParseDelegate(nestedDel));
                        break;
                }
            }

            return element;
        }

        private StructElement ParseStruct(StructDeclarationSyntax str)
        {
            var element = new StructElement(str.Identifier.Text);
            ParseCodeElementBase(element, str);
            ParseAccessAndModifiers(element, str.Modifiers);
            ParseTypeElementBase(element, str);

            if (element.Modifiers.HasFlag(ElementModifiers.Readonly))
            {
                element.IsReadonly = true;
                element.Modifiers &= ~ElementModifiers.Readonly;
            }

            foreach (var member in str.Members)
                ParseStructMember(element, member);

            return element;
        }

        private void ParseTypeMember(ClassElement cls, MemberDeclarationSyntax member)
        {
            switch (member)
            {
                case FieldDeclarationSyntax field:
                    foreach (var v in field.Declaration.Variables)
                        cls.Fields.Add(ParseField(field, v));
                    break;
                case PropertyDeclarationSyntax prop:
                    cls.Properties.Add(ParseProperty(prop));
                    break;
                case MethodDeclarationSyntax method:
                    cls.Methods.Add(ParseMethod(method));
                    break;
                case ConstructorDeclarationSyntax ctor:
                    cls.Constructors.Add(ParseConstructor(ctor));
                    break;
                case DestructorDeclarationSyntax dtor:
                    cls.Finalizer = ParseFinalizer(dtor);
                    break;
                case EventDeclarationSyntax evt:
                    cls.Events.Add(ParseEvent(evt));
                    break;
                case EventFieldDeclarationSyntax evtField:
                    foreach (var v in evtField.Declaration.Variables)
                        cls.Events.Add(ParseEventField(evtField, v));
                    break;
                case IndexerDeclarationSyntax indexer:
                    cls.Indexers.Add(ParseIndexer(indexer));
                    break;
                case OperatorDeclarationSyntax op:
                    cls.Operators.Add(ParseOperator(op));
                    break;
                case ConversionOperatorDeclarationSyntax convOp:
                    cls.Operators.Add(ParseConversionOperator(convOp));
                    break;
                case TypeDeclarationSyntax nestedType:
                    cls.NestedTypes.Add(ParseType(nestedType));
                    break;
                case EnumDeclarationSyntax nestedEnum:
                    cls.NestedTypes.Add(ParseEnum(nestedEnum));
                    break;
                case DelegateDeclarationSyntax nestedDel:
                    cls.NestedTypes.Add(ParseDelegate(nestedDel));
                    break;
            }
        }

        private void ParseStructMember(StructElement str, MemberDeclarationSyntax member)
        {
            switch (member)
            {
                case FieldDeclarationSyntax field:
                    foreach (var v in field.Declaration.Variables)
                        str.Fields.Add(ParseField(field, v));
                    break;
                case PropertyDeclarationSyntax prop:
                    str.Properties.Add(ParseProperty(prop));
                    break;
                case MethodDeclarationSyntax method:
                    str.Methods.Add(ParseMethod(method));
                    break;
                case ConstructorDeclarationSyntax ctor:
                    str.Constructors.Add(ParseConstructor(ctor));
                    break;
                case EventDeclarationSyntax evt:
                    str.Events.Add(ParseEvent(evt));
                    break;
                case EventFieldDeclarationSyntax evtField:
                    foreach (var v in evtField.Declaration.Variables)
                        str.Events.Add(ParseEventField(evtField, v));
                    break;
                case TypeDeclarationSyntax nestedType:
                    str.NestedTypes.Add(ParseType(nestedType));
                    break;
                case EnumDeclarationSyntax nestedEnum:
                    str.NestedTypes.Add(ParseEnum(nestedEnum));
                    break;
                case DelegateDeclarationSyntax nestedDel:
                    str.NestedTypes.Add(ParseDelegate(nestedDel));
                    break;
            }
        }

        private void ParseTypeElementBase(TypeElement element, TypeDeclarationSyntax typeSyntax)
        {
            // Generic type parameters
            if (typeSyntax.TypeParameterList != null)
            {
                foreach (var tp in typeSyntax.TypeParameterList.Parameters)
                    element.GenericTypeParameters.Add(ParseGenericTypeParameter(tp));
            }

            // Base types
            if (typeSyntax.BaseList != null)
            {
                foreach (var baseType in typeSyntax.BaseList.Types)
                    element.BaseTypes.Add(ParseTypeReference(baseType.Type));
            }

            // Generic constraints
            foreach (var constraint in typeSyntax.ConstraintClauses)
                element.GenericConstraints.Add(ParseGenericConstraint(constraint));
        }

        #endregion

        #region Enum

        private EnumElement ParseEnum(EnumDeclarationSyntax enumSyntax)
        {
            var element = new EnumElement(enumSyntax.Identifier.Text);
            ParseCodeElementBase(element, enumSyntax);
            ParseAccessAndModifiers(element, enumSyntax.Modifiers);

            // Underlying type
            if (enumSyntax.BaseList != null && enumSyntax.BaseList.Types.Count > 0)
                element.UnderlyingType = ParseTypeReference(enumSyntax.BaseList.Types[0].Type);

            // Check for [Flags] attribute
            element.IsFlags = element.Attributes.Any(a => a.AttributeName == "Flags");
            if (element.IsFlags)
                element.Attributes.RemoveAll(a => a.AttributeName == "Flags");

            // Members
            foreach (var member in enumSyntax.Members)
            {
                var memberElement = new EnumMemberElement(member.Identifier.Text);
                if (member.EqualsValue != null)
                    memberElement.Value = member.EqualsValue.Value.ToString();
                memberElement.Documentation = ExtractDocumentation(member);
                ParseAttributesFromSyntax(memberElement, member.AttributeLists);
                element.Members.Add(memberElement);
            }

            return element;
        }

        #endregion

        #region Delegate

        private DelegateElement ParseDelegate(DelegateDeclarationSyntax delSyntax)
        {
            var element = new DelegateElement();
            element.Name = delSyntax.Identifier.Text;
            element.ReturnType = ParseTypeReference(delSyntax.ReturnType);
            ParseCodeElementBase(element, delSyntax);
            ParseAccessAndModifiers(element, delSyntax.Modifiers);

            // Generic type parameters
            if (delSyntax.TypeParameterList != null)
            {
                foreach (var tp in delSyntax.TypeParameterList.Parameters)
                    element.GenericTypeParameters.Add(ParseGenericTypeParameter(tp));
            }

            // Constraints
            foreach (var constraint in delSyntax.ConstraintClauses)
                element.GenericConstraints.Add(ParseGenericConstraint(constraint));

            // Parameters
            foreach (var param in delSyntax.ParameterList.Parameters)
                element.Parameters.Add(ParseParameter(param));

            return element;
        }

        #endregion

        #region Field

        private FieldElement ParseField(FieldDeclarationSyntax fieldSyntax, VariableDeclaratorSyntax variable)
        {
            var element = new FieldElement();
            element.Name = variable.Identifier.Text;
            element.Type = ParseTypeReference(fieldSyntax.Declaration.Type);
            ParseCodeElementBase(element, fieldSyntax);
            ParseAccessAndModifiers(element, fieldSyntax.Modifiers);

            if (variable.Initializer != null)
                element.InitialValue = variable.Initializer.Value.ToString();

            return element;
        }

        #endregion

        #region Property

        private PropertyElement ParseProperty(PropertyDeclarationSyntax propSyntax)
        {
            var element = new PropertyElement();
            element.Name = propSyntax.Identifier.Text;
            element.Type = ParseTypeReference(propSyntax.Type);
            ParseCodeElementBase(element, propSyntax);
            ParseAccessAndModifiers(element, propSyntax.Modifiers);

            // Expression-bodied property
            if (propSyntax.ExpressionBody != null)
            {
                element.IsExpressionBodied = true;
                element.ExpressionBody = propSyntax.ExpressionBody.Expression.ToString();
                element.HasGetter = true;
                element.HasSetter = false;
                element.IsAutoImplemented = false;
                return element;
            }

            // Accessors
            if (propSyntax.AccessorList != null)
            {
                element.HasGetter = false;
                element.HasSetter = false;

                foreach (var accessor in propSyntax.AccessorList.Accessors)
                {
                    if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                    {
                        element.HasGetter = true;
                        var accessMod = ParseAccessModifierFromTokens(accessor.Modifiers);
                        if (accessMod != AccessModifier.Public)
                            element.GetterAccessModifier = accessMod;

                        if (accessor.Body != null)
                        {
                            element.IsAutoImplemented = false;
                            ParseStatements(element.GetterBody, accessor.Body.Statements);
                        }
                        else if (accessor.ExpressionBody != null)
                        {
                            element.IsAutoImplemented = false;
                            element.GetterBody.Statements.Add(new ReturnStatementElement(accessor.ExpressionBody.Expression.ToString()));
                        }
                    }
                    else if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
                    {
                        element.HasSetter = true;
                        var accessMod = ParseAccessModifierFromTokens(accessor.Modifiers);
                        if (accessMod != AccessModifier.Public)
                            element.SetterAccessModifier = accessMod;

                        if (accessor.Body != null)
                        {
                            element.IsAutoImplemented = false;
                            ParseStatements(element.SetterBody, accessor.Body.Statements);
                        }
                        else if (accessor.ExpressionBody != null)
                        {
                            element.IsAutoImplemented = false;
                            element.SetterBody.Statements.Add(new RawStatementElement(accessor.ExpressionBody.Expression.ToString()));
                        }
                    }
                    else if (accessor.IsKind(SyntaxKind.InitAccessorDeclaration))
                    {
                        element.HasSetter = true;
                        element.IsInitOnly = true;
                        var accessMod = ParseAccessModifierFromTokens(accessor.Modifiers);
                        if (accessMod != AccessModifier.Public)
                            element.SetterAccessModifier = accessMod;

                        if (accessor.Body != null)
                        {
                            element.IsAutoImplemented = false;
                            ParseStatements(element.SetterBody, accessor.Body.Statements);
                        }
                    }
                }
            }

            // Initializer
            if (propSyntax.Initializer != null)
                element.InitialValue = propSyntax.Initializer.Value.ToString();

            return element;
        }

        #endregion

        #region Method

        private MethodElement ParseMethod(MethodDeclarationSyntax methodSyntax)
        {
            var element = new MethodElement();
            element.Name = methodSyntax.Identifier.Text;
            element.ReturnType = ParseTypeReference(methodSyntax.ReturnType);
            ParseCodeElementBase(element, methodSyntax);
            ParseAccessAndModifiers(element, methodSyntax.Modifiers);

            // Generic type parameters
            if (methodSyntax.TypeParameterList != null)
            {
                foreach (var tp in methodSyntax.TypeParameterList.Parameters)
                    element.GenericTypeParameters.Add(ParseGenericTypeParameter(tp));
            }

            // Constraints
            foreach (var constraint in methodSyntax.ConstraintClauses)
                element.GenericConstraints.Add(ParseGenericConstraint(constraint));

            // Parameters
            foreach (var param in methodSyntax.ParameterList.Parameters)
            {
                var paramElement = ParseParameter(param);
                element.Parameters.Add(paramElement);
            }

            // Extension method check
            if (element.Parameters.Count > 0 && element.Parameters[0].IsExtensionMethodThis)
                element.IsExtensionMethod = true;

            // Expression body
            if (methodSyntax.ExpressionBody != null)
            {
                element.IsExpressionBodied = true;
                element.ExpressionBody = methodSyntax.ExpressionBody.Expression.ToString();
            }
            // Block body
            else if (methodSyntax.Body != null)
            {
                ParseStatements(element.Body, methodSyntax.Body.Statements);
            }

            return element;
        }

        #endregion

        #region Constructor

        private ConstructorElement ParseConstructor(ConstructorDeclarationSyntax ctorSyntax)
        {
            var element = new ConstructorElement();
            element.Name = ctorSyntax.Identifier.Text;
            ParseCodeElementBase(element, ctorSyntax);
            ParseAccessAndModifiers(element, ctorSyntax.Modifiers);

            if (ctorSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                element.IsStatic = true;
                element.Modifiers &= ~ElementModifiers.Static;
            }

            // Parameters
            foreach (var param in ctorSyntax.ParameterList.Parameters)
                element.Parameters.Add(ParseParameter(param));

            // Initializer (base or this call)
            if (ctorSyntax.Initializer != null)
            {
                var args = ctorSyntax.Initializer.ArgumentList.Arguments
                    .Select(a => a.ToString()).ToList();

                if (ctorSyntax.Initializer.ThisOrBaseKeyword.IsKind(SyntaxKind.BaseKeyword))
                    element.BaseCall = new ConstructorInitializer { Arguments = args };
                else
                    element.ThisCall = new ConstructorInitializer { Arguments = args };
            }

            // Body
            if (ctorSyntax.Body != null)
                ParseStatements(element.Body, ctorSyntax.Body.Statements);

            return element;
        }

        private FinalizerElement ParseFinalizer(DestructorDeclarationSyntax dtorSyntax)
        {
            var element = new FinalizerElement();
            element.Documentation = ExtractDocumentation(dtorSyntax);

            if (dtorSyntax.Body != null)
                element.Body = dtorSyntax.Body.Statements.ToString().Trim();

            return element;
        }

        #endregion

        #region Event

        private EventElement ParseEvent(EventDeclarationSyntax evtSyntax)
        {
            var element = new EventElement();
            element.Name = evtSyntax.Identifier.Text;
            element.Type = ParseTypeReference(evtSyntax.Type);
            element.IsFieldLike = false;
            ParseCodeElementBase(element, evtSyntax);
            ParseAccessAndModifiers(element, evtSyntax.Modifiers);

            if (evtSyntax.AccessorList != null)
            {
                foreach (var accessor in evtSyntax.AccessorList.Accessors)
                {
                    if (accessor.IsKind(SyntaxKind.AddAccessorDeclaration) && accessor.Body != null)
                        element.AddAccessorBody = accessor.Body.Statements.ToString().Trim();
                    else if (accessor.IsKind(SyntaxKind.RemoveAccessorDeclaration) && accessor.Body != null)
                        element.RemoveAccessorBody = accessor.Body.Statements.ToString().Trim();
                }
            }

            return element;
        }

        private EventElement ParseEventField(EventFieldDeclarationSyntax evtFieldSyntax, VariableDeclaratorSyntax variable)
        {
            var element = new EventElement();
            element.Name = variable.Identifier.Text;
            element.Type = ParseTypeReference(evtFieldSyntax.Declaration.Type);
            element.IsFieldLike = true;
            ParseCodeElementBase(element, evtFieldSyntax);
            ParseAccessAndModifiers(element, evtFieldSyntax.Modifiers);
            return element;
        }

        #endregion

        #region Indexer

        private IndexerElement ParseIndexer(IndexerDeclarationSyntax indexerSyntax)
        {
            var element = new IndexerElement();
            element.Type = ParseTypeReference(indexerSyntax.Type);
            ParseCodeElementBase(element, indexerSyntax);
            ParseAccessAndModifiers(element, indexerSyntax.Modifiers);

            foreach (var param in indexerSyntax.ParameterList.Parameters)
                element.Parameters.Add(ParseParameter(param));

            element.HasGetter = false;
            element.HasSetter = false;

            if (indexerSyntax.AccessorList != null)
            {
                foreach (var accessor in indexerSyntax.AccessorList.Accessors)
                {
                    if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                    {
                        element.HasGetter = true;
                        var accessMod = ParseAccessModifierFromTokens(accessor.Modifiers);
                        if (accessMod != AccessModifier.Public)
                            element.GetterAccessModifier = accessMod;
                        if (accessor.Body != null)
                            ParseStatements(element.GetterBody, accessor.Body.Statements);
                        else if (accessor.ExpressionBody != null)
                            element.GetterBody.Statements.Add(new ReturnStatementElement(accessor.ExpressionBody.Expression.ToString()));
                    }
                    else if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
                    {
                        element.HasSetter = true;
                        var accessMod = ParseAccessModifierFromTokens(accessor.Modifiers);
                        if (accessMod != AccessModifier.Public)
                            element.SetterAccessModifier = accessMod;
                        if (accessor.Body != null)
                            ParseStatements(element.SetterBody, accessor.Body.Statements);
                        else if (accessor.ExpressionBody != null)
                            element.SetterBody.Statements.Add(new RawStatementElement(accessor.ExpressionBody.Expression.ToString()));
                    }
                }
            }
            else if (indexerSyntax.ExpressionBody != null)
            {
                element.HasGetter = true;
                element.GetterBody.Statements.Add(new ReturnStatementElement(indexerSyntax.ExpressionBody.Expression.ToString()));
            }

            return element;
        }

        #endregion

        #region Operator

        private OperatorElement ParseOperator(OperatorDeclarationSyntax opSyntax)
        {
            var element = new OperatorElement();
            element.ReturnType = ParseTypeReference(opSyntax.ReturnType);
            element.OperatorType = ParseOperatorType(opSyntax.OperatorToken);
            ParseCodeElementBase(element, opSyntax);
            ParseAccessAndModifiers(element, opSyntax.Modifiers);

            foreach (var param in opSyntax.ParameterList.Parameters)
                element.Parameters.Add(ParseParameter(param));

            if (opSyntax.Body != null)
                ParseStatements(element.Body, opSyntax.Body.Statements);
            else if (opSyntax.ExpressionBody != null)
                element.Body.Statements.Add(new ReturnStatementElement(opSyntax.ExpressionBody.Expression.ToString()));

            return element;
        }

        private OperatorElement ParseConversionOperator(ConversionOperatorDeclarationSyntax convOpSyntax)
        {
            var element = new OperatorElement();
            element.ReturnType = ParseTypeReference(convOpSyntax.Type);
            element.IsImplicit = convOpSyntax.ImplicitOrExplicitKeyword.IsKind(SyntaxKind.ImplicitKeyword);
            element.OperatorType = element.IsImplicit ? OperatorType.Implicit : OperatorType.Explicit;
            ParseCodeElementBase(element, convOpSyntax);
            ParseAccessAndModifiers(element, convOpSyntax.Modifiers);

            foreach (var param in convOpSyntax.ParameterList.Parameters)
                element.Parameters.Add(ParseParameter(param));

            if (convOpSyntax.Body != null)
                ParseStatements(element.Body, convOpSyntax.Body.Statements);
            else if (convOpSyntax.ExpressionBody != null)
                element.Body.Statements.Add(new ReturnStatementElement(convOpSyntax.ExpressionBody.Expression.ToString()));

            return element;
        }

        private OperatorType ParseOperatorType(SyntaxToken token)
        {
            return token.Kind() switch
            {
                SyntaxKind.PlusToken => OperatorType.Addition,
                SyntaxKind.MinusToken => OperatorType.Subtraction,
                SyntaxKind.AsteriskToken => OperatorType.Multiplication,
                SyntaxKind.SlashToken => OperatorType.Division,
                SyntaxKind.PercentToken => OperatorType.Modulus,
                SyntaxKind.AmpersandToken => OperatorType.BitwiseAnd,
                SyntaxKind.BarToken => OperatorType.BitwiseOr,
                SyntaxKind.CaretToken => OperatorType.BitwiseXor,
                SyntaxKind.LessThanLessThanToken => OperatorType.LeftShift,
                SyntaxKind.GreaterThanGreaterThanToken => OperatorType.RightShift,
                SyntaxKind.GreaterThanGreaterThanGreaterThanToken => OperatorType.UnsignedRightShift,
                SyntaxKind.EqualsEqualsToken => OperatorType.Equality,
                SyntaxKind.ExclamationEqualsToken => OperatorType.Inequality,
                SyntaxKind.LessThanToken => OperatorType.LessThan,
                SyntaxKind.GreaterThanToken => OperatorType.GreaterThan,
                SyntaxKind.LessThanEqualsToken => OperatorType.LessThanOrEqual,
                SyntaxKind.GreaterThanEqualsToken => OperatorType.GreaterThanOrEqual,
                SyntaxKind.ExclamationToken => OperatorType.LogicalNot,
                SyntaxKind.TildeToken => OperatorType.BitwiseNot,
                SyntaxKind.PlusPlusToken => OperatorType.Increment,
                SyntaxKind.MinusMinusToken => OperatorType.Decrement,
                SyntaxKind.TrueKeyword => OperatorType.True,
                SyntaxKind.FalseKeyword => OperatorType.False,
                _ => OperatorType.Addition
            };
        }

        #endregion

        #region Parameter

        private ParameterElement ParseParameter(ParameterSyntax paramSyntax)
        {
            var element = new ParameterElement();
            element.Name = paramSyntax.Identifier.Text;
            element.Type = paramSyntax.Type != null ? ParseTypeReference(paramSyntax.Type) : new TypeReference("object");

            // Modifier
            foreach (var mod in paramSyntax.Modifiers)
            {
                switch (mod.Kind())
                {
                    case SyntaxKind.RefKeyword: element.Modifier = ParameterModifier.Ref; break;
                    case SyntaxKind.OutKeyword: element.Modifier = ParameterModifier.Out; break;
                    case SyntaxKind.InKeyword: element.Modifier = ParameterModifier.In; break;
                    case SyntaxKind.ParamsKeyword: element.Modifier = ParameterModifier.Params; break;
                    case SyntaxKind.ThisKeyword: element.IsExtensionMethodThis = true; break;
                }
            }

            // Default value
            if (paramSyntax.Default != null)
                element.DefaultValue = paramSyntax.Default.Value.ToString();

            // Attributes
            ParseAttributesFromSyntax(element, paramSyntax.AttributeLists);

            return element;
        }

        #endregion

        #region Attribute

        private AttributeElement ParseAttribute(AttributeSyntax attrSyntax)
        {
            var element = new AttributeElement();
            element.AttributeName = attrSyntax.Name.ToString();

            if (attrSyntax.ArgumentList != null)
            {
                foreach (var arg in attrSyntax.ArgumentList.Arguments)
                {
                    if (arg.NameEquals != null)
                        element.NamedArguments[arg.NameEquals.Name.ToString()] = arg.Expression.ToString();
                    else if (arg.NameColon != null)
                        element.Arguments.Add($"{arg.NameColon.Name}: {arg.Expression}");
                    else
                        element.Arguments.Add(arg.Expression.ToString());
                }
            }

            return element;
        }

        #endregion

        #region Generics

        private GenericTypeParameterElement ParseGenericTypeParameter(TypeParameterSyntax tp)
        {
            var element = new GenericTypeParameterElement(tp.Identifier.Text);

            if (tp.VarianceKeyword.IsKind(SyntaxKind.OutKeyword))
                element.Variance = GenericVariance.Covariant;
            else if (tp.VarianceKeyword.IsKind(SyntaxKind.InKeyword))
                element.Variance = GenericVariance.Contravariant;

            return element;
        }

        private GenericConstraintElement ParseGenericConstraint(TypeParameterConstraintClauseSyntax constraintClause)
        {
            var element = new GenericConstraintElement(constraintClause.Name.ToString());

            foreach (var constraint in constraintClause.Constraints)
            {
                switch (constraint)
                {
                    case ClassOrStructConstraintSyntax classOrStruct:
                        if (classOrStruct.ClassOrStructKeyword.IsKind(SyntaxKind.ClassKeyword))
                            element.ConstraintKind |= GenericConstraintKind.Class;
                        else
                            element.ConstraintKind |= GenericConstraintKind.Struct;
                        break;
                    case ConstructorConstraintSyntax:
                        element.ConstraintKind |= GenericConstraintKind.New;
                        break;
                    case TypeConstraintSyntax typeConstraint:
                        var typeName = typeConstraint.Type.ToString();
                        if (typeName == "notnull")
                            element.ConstraintKind |= GenericConstraintKind.NotNull;
                        else if (typeName == "unmanaged")
                            element.ConstraintKind |= GenericConstraintKind.Unmanaged;
                        else
                            element.ConstraintTypes.Add(ParseTypeReference(typeConstraint.Type));
                        break;
                    case DefaultConstraintSyntax:
                        element.ConstraintKind |= GenericConstraintKind.Default;
                        break;
                }
            }

            return element;
        }

        #endregion

        #region Type Reference

        private TypeReference ParseTypeReference(TypeSyntax typeSyntax)
        {
            switch (typeSyntax)
            {
                case NullableTypeSyntax nullable:
                    var inner = ParseTypeReference(nullable.ElementType);
                    inner.IsNullable = true;
                    return inner;

                case ArrayTypeSyntax array:
                    var arrayInner = ParseTypeReference(array.ElementType);
                    arrayInner.IsArray = true;
                    if (array.RankSpecifiers.Count > 0)
                        arrayInner.ArrayRank = array.RankSpecifiers[0].Rank;
                    return arrayInner;

                case GenericNameSyntax generic:
                    var genericRef = new TypeReference(generic.Identifier.Text);
                    foreach (var arg in generic.TypeArgumentList.Arguments)
                        genericRef.GenericArguments.Add(ParseTypeReference(arg));
                    return genericRef;

                case QualifiedNameSyntax qualified:
                    var qualRef = ParseTypeReference(qualified.Right);
                    qualRef.Namespace = qualified.Left.ToString();
                    return qualRef;

                case PredefinedTypeSyntax predefined:
                    return new TypeReference(predefined.Keyword.Text);

                case IdentifierNameSyntax identifier:
                    return new TypeReference(identifier.Identifier.Text);

                case TupleTypeSyntax tuple:
                    return new TypeReference(tuple.ToString());

                default:
                    return new TypeReference(typeSyntax.ToString());
            }
        }

        #endregion

        #region Statements

        private void ParseStatements(CompositeStatement target, SyntaxList<StatementSyntax> statements)
        {
            foreach (var stmt in statements)
                ParseStatement(target, stmt);
        }

        private void ParseStatement(CompositeStatement target, StatementSyntax stmt)
        {
            switch (stmt)
            {
                case ReturnStatementSyntax returnStmt:
                    target.Statements.Add(new ReturnStatementElement(returnStmt.Expression?.ToString()));
                    break;

                case ThrowStatementSyntax throwStmt:
                    target.Statements.Add(new ThrowStatementElement(throwStmt.Expression?.ToString()));
                    break;

                case ExpressionStatementSyntax exprStmt:
                    if (exprStmt.Expression is AssignmentExpressionSyntax assignment &&
                        assignment.OperatorToken.IsKind(SyntaxKind.EqualsToken))
                    {
                        target.Statements.Add(new AssignmentStatement(
                            assignment.Left.ToString(),
                            assignment.Right.ToString()));
                    }
                    else
                    {
                        target.Statements.Add(new RawStatementElement(exprStmt.ToString().TrimEnd()));
                    }
                    break;

                case IfStatementSyntax ifStmt:
                    ParseIfStatement(target, ifStmt);
                    break;

                case ForStatementSyntax forStmt:
                    var forElement = new ForStatementElement();
                    forElement.Name = "For";
                    if (forStmt.Declaration != null)
                        forElement.Initializer = forStmt.Declaration.ToString();
                    else if (forStmt.Initializers.Count > 0)
                        forElement.Initializer = string.Join(", ", forStmt.Initializers.Select(i => i.ToString()));
                    forElement.Condition = forStmt.Condition?.ToString();
                    forElement.Incrementer = string.Join(", ", forStmt.Incrementors.Select(i => i.ToString()));
                    if (forStmt.Statement is BlockSyntax forBlock)
                        ParseStatements(forElement.Body, forBlock.Statements);
                    else
                        ParseStatement(forElement.Body, forStmt.Statement);
                    target.Statements.Add(forElement);
                    break;

                case ForEachStatementSyntax forEachStmt:
                    var forEachElement = new ForEachStatementElement(
                        forEachStmt.Identifier.Text,
                        forEachStmt.Expression.ToString());
                    forEachElement.Name = "ForEach";
                    var varTypeText = forEachStmt.Type.ToString();
                    if (varTypeText != "var")
                        forEachElement.VariableType = ParseTypeReference(forEachStmt.Type);
                    if (forEachStmt.Statement is BlockSyntax forEachBlock)
                        ParseStatements(forEachElement.Body, forEachBlock.Statements);
                    else
                        ParseStatement(forEachElement.Body, forEachStmt.Statement);
                    target.Statements.Add(forEachElement);
                    break;

                case WhileStatementSyntax whileStmt:
                    var whileElement = new WhileStatementElement(whileStmt.Condition.ToString());
                    whileElement.Name = "While";
                    if (whileStmt.Statement is BlockSyntax whileBlock)
                        ParseStatements(whileElement.Body, whileBlock.Statements);
                    else
                        ParseStatement(whileElement.Body, whileStmt.Statement);
                    target.Statements.Add(whileElement);
                    break;

                case SwitchStatementSyntax switchStmt:
                    var switchElement = new SwitchStatementElement();
                    switchElement.Name = "Switch";
                    switchElement.Expression = switchStmt.Expression.ToString();
                    foreach (var section in switchStmt.Sections)
                    {
                        var isDefault = section.Labels.Any(l => l is DefaultSwitchLabelSyntax);
                        if (isDefault)
                        {
                            ParseStatements(switchElement.DefaultStatements, section.Statements);
                        }
                        else
                        {
                            var switchCase = new SwitchCase();
                            foreach (var label in section.Labels)
                            {
                                if (label is CaseSwitchLabelSyntax caseLabel)
                                    switchCase.Labels.Add(caseLabel.Value.ToString());
                                else if (label is CasePatternSwitchLabelSyntax patternLabel)
                                {
                                    switchCase.Pattern = patternLabel.Pattern.ToString();
                                    if (patternLabel.WhenClause != null)
                                        switchCase.WhenClause = patternLabel.WhenClause.Condition.ToString();
                                }
                            }
                            ParseStatements(switchCase.Statements, section.Statements);
                            switchElement.Cases.Add(switchCase);
                        }
                    }
                    target.Statements.Add(switchElement);
                    break;

                case TryStatementSyntax tryStmt:
                    var tryCatchElement = new TryCatchStatementElement();
                    tryCatchElement.Name = "TryCatch";
                    ParseStatements(tryCatchElement.TryStatements, tryStmt.Block.Statements);
                    foreach (var catchClause in tryStmt.Catches)
                    {
                        var catchBlock = new CatchBlock();
                        if (catchClause.Declaration != null)
                        {
                            catchBlock.ExceptionType = ParseTypeReference(catchClause.Declaration.Type);
                            if (!string.IsNullOrEmpty(catchClause.Declaration.Identifier.Text))
                                catchBlock.ExceptionVariable = catchClause.Declaration.Identifier.Text;
                        }
                        if (catchClause.Filter != null)
                            catchBlock.WhenFilter = catchClause.Filter.FilterExpression.ToString();
                        ParseStatements(catchBlock.Statements, catchClause.Block.Statements);
                        tryCatchElement.CatchBlocks.Add(catchBlock);
                    }
                    if (tryStmt.Finally != null)
                        ParseStatements(tryCatchElement.FinallyStatements, tryStmt.Finally.Block.Statements);
                    target.Statements.Add(tryCatchElement);
                    break;

                case UsingStatementSyntax usingStmt:
                    var usingElement = new UsingStatementElement();
                    usingElement.Name = "Using";
                    if (usingStmt.Declaration != null)
                        usingElement.Resource = usingStmt.Declaration.ToString();
                    else if (usingStmt.Expression != null)
                        usingElement.Resource = usingStmt.Expression.ToString();
                    if (usingStmt.Statement is BlockSyntax usingBlock)
                        ParseStatements(usingElement.Body, usingBlock.Statements);
                    else if (usingStmt.Statement != null)
                        ParseStatement(usingElement.Body, usingStmt.Statement);
                    target.Statements.Add(usingElement);
                    break;

                case LocalDeclarationStatementSyntax localDecl:
                    if (localDecl.UsingKeyword != default)
                    {
                        var usingDeclElement = new UsingStatementElement();
                        usingDeclElement.Name = "Using";
                        usingDeclElement.IsDeclaration = true;
                        usingDeclElement.Resource = localDecl.Declaration.ToString();
                        target.Statements.Add(usingDeclElement);
                    }
                    else
                    {
                        target.Statements.Add(new RawStatementElement(localDecl.ToString().TrimEnd()));
                    }
                    break;

                case BlockSyntax block:
                    var compositeStatement = new CompositeStatement();
                    ParseStatements(compositeStatement, block.Statements);
                    target.Statements.Add(compositeStatement);
                    break;

                default:
                    target.Statements.Add(new RawStatementElement(stmt.ToString().TrimEnd()));
                    break;
            }
        }

        private void ParseIfStatement(CompositeStatement target, IfStatementSyntax ifStmt)
        {
            var ifElement = new IfStatementElement(ifStmt.Condition.ToString());
            ifElement.Name = "If";

            // Then block
            if (ifStmt.Statement is BlockSyntax thenBlock)
                ParseStatements(ifElement.ThenStatements, thenBlock.Statements);
            else
                ParseStatement(ifElement.ThenStatements, ifStmt.Statement);

            // Else / else-if chain
            var currentElse = ifStmt.Else;
            while (currentElse != null)
            {
                if (currentElse.Statement is IfStatementSyntax elseIfStmt)
                {
                    var branch = new ElseIfBranch();
                    branch.Condition = elseIfStmt.Condition.ToString();
                    if (elseIfStmt.Statement is BlockSyntax elseIfBlock)
                        ParseStatements(branch.Statements, elseIfBlock.Statements);
                    else
                        ParseStatement(branch.Statements, elseIfStmt.Statement);
                    ifElement.ElseIfBranches.Add(branch);
                    currentElse = elseIfStmt.Else;
                }
                else
                {
                    if (currentElse.Statement is BlockSyntax elseBlock)
                        ParseStatements(ifElement.ElseStatements, elseBlock.Statements);
                    else
                        ParseStatement(ifElement.ElseStatements, currentElse.Statement);
                    currentElse = null;
                }
            }

            target.Statements.Add(ifElement);
        }

        #endregion

        #region Helpers

        private void ParseCodeElementBase(CodeElement element, SyntaxNode node)
        {
            element.Documentation = ExtractDocumentation(node);

            // Attributes
            if (node is MemberDeclarationSyntax memberDecl)
                ParseAttributesFromSyntax(element, memberDecl.AttributeLists);
        }

        private void ParseAttributesFromSyntax(CodeElement element, SyntaxList<AttributeListSyntax> attributeLists)
        {
            foreach (var attrList in attributeLists)
            {
                var target = attrList.Target?.Identifier.Text.ToLower() ?? null;
                foreach (var attr in attrList.Attributes)
                {
                    var attrElement = ParseAttribute(attr);
                    if (target != null)
                    {
                        attrElement.Target = target switch
                        {
                            "assembly" => AttributeTarget.Assembly,
                            "module" => AttributeTarget.Module,
                            "type" => AttributeTarget.Type,
                            "method" => AttributeTarget.Method,
                            "property" => AttributeTarget.Property,
                            "field" => AttributeTarget.Field,
                            "event" => AttributeTarget.Event,
                            "param" => AttributeTarget.Param,
                            "return" => AttributeTarget.Return,
                            "typevar" => AttributeTarget.TypeVar,
                            _ => AttributeTarget.Default
                        };
                    }
                    element.Attributes.Add(attrElement);
                }
            }
        }

        private void ParseAccessAndModifiers(CodeElement element, SyntaxTokenList modifiers)
        {
            element.AccessModifier = ParseAccessModifierFromTokens(modifiers);
            element.Modifiers = ParseElementModifiers(modifiers);
        }

        private AccessModifier ParseAccessModifierFromTokens(SyntaxTokenList modifiers)
        {
            bool hasPublic = modifiers.Any(SyntaxKind.PublicKeyword);
            bool hasPrivate = modifiers.Any(SyntaxKind.PrivateKeyword);
            bool hasProtected = modifiers.Any(SyntaxKind.ProtectedKeyword);
            bool hasInternal = modifiers.Any(SyntaxKind.InternalKeyword);

            if (hasPublic) return AccessModifier.Public;
            if (hasPrivate && hasProtected) return AccessModifier.PrivateProtected;
            if (hasProtected && hasInternal) return AccessModifier.ProtectedInternal;
            if (hasPrivate) return AccessModifier.Private;
            if (hasProtected) return AccessModifier.Protected;
            if (hasInternal) return AccessModifier.Internal;
            return AccessModifier.Public;
        }

        private ElementModifiers ParseElementModifiers(SyntaxTokenList modifiers)
        {
            var result = ElementModifiers.None;

            foreach (var mod in modifiers)
            {
                result |= mod.Kind() switch
                {
                    SyntaxKind.StaticKeyword => ElementModifiers.Static,
                    SyntaxKind.AbstractKeyword => ElementModifiers.Abstract,
                    SyntaxKind.VirtualKeyword => ElementModifiers.Virtual,
                    SyntaxKind.OverrideKeyword => ElementModifiers.Override,
                    SyntaxKind.SealedKeyword => ElementModifiers.Sealed,
                    SyntaxKind.ReadOnlyKeyword => ElementModifiers.Readonly,
                    SyntaxKind.ConstKeyword => ElementModifiers.Const,
                    SyntaxKind.AsyncKeyword => ElementModifiers.Async,
                    SyntaxKind.PartialKeyword => ElementModifiers.Partial,
                    SyntaxKind.ExternKeyword => ElementModifiers.Extern,
                    SyntaxKind.VolatileKeyword => ElementModifiers.Volatile,
                    SyntaxKind.NewKeyword => ElementModifiers.New,
                    SyntaxKind.RequiredKeyword => ElementModifiers.Required,
                    _ => ElementModifiers.None
                };
            }

            return result;
        }

        private string? ExtractDocumentation(SyntaxNode node)
        {
            var trivia = node.GetLeadingTrivia();
            var docTrivia = trivia.FirstOrDefault(t =>
                t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
                t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));

            if (docTrivia == default) return null;

            var structure = docTrivia.GetStructure();
            if (structure is DocumentationCommentTriviaSyntax docComment)
            {
                // Extract text from <summary> element
                var summaryElement = docComment.Content
                    .OfType<XmlElementSyntax>()
                    .FirstOrDefault(e => e.StartTag.Name.ToString() == "summary");

                if (summaryElement != null)
                {
                    var text = new StringBuilder();
                    foreach (var content in summaryElement.Content)
                    {
                        if (content is XmlTextSyntax xmlText)
                        {
                            foreach (var token in xmlText.TextTokens)
                            {
                                if (!token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                                    text.Append(token.Text);
                            }
                        }
                    }
                    var result = text.ToString().Trim();
                    return string.IsNullOrEmpty(result) ? null : result;
                }
            }

            return null;
        }

        #endregion
    }
}
