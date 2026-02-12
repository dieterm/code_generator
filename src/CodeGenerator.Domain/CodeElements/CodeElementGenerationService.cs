using CodeGenerator.Domain.CodeElements.Statements;
using CodeGenerator.Domain.ProgrammingLanguages.CSharp;
using System.Text;

namespace CodeGenerator.Domain.CodeElements
{
    public class CodeElementGenerationService
    {
        private readonly StringBuilder Output = new StringBuilder();
        private int _indent = 0;

        private CodeElementGenerationService() { }

        public static string GenerateCode(CodeFileElement codeElement)
        {
            var service = new CodeElementGenerationService();
            service.GenerateCodeFileElement(codeElement);
            return service.Output.ToString();
        }

        private void AppendLine(string line = "")
        {
            if (string.IsNullOrEmpty(line))
                Output.AppendLine();
            else
                Output.AppendLine($"{new string('\t', _indent)}{line}");
        }

        private string Escape(string? value) => value?.Replace("\\", "\\\\").Replace("\"", "\\\"") ?? "";

        private string Quote(string? value) => $"\"{Escape(value)}\"";

        private void GenerateCodeFileElement(CodeFileElement codeFile)
        {
            AppendLine($"var codeFileElement = new CodeFileElement({Quote(codeFile.FileName)}, {GetLanguageExpression(codeFile.Language)});");

            if (!string.IsNullOrEmpty(codeFile.FileHeader))
                AppendLine($"codeFileElement.FileHeader = {Quote(codeFile.FileHeader)};");
            if (codeFile.NullableContext.HasValue)
                AppendLine($"codeFileElement.NullableContext = {BoolLiteral(codeFile.NullableContext.Value)};");
            if (codeFile.UseImplicitUsings)
                AppendLine($"codeFileElement.UseImplicitUsings = true;");
            if (codeFile.FileExtension != ".cs")
                AppendLine($"codeFileElement.FileExtension = {Quote(codeFile.FileExtension)};");

            GenerateCodeElementBaseProperties("codeFileElement", codeFile);

            if (codeFile.Usings.Any())
            {
                AppendLine($"codeFileElement.Usings = new List<UsingElement>");
                AppendLine("{");
                _indent++;
                foreach (var u in codeFile.Usings)
                    GenerateUsingElementInline(u);
                _indent--;
                AppendLine("};");
            }

            if (codeFile.GlobalUsings.Any())
            {
                AppendLine($"codeFileElement.GlobalUsings = new List<UsingElement>");
                AppendLine("{");
                _indent++;
                foreach (var u in codeFile.GlobalUsings)
                    GenerateUsingElementInline(u);
                _indent--;
                AppendLine("};");
            }

            if (codeFile.FileAttributes.Any())
                GenerateAttributeList("codeFileElement.FileAttributes", codeFile.FileAttributes);

            if (codeFile.TopLevelStatements.Any())
            {
                AppendLine($"codeFileElement.TopLevelStatements = new List<string>");
                AppendLine("{");
                _indent++;
                foreach (var s in codeFile.TopLevelStatements)
                    AppendLine($"{Quote(s)},");
                _indent--;
                AppendLine("};");
            }

            foreach (var ns in codeFile.Namespaces)
            {
                AppendLine();
                var nsVar = GetVariableName("ns", ns.FullName);
                GenerateNamespaceElement(nsVar, ns);
                AppendLine($"codeFileElement.Namespaces.Add({nsVar});");
            }

            foreach (var type in codeFile.TopLevelTypes)
            {
                AppendLine();
                var typeVar = GenerateTypeElement(type);
                AppendLine($"codeFileElement.TopLevelTypes.Add({typeVar});");
            }
        }

        private void GenerateUsingElementInline(UsingElement u)
        {
            var properties = new List<string> { $"Namespace = {Quote(u.Namespace)}" };
            if (!string.IsNullOrEmpty(u.Alias)) properties.Add($"Alias = {Quote(u.Alias)}");
            if (u.IsStatic) properties.Add("IsStatic = true");
            if (u.IsGlobal) properties.Add("IsGlobal = true");
            if (u.Name != null && u.Name != u.Namespace) properties.Add($"Name = {Quote(u.Name)}");
            AddCodeElementBasePropertiesToList(properties, u);
            AppendLine($"new UsingElement {{ {string.Join(", ", properties)} }},");
        }

