using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using CodeGenerator.Core.Workspaces.Operations.Domains;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.CodeElements.Statements;
using CodeGenerator.Domain.ProgrammingLanguages.CSharp;
using CodeGenerator.Generators.DotNet.Repositories.Csv.Workspace.Artifacts;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Operations;
using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Repositories.Csv.Workspace.Subscribers
{
    public class TableArtifactContextMenuOpeningSubscriber : WorkspaceArtifactContextMenuOpeningSubscriber<TableArtifact>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;
        public TableArtifactContextMenuOpeningSubscriber(IWorkspaceContextProvider workspaceContextProvider) 
        {
            _workspaceContextProvider = workspaceContextProvider;
        }
        protected override void HandleArtifactContextMenuOpening(ArtifactContextMenuOpeningEventArgs args, TableArtifact artifact)
        {
            var createObjectCommand = new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "create_object",
                Text = "Create object",
                IconKey = "script",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };

            var newRepositoryCommand = new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "new_repository",
                Text = "New Repository",
                IconKey = "script",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };
            createObjectCommand.SubCommands.Add(newRepositoryCommand);

            var newCsvRepositoryCommand = new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "new_csv_repository",
                Text = "New CSV Repository",
                IconKey = "script",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };
            newRepositoryCommand.SubCommands.Add(newCsvRepositoryCommand);

            var domains = _workspaceContextProvider.CurrentWorkspace!.GetAllScopes(false, true).SelectMany(s => s.Domains).ToList();
            foreach (var domain in domains)
            {
                var domainCommand = new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                {
                    Id = $"new_entity_{domain.Id}",
                    Text = domain.Name,
                    IconKey = "domain",
                    Execute = async (a) =>
                    {
                        CreateCsvRepositoryFromTableInDomain(artifact, domain);
                        await Task.CompletedTask;
                    }
                };
                newCsvRepositoryCommand.SubCommands.Add(domainCommand);
            }

            args.Commands.Add(createObjectCommand);
        }

        private void CreateCsvRepositoryFromTableInDomain(TableArtifact artifact, DomainArtifact domain)
        {
            // 1. add CsvRepositoryBase to Repositories container in the shared scope infrastructure layer
            // CsvRepositoryBase should be an abstract class inheriting from RepositoryBase or implementing IRepository<T> interface
            // CsvRepositoryBase should be implementing basic csv handling (using CsvHelper or similar library) and providing abstract methods for mapping between CsvRow and the entity,
            // as well as for getting the file path for the csv file (which can be based on the table name)
            // In the generator also make sure the required nuget-packages are added to the dotnetproject artifact (CsvHelper)
            var targetScope = domain.Scope;
            var servicesNamespace = $"{targetScope.Infrastructure.Context.Namespace}.Services";
            var infrastructureLayer = targetScope.Infrastructure.Services.AddChild(new CsvValueObjectReaderArtifact(CreateCsvValueObjectReader(servicesNamespace)));

            // 2. add CsvRow class to ValueTypes in domain
            var className = $"{artifact.Name}CsvRow";
            var valueType = domain.AddValueType(new Core.Workspaces.Artifacts.Domains.ValueTypes.ValueTypeArtifact(className));

            foreach (var column in artifact.GetColumns())
            {
                valueType.AddProperty(column.ToPropertyArtifact());
            }

            // 3. add CsvValueObjectReaderImplementation class to Services in Infrastructure-layer, inheriting from CsvValueObjectReader and using the '<Table>CsvRow' class
            targetScope.Infrastructure.Services.AddChild(new CsvValueObjectReaderImplementationArtifact(CreateCsvValueObjectReaderImplementation(servicesNamespace, className, artifact.Name), valueType.Id));
        }

        private CodeFileElement CreateCsvValueObjectReaderImplementation(string @namespace, string valueTypeclassName, string tableName)
        {
            var className = $"{tableName}Reader";
            var codeFileElement = new CodeFileElement(className, CSharpLanguage.Instance);

            var cls_CsvRowReader = codeFileElement.AddNamespace(@namespace, new ClassElement(className) {
                BaseTypes = [TypeReference.Generic(CsvValueObjectReaderClassName, new TypeReference(valueTypeclassName))]
            });

            cls_CsvRowReader.Constructors.Add(new ConstructorElement(new ParameterElement("csvFilePath", TypeReference.Common.String))
            {
                BaseCall = new ConstructorInitializer { Arguments = ["csvFilePath"] }
            });

            return codeFileElement;
        }

        public const string CsvValueObjectReaderClassName = "CsvValueObjectReader";
        private CodeFileElement CreateCsvValueObjectReader(string @namespace)
        {
            var codeFileElement = new CodeFileElement(CsvValueObjectReaderClassName, CSharpLanguage.Instance);

            codeFileElement
                .AddUsing("CsvHelper")
                .AddUsing("CsvHelper.Configuration")
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Diagnostics")
                .AddUsing("System.Globalization")
                .AddUsing("System.IO")
                .AddUsing("System.Threading");

            var typeT = new TypeReference("T");
            
            var cls_CsvRepositoryBase = codeFileElement
                .AddNamespace(@namespace, new ClassElement(CsvValueObjectReaderClassName)
                {
                    Modifiers = ElementModifiers.Abstract,
                    GenericTypeParameters = [new GenericTypeParameterElement(typeT.TypeName)],
                    GenericConstraints = [new GenericConstraintElement(typeT.TypeName) { ConstraintTypes = [new TypeReference("IValueObject")] }],
                    BaseTypes = [TypeReference.Generic("IProgress", new TypeReference("Progress"))]
                });
            
            // Properties
            cls_CsvRepositoryBase.Properties.Add(new PropertyElement("CsvFilePath", TypeReference.Common.String) { HasSetter = false });
            
            // Constructor
            var constructor = new ConstructorElement(new ParameterElement("csvFilePath", TypeReference.Common.String));
            constructor.Body.Statements.Add(new AssignmentStatement("this.CsvFilePath", "csvFilePath ?? throw new ArgumentNullException(nameof(csvFilePath))"));
            cls_CsvRepositoryBase.Constructors.Add(constructor);

            // Methods
            // Method GetAll without parameters, calling the overload with CancellationToken.None and this as progress
            var method_GetAll = new MethodElement("GetAll", TypeReference.Generic("IEnumerable", typeT));
            method_GetAll.Body.Statements.Add(new ReturnStatementElement("this.GetAll(CancellationToken.None, this)"));
            cls_CsvRepositoryBase.Methods.Add(method_GetAll);

            // Method GetAll with CancellationToken and IProgress parameters, implementing the csv reading logic with progress reporting
            var method_GetAll_2 = new MethodElement("GetAll", TypeReference.Generic("IEnumerable", typeT)) { Modifiers = (ElementModifiers)4 };
            var param_cancellationToken = new ParameterElement("cancellationToken", new TypeReference { TypeName = "CancellationToken" });
            method_GetAll_2.Parameters.Add(param_cancellationToken);
            var param_progress = new ParameterElement("progress", TypeReference.Generic("IProgress", new TypeReference("Progress")));
            method_GetAll_2.Parameters.Add(param_progress);

            var ifStmt_11 = new IfStatementElement("progress == null") { Name = "If" };
            ifStmt_11.ThenStatements.Statements.Add(new AssignmentStatement("progress", "this"));
            method_GetAll_2.Body.Statements.Add(ifStmt_11);
            method_GetAll_2.Body.Statements.Add(new RawStatementElement("Progress progressState = new Progress();"));
            method_GetAll_2.Body.Statements.Add(new RawStatementElement("progressState.Report(0.0, \"Start loading \" + this.CsvFilePath);"));
            method_GetAll_2.Body.Statements.Add(new RawStatementElement("progress?.Report(progressState);"));
            var usingStmt_12 = new UsingStatementElement();
            usingStmt_12.Resource = "StreamReader reader = new StreamReader(this.CsvFilePath)";
            usingStmt_12.Name = "Using";
            var usingStmt_13 = new UsingStatementElement();
            usingStmt_13.Resource = @"CsvReader csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            })";
            usingStmt_13.Name = "Using";
            usingStmt_13.Body.Statements.Add(new RawStatementElement("string typeName = typeof(T).Name;"));
            usingStmt_13.Body.Statements.Add(new RawStatementElement("uint recordCount = 0;"));
            usingStmt_13.Body.Statements.Add(new RawStatementElement("double previousProgress = 0.0;"));
            usingStmt_13.Body.Statements.Add(new RawStatementElement("IEnumerator<T> enumerator = csv.GetRecords<T>().GetEnumerator();"));
            var whileStmt_14 = new WhileStatementElement("enumerator.MoveNext()");
            whileStmt_14.Name = "While";
            whileStmt_14.Body.Statements.Add(new RawStatementElement("T record = enumerator.Current;"));
            whileStmt_14.Body.Statements.Add(new RawStatementElement("++recordCount;"));
            whileStmt_14.Body.Statements.Add(new AssignmentStatement("record.CsvLine", "recordCount + 1"));
            var ifStmt_15 = new IfStatementElement("!cancellationToken.IsCancellationRequested");
            ifStmt_15.Name = "If";
            ifStmt_15.ThenStatements.Statements.Add(new RawStatementElement("yield return record;"));
            var ifStmt_16 = new IfStatementElement("recordCount % 4000 == 0");
            ifStmt_16.Name = "If";
            ifStmt_16.ThenStatements.Statements.Add(new RawStatementElement("double myFloat = (double)reader.BaseStream.Position / (double)reader.BaseStream.Length;"));
            ifStmt_16.ThenStatements.Statements.Add(new RawStatementElement("double currentProgress = double.Parse(Math.Round(myFloat, 2).ToString());"));
            var ifStmt_17 = new IfStatementElement("currentProgress > previousProgress");
            ifStmt_17.Name = "If";
            ifStmt_17.ThenStatements.Statements.Add(new AssignmentStatement("previousProgress", "currentProgress"));
            ifStmt_17.ThenStatements.Statements.Add(new RawStatementElement("progressState.Report(currentProgress, $\"{recordCount} {typeName}(s)\");"));
            ifStmt_17.ThenStatements.Statements.Add(new RawStatementElement("progress?.Report(progressState);"));
            ifStmt_16.ThenStatements.Statements.Add(ifStmt_17);
            ifStmt_15.ThenStatements.Statements.Add(ifStmt_16);
            ifStmt_15.ThenStatements.Statements.Add(new AssignmentStatement("record", "default(T)"));
            ifStmt_15.ElseStatements.Statements.Add(new RawStatementElement("break;"));
            whileStmt_14.Body.Statements.Add(ifStmt_15);
            usingStmt_13.Body.Statements.Add(whileStmt_14);
            usingStmt_13.Body.Statements.Add(new RawStatementElement("progressState.Report(1.0, $\"Finished loading {this.CsvFilePath} with {recordCount} {typeName}(s)\");"));
            usingStmt_13.Body.Statements.Add(new RawStatementElement("progress?.Report(progressState);"));
            usingStmt_12.Body.Statements.Add(usingStmt_13);
            method_GetAll_2.Body.Statements.Add(usingStmt_12);
            cls_CsvRepositoryBase.Methods.Add(method_GetAll_2);

            var method_Report_18 = new MethodElement("Report", new TypeReference { TypeName = "void" });
            var param_value_19 = new ParameterElement("value", new TypeReference { TypeName = "Progress" });
            method_Report_18.Parameters.Add(param_value_19);
            method_Report_18.Body.Statements.Add(new RawStatementElement("Debug.WriteLine(value.ToString());"));
            cls_CsvRepositoryBase.Methods.Add(method_Report_18);
            //ns_KBODataKboOpenDataCsvRepositories_1.Types.Add(cls_CsvRepositoryBase_2);
            //codeFileElement.Namespaces.Add(ns_KBODataKboOpenDataCsvRepositories_1);
            return codeFileElement;
        }

    }
}
