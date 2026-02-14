using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Workspaces.Generators;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Domain.Mermaid;
using CodeGenerator.Generators.DotNet.Events;
using Markdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace CodeGenerator.Generators.DotNet.Generators
{
    public class ProjectStructureDocumentationGenerator : GeneratorBase, IProgress<ArtifactGenerationProgress>
    {
        public const string GENERATOR_ID = "DotNet.ProjectStructureDocumentation";
        private Action<DotNetSolutionArtifactGeneratedEventArgs>? _dotNetSolutionGeneratedSubscriptionToken;

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            _dotNetSolutionGeneratedSubscriptionToken = messageBus.Subscribe<DotNetSolutionArtifactGeneratedEventArgs>(OnDotNetSolutionGenerated);
        }

        //private async Task<FolderArtifact> EnsureDocsFolderExists(GenerationResult result)
        //{
        //    return CreateFolder("docs", result);
        //    var docsFolder = result.RootArtifact.Children.OfType<FolderArtifact>().FirstOrDefault(f => f.FolderName.Equals("docs", StringComparison.OrdinalIgnoreCase));
        //    if (docsFolder == null)
        //    {
        //        docsFolder = new FolderArtifact("docs");
        //        AddChildArtifactToParent(result.RootArtifact, docsFolder, result);
        //        // todo: create arifact in an earlier stage of the generation process so it is generated normally
        //        await docsFolder.GenerateAsync(this, CancellationToken.None);
        //    }
            
        //    return docsFolder;
        //}

        private async void OnDotNetSolutionGenerated(DotNetSolutionArtifactGeneratedEventArgs args)
        {
            var docsFolderArtifact = CreateFolder("docs", args.Result.RootArtifact, args.Result);

            // see https://github.com/charlesdevandiere/markdown-builder-dotnet
            IMarkdownDocument doc = new MarkdownDocument();
            doc.AppendHeader("Project Structure Documentation", 1);
            doc.AppendParagraph("This document provides an overview of the project structure.");

            // Build Mermaid dependency graph from DotNetProjectReferences
            var projectReferences = args.Result.DotNetProjectReferences;
            var mermaidDiagram = BuildDependencyGraph(projectReferences);

            doc.AppendHeader("Dependency Graph", 2);
            doc.AppendParagraph("The following diagram shows the project dependencies:");
            doc.AppendCode("mermaid", mermaidDiagram);

            var readmeContent = doc.ToString();

            var projectStructureArtifact = new FileArtifact("ProjectStructure.md");
            projectStructureArtifact.SetTextContent(readmeContent);
            AddChildArtifactToParent(docsFolderArtifact, projectStructureArtifact, args.Result);
            // todo: create arifact in an earlier stage of the generation process so it is generated normally
            //await readmeArtifact.GenerateAsync(this, CancellationToken.None);
            var docsFolder = args.Solution.SolutionItems.CreateSubFolder("docs");
            docsFolder.Items.Add(new Domain.DotNet.ProjectType.DotNetSolutionItem(projectStructureArtifact.FullPath));
        }

        private string BuildDependencyGraph(ScopeDotNetProjectReferencesCollection projectReferences)
        {
            var mermaid = new MermaidGraphBuilder("LR");

            // Collect all unique projects and their references across all scopes
            var allProjects = new Dictionary<string, DotNetProjectArtifact>();
            CollectProjects(projectReferences, allProjects);

            // Add subgraphs per scope and nodes per project
            foreach (var scopeRef in projectReferences)
            {
                var scopeId = SanitizeId(scopeRef.ScopeArtifact.Name);

                if (scopeRef.DotNetProjectArtifacts.Count > 0 || scopeRef.SubScopeReferences.Count > 0)
                {
                    mermaid.SubgraphStart(scopeId, scopeRef.ScopeArtifact.Name);

                    foreach (var (layerName, project) in scopeRef.DotNetProjectArtifacts)
                    {
                        var nodeId = SanitizeId(project.Name);
                        mermaid.Node(nodeId, project.Name);
                    }

                    // Nested sub-scopes
                    AddSubScopeNodes(mermaid, scopeRef.SubScopeReferences);

                    mermaid.SubgraphEnd();
                }
            }

            // Add all dependency links
            foreach (var project in allProjects.Values)
            {
                var fromId = SanitizeId(project.Name);
                foreach (var reference in project.ProjectReferences)
                {
                    var toId = SanitizeId(reference.ProjectArtifact.Name);
                    mermaid.Link(fromId, toId);
                }
            }

            return mermaid.ToString();
        }

        private void AddSubScopeNodes(MermaidGraphBuilder mermaid, List<ScopeDotNetProjectReferences> subScopes)
        {
            foreach (var subScope in subScopes)
            {
                var subScopeId = SanitizeId(subScope.ScopeArtifact.Name);

                if (subScope.DotNetProjectArtifacts.Count > 0 || subScope.SubScopeReferences.Count > 0)
                {
                    mermaid.SubgraphStart(subScopeId, subScope.ScopeArtifact.Name);

                    foreach (var (layerName, project) in subScope.DotNetProjectArtifacts)
                    {
                        var nodeId = SanitizeId(project.Name);
                        mermaid.Node(nodeId, project.Name);
                    }

                    AddSubScopeNodes(mermaid, subScope.SubScopeReferences);

                    mermaid.SubgraphEnd();
                }
            }
        }

        private void CollectProjects(ScopeDotNetProjectReferencesCollection collection, Dictionary<string, DotNetProjectArtifact> allProjects)
        {
            foreach (var scopeRef in collection)
            {
                CollectProjectsFromScope(scopeRef, allProjects);
            }
        }

        private void CollectProjectsFromScope(ScopeDotNetProjectReferences scopeRef, Dictionary<string, DotNetProjectArtifact> allProjects)
        {
            foreach (var (_, project) in scopeRef.DotNetProjectArtifacts)
            {
                allProjects.TryAdd(project.Name, project);
            }
            foreach (var subScope in scopeRef.SubScopeReferences)
            {
                CollectProjectsFromScope(subScope, allProjects);
            }
        }

        private static string SanitizeId(string name)
        {
            return name.Replace(".", "_").Replace(" ", "_").Replace("-", "_");
        }

        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if (_dotNetSolutionGeneratedSubscriptionToken != null)
            {
                messageBus.Unsubscribe(_dotNetSolutionGeneratedSubscriptionToken);
                _dotNetSolutionGeneratedSubscriptionToken = null;
            }
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            return new GeneratorSettingsDescription(GENERATOR_ID, "Project Structure Documentation", "Generates documentation for the project structure.", "Documentation", "Documentation");
        }

        public void Report(ArtifactGenerationProgress value)
        {
            //throw new NotImplementedException();
        }
    }
}