        private void GenerateNamespaceElement(string varName, NamespaceElement ns)
        {
            AppendLine($"var {varName} = new NamespaceElement({Quote(ns.FullName)});");
            if (!ns.IsFileScoped)
                AppendLine($"{varName}.IsFileScoped = false;");

            GenerateCodeElementBaseProperties(varName, ns);

            if (ns.Usings.Any())
            {
                AppendLine($"{varName}.Usings = new List<UsingElement>");
                AppendLine("{");
                _indent++;
                foreach (var u in ns.Usings)
                    GenerateUsingElementInline(u);
                _indent--;
                AppendLine("};");
            }

            foreach (var nestedNs in ns.NestedNamespaces)
            {
                var nestedVar = GetVariableName("nestedNs", nestedNs.FullName);
                GenerateNamespaceElement(nestedVar, nestedNs);
                AppendLine($"{varName}.NestedNamespaces.Add({nestedVar});");
            }

            foreach (var type in ns.Types)
            {
                AppendLine();
                var typeVar = GenerateTypeElement(type);
                AppendLine($"{varName}.Types.Add({typeVar});");
            }
        }

        private string GenerateTypeElement(TypeElement type)
        {
            return type switch
            {
                ClassElement c => GenerateClassElement(c),
                InterfaceElement i => GenerateInterfaceElement(i),
                StructElement s => GenerateStructElement(s),
                EnumElement e => GenerateEnumElement(e),
                DelegateElement d => GenerateDelegateElement(d),
                _ => throw new NotSupportedException($"Unsupported type: {type.GetType().Name}")
            };
        }

        private string GenerateClassElement(ClassElement cls)
        {
            var varName = GetVariableName("cls", cls.Name);
            AppendLine($"var {varName} = new ClassElement({Quote(cls.Name)});");

            if (cls.IsRecord) AppendLine($"{varName}.IsRecord = true;");

            GenerateTypeElementBaseProperties(varName, cls);

            foreach (var f in cls.Fields)
            {
                var fVar = GenerateFieldElement(f);
                AppendLine($"{varName}.Fields.Add({fVar});");
            }
            foreach (var p in cls.Properties)
            {
                var pVar = GeneratePropertyElement(p);
                AppendLine($"{varName}.Properties.Add({pVar});");
            }
            foreach (var c in cls.Constructors)
            {
                var cVar = GenerateConstructorElement(c);
                AppendLine($"{varName}.Constructors.Add({cVar});");
            }
            foreach (var m in cls.Methods)
            {
                var mVar = GenerateMethodElement(m);
                AppendLine($"{varName}.Methods.Add({mVar});");
            }
            foreach (var e in cls.Events)
            {
                var eVar = GenerateEventElement(e);
                AppendLine($"{varName}.Events.Add({eVar});");
            }
            foreach (var idx in cls.Indexers)
            {
                var iVar = GenerateIndexerElement(idx);
                AppendLine($"{varName}.Indexers.Add({iVar});");
            }
            foreach (var op in cls.Operators)
            {
                var oVar = GenerateOperatorElement(op);
                AppendLine($"{varName}.Operators.Add({oVar});");
            }
            if (cls.Finalizer != null)
                GenerateFinalizerElement(varName, cls.Finalizer);
            foreach (var param in cls.PrimaryConstructorParameters)
            {
                var pVar = GenerateParameterElement(param);
                AppendLine($"{varName}.PrimaryConstructorParameters.Add({pVar});");
            }

            return varName;
        }

        private string GenerateInterfaceElement(InterfaceElement iface)
        {
            var varName = GetVariableName("iface", iface.Name);
            AppendLine($"var {varName} = new InterfaceElement({Quote(iface.Name)});");

            GenerateTypeElementBaseProperties(varName, iface);

            foreach (var p in iface.Properties)
            {
                var pVar = GeneratePropertyElement(p);
                AppendLine($"{varName}.Properties.Add({pVar});");
            }
            foreach (var m in iface.Methods)
            {
                var mVar = GenerateMethodElement(m);
                AppendLine($"{varName}.Methods.Add({mVar});");
            }
            foreach (var e in iface.Events)
            {
                var eVar = GenerateEventElement(e);
                AppendLine($"{varName}.Events.Add({eVar});");
            }
            foreach (var idx in iface.Indexers)
            {
                var iVar = GenerateIndexerElement(idx);
                AppendLine($"{varName}.Indexers.Add({iVar});");
            }

            return varName;
        }

        private string GenerateStructElement(StructElement str)
        {
            var varName = GetVariableName("str", str.Name);
            AppendLine($"var {varName} = new StructElement({Quote(str.Name)});");

            if (str.IsRecord) AppendLine($"{varName}.IsRecord = true;");
            if (str.IsReadonly) AppendLine($"{varName}.IsReadonly = true;");
            if (str.IsRef) AppendLine($"{varName}.IsRef = true;");

            GenerateTypeElementBaseProperties(varName, str);

            foreach (var f in str.Fields)
            {
                var fVar = GenerateFieldElement(f);
                AppendLine($"{varName}.Fields.Add({fVar});");
            }
            foreach (var p in str.Properties)
            {
                var pVar = GeneratePropertyElement(p);
                AppendLine($"{varName}.Properties.Add({pVar});");
            }
            foreach (var c in str.Constructors)
            {
                var cVar = GenerateConstructorElement(c);
                AppendLine($"{varName}.Constructors.Add({cVar});");
            }
            foreach (var m in str.Methods)
            {
                var mVar = GenerateMethodElement(m);
                AppendLine($"{varName}.Methods.Add({mVar});");
            }
            foreach (var e in str.Events)
            {
                var eVar = GenerateEventElement(e);
                AppendLine($"{varName}.Events.Add({eVar});");
            }
            foreach (var param in str.PrimaryConstructorParameters)
            {
                var pVar = GenerateParameterElement(param);
                AppendLine($"{varName}.PrimaryConstructorParameters.Add({pVar});");
            }

            return varName;
        }

