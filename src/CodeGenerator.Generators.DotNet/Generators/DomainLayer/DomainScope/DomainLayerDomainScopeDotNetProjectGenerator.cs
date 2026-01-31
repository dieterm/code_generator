using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.DotNet.Events;
using CodeGenerator.Generators.DotNet.Generators.DomainLayer.ApplicationScope;
using CodeGenerator.TemplateEngines.DotNetProject;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Generators.DomainLayer.DomainScope
{
    public class DomainLayerDomainScopeDotNetProjectGenerator : DotNetProjectGenerator<DomainLayerArtifact>
    {
        //private DotNetProjectArtifact? _domainLayerDomainScopeProjectArtifact = null;
       // private DotNetProjectArtifact? _domainLayerSharedScopeProjectArtifact = null;

        public DomainLayerDomainScopeDotNetProjectGenerator(ILogger<DomainLayerDomainScopeDotNetProjectGenerator> logger)
            : base(CodeArchitectureLayerArtifact.DOMAIN_LAYER, CodeArchitectureLayerArtifact.DOMAIN_SCOPE, logger)
        {
        }

        public override bool LayerArtifactFilter(CreatedArtifactEventArgs e)
        {
            return e.Artifact is DomainLayerArtifact a && a.Scope != CodeArchitectureLayerArtifact.APPLICATION_SCOPE && a.Scope != CodeArchitectureLayerArtifact.SHARED_SCOPE;
        }

        //protected override void OnCreatingRootArtifact(CreatingArtifactEventArgs e)
        //{
        //    // reset cached project artifacts
        //    _domainLayerDomainScopeProjectArtifact = null;
        //    _domainLayerSharedScopeProjectArtifact = null;
        //}

        //protected override void OnDotNetProjectCreated(DotNetProjectArtifactCreatedEventArgs e)
        //{
        //    // LISTEN for both our own domain scope project and the shared scope project
        //    if (e.Layer == CodeArchitectureLayerArtifact.DOMAIN_LAYER && e.Scope != CodeArchitectureLayerArtifact.SHARED_SCOPE && e.Scope != CodeArchitectureLayerArtifact.APPLICATION_SCOPE)
        //    {
        //        // store our own project artifact
        //        _domainLayerDomainScopeProjectArtifact = e.DotNetProjectArtifact;
        //    }
        //    else if(e.Layer == CodeArchitectureLayerArtifact.DOMAIN_LAYER && e.Scope == CodeArchitectureLayerArtifact.SHARED_SCOPE)
        //    {
        //        // store shared scope project artifact
        //        _domainLayerSharedScopeProjectArtifact = e.DotNetProjectArtifact;
        //    }

        //    // AFTER both projects are created, add the project reference
        //    if (_domainLayerDomainScopeProjectArtifact != null && _domainLayerSharedScopeProjectArtifact != null)
        //    {
        //        // add project reference from domain scope to shared scope
        //        _domainLayerDomainScopeProjectArtifact.ProjectReferences.Add(new DotNetProjectReference(_domainLayerSharedScopeProjectArtifact));
        //    }
        //}

        protected override async Task<DotNetProjectArtifact> OnLayerScopeCreatedAsync(CreatedArtifactEventArgs args)
        {
            var dotNetProjectArtifact = await base.OnLayerScopeCreatedAsync(args);
            // Additional configuration for Application Scope projects can be added here
            return dotNetProjectArtifact;
        }
    }
}
