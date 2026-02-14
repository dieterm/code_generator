using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Domain.DotNet;
using OfficeIMO.Markdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.CodeArchitectureLayers
{
    public class ReadmeDocumentationGenerator : GeneratorBase
    {
        public const string GENERATOR_ID = "CodeArchitectureLayers.ReadmeDocumentationGenerator";
        private Action<RootArtifactCreatedEventArgs>? _unsubscribeHandler;

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            _unsubscribeHandler = messageBus.Subscribe<RootArtifactCreatedEventArgs>(OnCreatingRootArtifact);
            messageBus.Subscribe<CreatedArtifactEventArgs>(OnDotNetSolutionArtifactCreated, (args) => args.Artifact is DotNetSolutionArtifact);
        }

        private void OnDotNetSolutionArtifactCreated(CreatedArtifactEventArgs args)
        {
            var dotnetSolutionArtifact = args.Artifact as DotNetSolutionArtifact;
            var outputFolder = args.Result.Workspace.OutputDirectory;
            var readmePath = Path.Combine(outputFolder, "README.md");
            dotnetSolutionArtifact.Solution.SolutionItems.Items.Add(new Domain.DotNet.ProjectType.DotNetSolutionItem(readmePath));
        }

        private void OnCreatingRootArtifact(RootArtifactCreatedEventArgs args)
        {
            var docsFolder = CreateFolder("docs", args.RootArtifact, args.Result);

            var md = MarkdownDoc
                .Create()
                .FrontMatter(new { title = args.Result.Workspace.Name, tags = args.Result.Workspace.GetAllScopes(true, true).Select(scope => scope.Name).ToArray() })
                .H1(args.Result.Workspace.Name)
                .P(args.Result.Workspace.Documentation)
                .Callout("info", "Early access", "APIs may change before 1.0.")
                .H2("Install")
                .Code("bash", "dotnet tool install -g DomainDetective")
                .H2("Quick start")
                .Code("powershell",
                    "Test-DDMailDomainClassification -DomainName 'evotec.pl','evotec.xyz' -ExportFormat Word")
                .H2("Features")
                .Ul(ul => ul
                    .Item($"Language: {args.Result.Workspace.DefaultLanguage}")
                    .Item($"Code Architecture: {args.Result.Workspace.CodeArchitecture?.Name ?? "N/A"}")
                    .Item($"Dependency Injection Framework: {args.Result.Workspace.DependencyInjectionFramework?.Name ?? "N/A"}")
                    )
                .H2("Scopes")
                .Ul(ul => GenerateScopeItems(ul, args.Result.Workspace.Scopes));
                    //.ItemLink("Docs", "https://evotec.xyz/hub/")
                    //.ItemLink("Issues", "https://github.com/EvotecIT/DomainDetective/issues"));
            //foreach(var scope in args.Result.Workspace.GetAllScopes(true, true))
            //{
            //        md.H3(scope.Name)
            //            .P(scope.Documentation);
            //}
            var markdown = md.ToMarkdown();
            //var htmlFrag = md.ToHtmlFragment();
            //var htmlDoc = md.ToHtmlDocument();
            var readmeArtifact = new FileArtifact("README.md");
            readmeArtifact.SetTextContent(markdown);
            AddChildArtifactToParent(args.RootArtifact, readmeArtifact, args.Result);
        }

        private void GenerateScopeItems(UnorderedListBuilder ul, IEnumerable<ScopeArtifact> scopes)
        {
            foreach(var scope in scopes)
            {
                ul.ItemLink(scope.Name, $"#{scope.Name.ToLower().Replace(" ", "-")}");
                if (scope.SubScopes.Any())
                {
                    var sublist = new UnorderedListBuilder();
                    GenerateScopeItems(sublist, scope.SubScopes);
                    ul.Item(sublist.ToString()!);
                }
            }
        }

        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if (_unsubscribeHandler != null) { 
                messageBus.Unsubscribe(_unsubscribeHandler);
                _unsubscribeHandler = null;
            }
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            return new GeneratorSettingsDescription(GENERATOR_ID, "README Documentation Generator", "Documentation");
        }
    }
}