        private string GenerateEnumElement(EnumElement enm)
        {
            var varName = GetVariableName("enm", enm.Name);
            AppendLine($"var {varName} = new EnumElement({Quote(enm.Name)});");

            if (enm.IsFlags) AppendLine($"{varName}.IsFlags = true;");
            if (enm.UnderlyingType != null)
                AppendLine($"{varName}.UnderlyingType = {GenerateTypeReferenceInline(enm.UnderlyingType)};");

            GenerateTypeElementBaseProperties(varName, enm);

            foreach (var member in enm.Members)
            {
                var props = new List<string> { $"Name = {Quote(member.Name)}" };
                if (member.Value != null) props.Add($"Value = {Quote(member.Value.ToString())}");
                AddCodeElementBasePropertiesToList(props, member);
                AppendLine($"{varName}.Members.Add(new EnumMemberElement {{ {string.Join(", ", props)} }});");
            }

            return varName;
        }

        private string GenerateDelegateElement(DelegateElement del)
        {
            var varName = GetVariableName("del", del.Name);
            AppendLine($"var {varName} = new DelegateElement({Quote(del.Name)}, {GenerateTypeReferenceInline(del.ReturnType)});");

            GenerateTypeElementBaseProperties(varName, del);

            foreach (var p in del.Parameters)
            {
                var pVar = GenerateParameterElement(p);
                AppendLine($"{varName}.Parameters.Add({pVar});");
            }

            return varName;
        }

        private string GenerateFieldElement(FieldElement field)
        {
            var varName = GetVariableName("field", field.Name);
            AppendLine($"var {varName} = new FieldElement({Quote(field.Name)}, {GenerateTypeReferenceInline(field.Type)});");

            if (!string.IsNullOrEmpty(field.InitialValue))
                AppendLine($"{varName}.InitialValue = {Quote(field.InitialValue)};");

            GenerateCodeElementBaseProperties(varName, field, defaultAccess: AccessModifier.Private);
            return varName;
        }

        private string GeneratePropertyElement(PropertyElement prop)
        {
            var varName = GetVariableName("prop", prop.Name);
            AppendLine($"var {varName} = new PropertyElement({Quote(prop.Name)}, {GenerateTypeReferenceInline(prop.Type)});");

            if (!prop.HasGetter) AppendLine($"{varName}.HasGetter = false;");
            if (!prop.HasSetter) AppendLine($"{varName}.HasSetter = false;");
            if (prop.GetterAccessModifier.HasValue) AppendLine($"{varName}.GetterAccessModifier = AccessModifier.{prop.GetterAccessModifier.Value};");
            if (prop.SetterAccessModifier.HasValue) AppendLine($"{varName}.SetterAccessModifier = AccessModifier.{prop.SetterAccessModifier.Value};");
            if (prop.IsInitOnly) AppendLine($"{varName}.IsInitOnly = true;");
            if (!prop.IsAutoImplemented) AppendLine($"{varName}.IsAutoImplemented = false;");
            if (prop.IsExpressionBodied) AppendLine($"{varName}.IsExpressionBodied = true;");
            if (!string.IsNullOrEmpty(prop.ExpressionBody)) AppendLine($"{varName}.ExpressionBody = {Quote(prop.ExpressionBody)};");
            if (!string.IsNullOrEmpty(prop.InitialValue)) AppendLine($"{varName}.InitialValue = {Quote(prop.InitialValue)};");

            if (prop.GetterBody.HasStatements)
                GenerateCompositeStatementBody($"{varName}.GetterBody", prop.GetterBody);
            if (prop.SetterBody.HasStatements)
                GenerateCompositeStatementBody($"{varName}.SetterBody", prop.SetterBody);

            GenerateCodeElementBaseProperties(varName, prop);
            return varName;
        }

