using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.ProgrammingLanguages.CSharp;
using CodeGenerator.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Workspace
{
    public class EntityArtifactSubscriber : WorkspaceArtifactContextMenuOpeningSubscriber<EntityArtifact>
    {
        protected override void HandleArtifactContextMenuOpening(ArtifactContextMenuOpeningEventArgs args, EntityArtifact artifact)
        {
            args.Commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE) { 
                Id = "generate_csharp_class",
                Text = "Generate C# Class",
                IconKey = "csharp_icon",
                Execute = async (art) => 
                {
                    await GenerateClassAsync(artifact);
                }

            });
        }

        private Task GenerateClassAsync(EntityArtifact artifact)
        {
            // use CodeGenerator.Domain.CodeElements to generate a C# class based on the entity artifact
            // use artifact.Name, artifact.States, artifact.Relations, etc. to build the class
            var workspace = artifact.GetWorkspace();
            var codeFileElement = new CodeFileElement($"{artifact.Name}.cs", ProgrammingLanguage.CSharp);
            var classElement = new Domain.CodeElements.ClassElement
            {
                Name = artifact.Name,
                AccessModifier = AccessModifier.Public,
            };
            codeFileElement.AddNamespace(workspace.RootNamespace, classElement);
            classElement.Properties.Add(new PropertyElement("Id", new TypeReference("Guid"))
            {
                AccessModifier = AccessModifier.Public,
                IsAutoImplemented = true,
                InitialValue = "Guid.NewGuid()"
            });
            // Add properties for each state
            foreach (var state in artifact.States)
            {
                classElement.Properties.Add(new PropertyElement(state.Name, new TypeReference("string"))
                {
                    AccessModifier = AccessModifier.Public,
                    IsAutoImplemented = true
                });
            }
            foreach (var relation in artifact.Relations.Where(r => r.TargetEntity != null))
            {
                var typeRef = new TypeReference(relation.TargetEntity!.Name);
                if (relation.SourceCardinality == RelationCardinality.ZeroOrMany)
                {
                    typeRef = TypeReference.Generic("List", typeRef);
                }
                classElement.Properties.Add(new PropertyElement(relation.Name, typeRef)
                {
                    AccessModifier = AccessModifier.Public,
                    IsAutoImplemented = true
                });
            }
            if (artifact.DefaultState != null) { 
                foreach (var property in artifact.DefaultState.Properties)
                {
                    var dataTypeMapping = CSharpLanguage.Instance.GetMapping(property.DataType);
                    classElement.Properties.Add(new PropertyElement(property.Name, new TypeReference(dataTypeMapping.NativeTypeName))
                    {
                        AccessModifier = AccessModifier.Public,
                        IsAutoImplemented = true
                    });
                }
            }
            classElement.Methods.Add(new MethodElement("ToString")
            {
                AccessModifier = AccessModifier.Public,
                Body = $"return $\"{artifact.Name} [Id={{Id}}]\";"
            });
            classElement.Methods.Add(new MethodElement()
            {
                RawCode = "\tpublic EntityArtifact ToEntityArtifact() \n{\n SomeCodeHere();\n }"
            });

            // Generate C# class code
            var csharpCodegenerator = new CSharpCodeGenerator();
            var csharpCode = csharpCodegenerator.GenerateCodeFile(codeFileElement);

            // Show the generated code in an artifact preview window
            var applicationMessageBus = ServiceProviderHolder.GetRequiredService<ApplicationMessageBus>();
            applicationMessageBus.ShowArtifactPreview(new ArtifactPreviewViewModel
            {
                FileName = codeFileElement.FileName,
                TextContent = csharpCode,
                TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.CSharp
            });

            return Task.CompletedTask;
        }
    }
}