        private string GenerateMethodElement(MethodElement method)
        {
            var varName = GetVariableName("method", method.Name);
            AppendLine($"var {varName} = new MethodElement({Quote(method.Name)}, {GenerateTypeReferenceInline(method.ReturnType)});");

            if (method.IsExpressionBodied) AppendLine($"{varName}.IsExpressionBodied = true;");
            if (!string.IsNullOrEmpty(method.ExpressionBody)) AppendLine($"{varName}.ExpressionBody = {Quote(method.ExpressionBody)};");
            if (method.IsExtensionMethod) AppendLine($"{varName}.IsExtensionMethod = true;");

            GenerateCodeElementBaseProperties(varName, method);

            foreach (var p in method.Parameters)
            {
                var pVar = GenerateParameterElement(p);
                AppendLine($"{varName}.Parameters.Add({pVar});");
            }

            GenerateGenericTypeParameters(varName, method.GenericTypeParameters);
            GenerateGenericConstraints(varName, method.GenericConstraints);

            if (method.Body.HasStatements)
                GenerateCompositeStatementBody($"{varName}.Body", method.Body);

            foreach (var lf in method.LocalFunctions)
            {
                var lfVar = GenerateMethodElement(lf);
                AppendLine($"{varName}.LocalFunctions.Add({lfVar});");
            }

            return varName;
        }

        private string GenerateConstructorElement(ConstructorElement ctor)
        {
            var varName = GetVariableName("ctor", null);
            AppendLine($"var {varName} = new ConstructorElement();");

            if (ctor.IsPrimary) AppendLine($"{varName}.IsPrimary = true;");
            if (ctor.IsStatic) AppendLine($"{varName}.IsStatic = true;");

            if (ctor.BaseCall != null)
                AppendLine($"{varName}.BaseCall = new ConstructorInitializer {{ Arguments = new List<string> {{ {string.Join(", ", ctor.BaseCall.Arguments.Select(Quote))} }} }};");
            if (ctor.ThisCall != null)
                AppendLine($"{varName}.ThisCall = new ConstructorInitializer {{ Arguments = new List<string> {{ {string.Join(", ", ctor.ThisCall.Arguments.Select(Quote))} }} }};");

            GenerateCodeElementBaseProperties(varName, ctor);

            foreach (var p in ctor.Parameters)
            {
                var pVar = GenerateParameterElement(p);
                AppendLine($"{varName}.Parameters.Add({pVar});");
            }

            if (ctor.Body.HasStatements)
                GenerateCompositeStatementBody($"{varName}.Body", ctor.Body);

            return varName;
        }

        private string GenerateParameterElement(ParameterElement param)
        {
            var varName = GetVariableName("param", param.Name);
            AppendLine($"var {varName} = new ParameterElement({Quote(param.Name)}, {GenerateTypeReferenceInline(param.Type)});");

            if (param.Modifier != ParameterModifier.None) AppendLine($"{varName}.Modifier = ParameterModifier.{param.Modifier};");
            if (!string.IsNullOrEmpty(param.DefaultValue)) AppendLine($"{varName}.DefaultValue = {Quote(param.DefaultValue)};");
            if (param.IsExtensionMethodThis) AppendLine($"{varName}.IsExtensionMethodThis = true;");

            GenerateCodeElementBaseProperties(varName, param);
            return varName;
        }

        private string GenerateEventElement(EventElement evt)
        {
            var varName = GetVariableName("evt", evt.Name);
            AppendLine($"var {varName} = new EventElement({Quote(evt.Name)}, {GenerateTypeReferenceInline(evt.Type)});");

            if (!evt.IsFieldLike) AppendLine($"{varName}.IsFieldLike = false;");
            if (!string.IsNullOrEmpty(evt.AddAccessorBody)) AppendLine($"{varName}.AddAccessorBody = {Quote(evt.AddAccessorBody)};");
            if (!string.IsNullOrEmpty(evt.RemoveAccessorBody)) AppendLine($"{varName}.RemoveAccessorBody = {Quote(evt.RemoveAccessorBody)};");

            GenerateCodeElementBaseProperties(varName, evt);
            return varName;
        }

        private string GenerateIndexerElement(IndexerElement indexer)
        {
            var varName = GetVariableName("indexer", null);
            AppendLine($"var {varName} = new IndexerElement();");
            AppendLine($"{varName}.Type = {GenerateTypeReferenceInline(indexer.Type)};");

            if (!indexer.HasGetter) AppendLine($"{varName}.HasGetter = false;");
            if (!indexer.HasSetter) AppendLine($"{varName}.HasSetter = false;");
            if (indexer.GetterAccessModifier.HasValue) AppendLine($"{varName}.GetterAccessModifier = AccessModifier.{indexer.GetterAccessModifier.Value};");
            if (indexer.SetterAccessModifier.HasValue) AppendLine($"{varName}.SetterAccessModifier = AccessModifier.{indexer.SetterAccessModifier.Value};");

            GenerateCodeElementBaseProperties(varName, indexer);

            foreach (var p in indexer.Parameters)
            {
                var pVar = GenerateParameterElement(p);
                AppendLine($"{varName}.Parameters.Add({pVar});");
            }

            if (indexer.GetterBody.HasStatements)
                GenerateCompositeStatementBody($"{varName}.GetterBody", indexer.GetterBody);
            if (indexer.SetterBody.HasStatements)
                GenerateCompositeStatementBody($"{varName}.SetterBody", indexer.SetterBody);

            return varName;
        }

        private string GenerateOperatorElement(OperatorElement op)
        {
            var varName = GetVariableName("op", null);
            AppendLine($"var {varName} = new OperatorElement(OperatorType.{op.OperatorType}, {GenerateTypeReferenceInline(op.ReturnType)});");

            if (op.IsImplicit) AppendLine($"{varName}.IsImplicit = true;");

            GenerateCodeElementBaseProperties(varName, op);

            foreach (var p in op.Parameters)
            {
                var pVar = GenerateParameterElement(p);
                AppendLine($"{varName}.Parameters.Add({pVar});");
            }

            if (op.Body.HasStatements)
                GenerateCompositeStatementBody($"{varName}.Body", op.Body);

            return varName;
        }

        private void GenerateFinalizerElement(string parentVar, FinalizerElement finalizer)
        {
            var props = new List<string>();
            if (!string.IsNullOrEmpty(finalizer.Body)) props.Add($"Body = {Quote(finalizer.Body)}");
            AddCodeElementBasePropertiesToList(props, finalizer);
            if (props.Any())
                AppendLine($"{parentVar}.Finalizer = new FinalizerElement {{ {string.Join(", ", props)} }};");
            else
                AppendLine($"{parentVar}.Finalizer = new FinalizerElement();");
        }

        // --- Statements ---

        private void GenerateCompositeStatementBody(string targetExpr, CompositeStatement composite)
        {
            foreach (var statement in composite.Statements)
                GenerateStatement(targetExpr, statement);
        }

        private void GenerateStatement(string parentExpr, StatementElement statement)
        {
            switch (statement)
            {
                case AssignmentStatement assign:
                    AppendLine($"{parentExpr}.Statements.Add(new AssignmentStatement({Quote(assign.Left)}, {Quote(assign.Right)}));");
                    break;

                case CommentStatement comment:
                    AppendLine($"{parentExpr}.Statements.Add(new CommentStatement({Quote(comment.Text)}));");
                    break;

                case RawStatementElement raw:
                    AppendLine($"{parentExpr}.Statements.Add(new RawStatementElement({Quote(raw.Code)}));");
                    break;

                case ReturnStatementElement ret:
                    if (!string.IsNullOrEmpty(ret.Expression))
                        AppendLine($"{parentExpr}.Statements.Add(new ReturnStatementElement({Quote(ret.Expression)}));");
                    else
                        AppendLine($"{parentExpr}.Statements.Add(new ReturnStatementElement());");
                    break;

                case ThrowStatementElement thr:
                    if (!string.IsNullOrEmpty(thr.Expression))
                        AppendLine($"{parentExpr}.Statements.Add(new ThrowStatementElement({Quote(thr.Expression)}));");
                    else
                        AppendLine($"{parentExpr}.Statements.Add(new ThrowStatementElement());");
                    break;

                case IfStatementElement ifStmt:
                    GenerateIfStatement(parentExpr, ifStmt);
                    break;

                case ForStatementElement forStmt:
                    GenerateForStatement(parentExpr, forStmt);
                    break;

                case ForEachStatementElement forEach:
                    GenerateForEachStatement(parentExpr, forEach);
                    break;

                case WhileStatementElement whileStmt:
                    GenerateWhileStatement(parentExpr, whileStmt);
                    break;

                case SwitchStatementElement switchStmt:
                    GenerateSwitchStatement(parentExpr, switchStmt);
                    break;

                case TryCatchStatementElement tryCatch:
                    GenerateTryCatchStatement(parentExpr, tryCatch);
                    break;

                case UsingStatementElement usingStmt:
                    GenerateUsingStatement(parentExpr, usingStmt);
                    break;

                case CompositeStatement comp:
                    GenerateCompositeStatement(parentExpr, comp);
                    break;

                default:
                    throw new NotSupportedException($"Unsupported statement type: {statement.GetType().Name}");
            }
        }

        private void GenerateIfStatement(string parentExpr, IfStatementElement ifStmt)
        {
            var varName = GetVariableName("ifStmt", null);
            AppendLine($"var {varName} = new IfStatementElement({Quote(ifStmt.Condition)});");
            GenerateStatementBaseProperties(varName, ifStmt);

            if (ifStmt.ThenStatements.HasStatements)
                GenerateCompositeStatementBody($"{varName}.ThenStatements", ifStmt.ThenStatements);

            if (ifStmt.ElseStatements.HasStatements)
                GenerateCompositeStatementBody($"{varName}.ElseStatements", ifStmt.ElseStatements);

            foreach (var branch in ifStmt.ElseIfBranches)
            {
                var branchVar = GetVariableName("elseIf", null);
                AppendLine($"var {branchVar} = new ElseIfBranch {{ Condition = {Quote(branch.Condition)} }};");
                GenerateStatementBaseProperties(branchVar, branch);
                if (branch.Statements.HasStatements)
                    GenerateCompositeStatementBody($"{branchVar}.Statements", branch.Statements);
                AppendLine($"{varName}.ElseIfBranches.Add({branchVar});");
            }

            AppendLine($"{parentExpr}.Statements.Add({varName});");
        }

        private void GenerateForStatement(string parentExpr, ForStatementElement forStmt)
        {
            var varName = GetVariableName("forStmt", null);
            AppendLine($"var {varName} = new ForStatementElement();");
            if (!string.IsNullOrEmpty(forStmt.Initializer)) AppendLine($"{varName}.Initializer = {Quote(forStmt.Initializer)};");
            if (!string.IsNullOrEmpty(forStmt.Condition)) AppendLine($"{varName}.Condition = {Quote(forStmt.Condition)};");
            if (!string.IsNullOrEmpty(forStmt.Incrementer)) AppendLine($"{varName}.Incrementer = {Quote(forStmt.Incrementer)};");
            GenerateStatementBaseProperties(varName, forStmt);

            if (forStmt.Body.HasStatements)
                GenerateCompositeStatementBody($"{varName}.Body", forStmt.Body);

            AppendLine($"{parentExpr}.Statements.Add({varName});");
        }

        private void GenerateForEachStatement(string parentExpr, ForEachStatementElement forEach)
        {
            var varName = GetVariableName("forEach", null);
            AppendLine($"var {varName} = new ForEachStatementElement({Quote(forEach.VariableName)}, {Quote(forEach.Collection)});");
            if (forEach.VariableType != null)
                AppendLine($"{varName}.VariableType = {GenerateTypeReferenceInline(forEach.VariableType)};");
            GenerateStatementBaseProperties(varName, forEach);

            if (forEach.Body.HasStatements)
                GenerateCompositeStatementBody($"{varName}.Body", forEach.Body);

            AppendLine($"{parentExpr}.Statements.Add({varName});");
        }

        private void GenerateWhileStatement(string parentExpr, WhileStatementElement whileStmt)
        {
            var varName = GetVariableName("whileStmt", null);
            AppendLine($"var {varName} = new WhileStatementElement({Quote(whileStmt.Condition)});");
            GenerateStatementBaseProperties(varName, whileStmt);

            if (whileStmt.Body.HasStatements)
                GenerateCompositeStatementBody($"{varName}.Body", whileStmt.Body);

            AppendLine($"{parentExpr}.Statements.Add({varName});");
        }

        private void GenerateSwitchStatement(string parentExpr, SwitchStatementElement switchStmt)
        {
            var varName = GetVariableName("switchStmt", null);
            AppendLine($"var {varName} = new SwitchStatementElement();");
            AppendLine($"{varName}.Expression = {Quote(switchStmt.Expression)};");
            GenerateStatementBaseProperties(varName, switchStmt);

            foreach (var c in switchStmt.Cases)
            {
                var caseVar = GetVariableName("switchCase", null);
                AppendLine($"var {caseVar} = new SwitchCase();");
                if (c.Labels.Any())
                    AppendLine($"{caseVar}.Labels = new List<string> {{ {string.Join(", ", c.Labels.Select(Quote))} }};");
                if (!string.IsNullOrEmpty(c.Pattern)) AppendLine($"{caseVar}.Pattern = {Quote(c.Pattern)};");
                if (!string.IsNullOrEmpty(c.WhenClause)) AppendLine($"{caseVar}.WhenClause = {Quote(c.WhenClause)};");
                GenerateStatementBaseProperties(caseVar, c);
                if (c.Statements.HasStatements)
                    GenerateCompositeStatementBody($"{caseVar}.Statements", c.Statements);
                AppendLine($"{varName}.Cases.Add({caseVar});");
            }

            if (switchStmt.DefaultStatements.HasStatements)
                GenerateCompositeStatementBody($"{varName}.DefaultStatements", switchStmt.DefaultStatements);

            AppendLine($"{parentExpr}.Statements.Add({varName});");
        }

        private void GenerateTryCatchStatement(string parentExpr, TryCatchStatementElement tryCatch)
        {
            var varName = GetVariableName("tryCatch", null);
            AppendLine($"var {varName} = new TryCatchStatementElement();");
            GenerateStatementBaseProperties(varName, tryCatch);

            if (tryCatch.TryStatements.HasStatements)
                GenerateCompositeStatementBody($"{varName}.TryStatements", tryCatch.TryStatements);

            foreach (var cb in tryCatch.CatchBlocks)
            {
                var cbVar = GetVariableName("catchBlock", null);
                AppendLine($"var {cbVar} = new CatchBlock();");
                if (cb.ExceptionType != null) AppendLine($"{cbVar}.ExceptionType = {GenerateTypeReferenceInline(cb.ExceptionType)};");
                if (!string.IsNullOrEmpty(cb.ExceptionVariable)) AppendLine($"{cbVar}.ExceptionVariable = {Quote(cb.ExceptionVariable)};");
                if (!string.IsNullOrEmpty(cb.WhenFilter)) AppendLine($"{cbVar}.WhenFilter = {Quote(cb.WhenFilter)};");
                GenerateStatementBaseProperties(cbVar, cb);
                if (cb.Statements.HasStatements)
                    GenerateCompositeStatementBody($"{cbVar}.Statements", cb.Statements);
                AppendLine($"{varName}.CatchBlocks.Add({cbVar});");
            }

            if (tryCatch.FinallyStatements.HasStatements)
                GenerateCompositeStatementBody($"{varName}.FinallyStatements", tryCatch.FinallyStatements);

            AppendLine($"{parentExpr}.Statements.Add({varName});");
        }

        private void GenerateUsingStatement(string parentExpr, UsingStatementElement usingStmt)
        {
            var varName = GetVariableName("usingStmt", null);
            AppendLine($"var {varName} = new UsingStatementElement();");
            AppendLine($"{varName}.Resource = {Quote(usingStmt.Resource)};");
            if (usingStmt.IsDeclaration) AppendLine($"{varName}.IsDeclaration = true;");
            GenerateStatementBaseProperties(varName, usingStmt);

            if (usingStmt.Body.HasStatements)
                GenerateCompositeStatementBody($"{varName}.Body", usingStmt.Body);

            AppendLine($"{parentExpr}.Statements.Add({varName});");
        }

        private void GenerateCompositeStatement(string parentExpr, CompositeStatement comp)
        {
            var varName = GetVariableName("block", null);
            AppendLine($"var {varName} = new CompositeStatement();");
            GenerateStatementBaseProperties(varName, comp);

            if (comp.HasStatements)
                GenerateCompositeStatementBody(varName, comp);

            AppendLine($"{parentExpr}.Statements.Add({varName});");
        }

        // --- Shared helpers ---

        private void GenerateTypeElementBaseProperties(string varName, TypeElement type)
        {
            GenerateCodeElementBaseProperties(varName, type);
            GenerateGenericTypeParameters(varName, type.GenericTypeParameters);
            GenerateGenericConstraints(varName, type.GenericConstraints);

            if (type.BaseTypes.Any())
            {
                AppendLine($"{varName}.BaseTypes = new List<TypeReference>");
                AppendLine("{");
                _indent++;
                foreach (var bt in type.BaseTypes)
                    AppendLine($"{GenerateTypeReferenceInline(bt)},");
                _indent--;
                AppendLine("};");
            }

            foreach (var nested in type.NestedTypes)
            {
                var nestedVar = GenerateTypeElement(nested);
                AppendLine($"{varName}.NestedTypes.Add({nestedVar});");
            }
        }

        private void GenerateGenericTypeParameters(string varName, List<GenericTypeParameterElement> parameters)
        {
            foreach (var gtp in parameters)
            {
                var props = new List<string> { $"Name = {Quote(gtp.Name)}" };
                if (gtp.Variance != GenericVariance.Invariant)
                    props.Add($"Variance = GenericVariance.{gtp.Variance}");
                AddCodeElementBasePropertiesToList(props, gtp);
                AppendLine($"{varName}.GenericTypeParameters.Add(new GenericTypeParameterElement {{ {string.Join(", ", props)} }});");
            }
        }

        private void GenerateGenericConstraints(string varName, List<GenericConstraintElement> constraints)
        {
            foreach (var gc in constraints)
            {
                var gcVar = GetVariableName("constraint", gc.TypeParameterName);
                AppendLine($"var {gcVar} = new GenericConstraintElement({Quote(gc.TypeParameterName)});");
                if (gc.ConstraintKind != GenericConstraintKind.None)
                    AppendLine($"{gcVar}.ConstraintKind = (GenericConstraintKind){(int)gc.ConstraintKind};");
                if (gc.ConstraintTypes.Any())
                {
                    AppendLine($"{gcVar}.ConstraintTypes = new List<TypeReference>");
                    AppendLine("{");
                    _indent++;
                    foreach (var ct in gc.ConstraintTypes)
                        AppendLine($"{GenerateTypeReferenceInline(ct)},");
                    _indent--;
                    AppendLine("};");
                }
                AppendLine($"{varName}.GenericConstraints.Add({gcVar});");
            }
        }

        private void GenerateCodeElementBaseProperties(string varName, CodeElement element, AccessModifier defaultAccess = AccessModifier.Public)
        {
            if (element.AccessModifier != defaultAccess)
                AppendLine($"{varName}.AccessModifier = AccessModifier.{element.AccessModifier};");
            if (element.Modifiers != ElementModifiers.None)
                AppendLine($"{varName}.Modifiers = (ElementModifiers){(int)element.Modifiers};");
            if (!string.IsNullOrEmpty(element.Documentation))
                AppendLine($"{varName}.Documentation = {Quote(element.Documentation)};");
            if (!string.IsNullOrEmpty(element.RawCode))
                AppendLine($"{varName}.RawCode = {Quote(element.RawCode)};");

            if (element.Attributes.Any())
                GenerateAttributeList($"{varName}.Attributes", element.Attributes);
        }

        private void AddCodeElementBasePropertiesToList(List<string> properties, CodeElement element)
        {
            if (element.AccessModifier != AccessModifier.Public)
                properties.Add($"AccessModifier = AccessModifier.{element.AccessModifier}");
            if (element.Modifiers != ElementModifiers.None)
                properties.Add($"Modifiers = (ElementModifiers){(int)element.Modifiers}");
            if (!string.IsNullOrEmpty(element.Documentation))
                properties.Add($"Documentation = {Quote(element.Documentation)}");
            if (!string.IsNullOrEmpty(element.RawCode))
                properties.Add($"RawCode = {Quote(element.RawCode)}");
        }

        private void GenerateStatementBaseProperties(string varName, StatementElement statement)
        {
            if (statement.Name != null)
                AppendLine($"{varName}.Name = {Quote(statement.Name)};");
            if (statement.AccessModifier != AccessModifier.Public)
                AppendLine($"{varName}.AccessModifier = AccessModifier.{statement.AccessModifier};");
            if (statement.Modifiers != ElementModifiers.None)
                AppendLine($"{varName}.Modifiers = (ElementModifiers){(int)statement.Modifiers};");
            if (!string.IsNullOrEmpty(statement.Documentation))
                AppendLine($"{varName}.Documentation = {Quote(statement.Documentation)};");
            if (!string.IsNullOrEmpty(statement.RawCode))
                AppendLine($"{varName}.RawCode = {Quote(statement.RawCode)};");
            if (statement.Attributes.Any())
                GenerateAttributeList($"{varName}.Attributes", statement.Attributes);
        }

        private void GenerateAttributeList(string targetExpr, List<AttributeElement> attributes)
        {
            AppendLine($"{targetExpr} = new List<AttributeElement>");
            AppendLine("{");
            _indent++;
            foreach (var attr in attributes)
            {
                var props = new List<string> { $"AttributeName = {Quote(attr.AttributeName)}" };
                if (attr.Target != AttributeTarget.Default)
                    props.Add($"Target = AttributeTarget.{attr.Target}");
                if (attr.Arguments.Any())
                    props.Add($"Arguments = new List<string> {{ {string.Join(", ", attr.Arguments.Select(Quote))} }}");
                if (attr.NamedArguments.Any())
                    props.Add($"NamedArguments = new Dictionary<string, string> {{ {string.Join(", ", attr.NamedArguments.Select(kv => $"{{ {Quote(kv.Key)}, {Quote(kv.Value)} }}"))} }}");
                AddCodeElementBasePropertiesToList(props, attr);
                AppendLine($"new AttributeElement {{ {string.Join(", ", props)} }},");
            }
            _indent--;
            AppendLine("};");
        }

        private string GenerateTypeReferenceInline(TypeReference typeRef)
        {
            var props = new List<string>();

            if (!string.IsNullOrEmpty(typeRef.TypeName))
                props.Add($"TypeName = {Quote(typeRef.TypeName)}");
            if (typeRef.IsNullable) props.Add("IsNullable = true");
            if (typeRef.IsArray) props.Add("IsArray = true");
            if (typeRef.IsArray && typeRef.ArrayRank != 1) props.Add($"ArrayRank = {typeRef.ArrayRank}");
            if (!string.IsNullOrEmpty(typeRef.Namespace)) props.Add($"Namespace = {Quote(typeRef.Namespace)}");
            if (typeRef.GenericArguments.Any())
                props.Add($"GenericArguments = new List<TypeReference> {{ {string.Join(", ", typeRef.GenericArguments.Select(GenerateTypeReferenceInline))} }}");

            return $"new TypeReference {{ {string.Join(", ", props)} }}";
        }

        private string GetLanguageExpression(ProgrammingLanguages.ProgrammingLanguage language)
        {
            if (language is CSharpLanguage)
                return "CSharpLanguage.Instance";
            return $"/* TODO: unsupported language {language.Name} */ CSharpLanguage.Instance";
        }

        private static string BoolLiteral(bool value) => value ? "true" : "false";

        private int _varCounter = 0;
        private string GetVariableName(string prefix, string? name)
        {
            _varCounter++;
            if (!string.IsNullOrEmpty(name))
            {
                var sanitized = new string(name.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray());
                if (sanitized.Length > 0 && char.IsDigit(sanitized[0]))
                    sanitized = "_" + sanitized;
                if (!string.IsNullOrEmpty(sanitized))
                    return $"{prefix}_{sanitized}_{_varCounter}";
            }
            return $"{prefix}_{_varCounter}";
        }
    }
}
