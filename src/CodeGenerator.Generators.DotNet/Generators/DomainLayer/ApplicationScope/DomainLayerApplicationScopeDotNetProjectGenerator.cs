using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.DotNet.Events;
using CodeGenerator.Generators.DotNet.Generators.ApplicationLayer.SharedScope;
using CodeGenerator.TemplateEngines.DotNetProject;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Generators.DomainLayer.ApplicationScope
{
    public class DomainLayerApplicationScopeDotNetProjectGenerator : DotNetProjectGenerator<DomainLayerArtifact>
    {
        //private DotNetProjectArtifact? _domainLayerApplicationScopeProjectArtifact = null;
        //private DotNetProjectArtifact? _domainLayerSharedScopeProjectArtifact = null;
        public DomainLayerApplicationScopeDotNetProjectGenerator(ILogger<DomainLayerApplicationScopeDotNetProjectGenerator> logger)
            : base(CodeArchitectureLayerArtifact.DOMAIN_LAYER, CodeArchitectureLayerArtifact.APPLICATION_SCOPE, logger)
        {
        }

        //protected override void OnCreatingRootArtifact(CreatingArtifactEventArgs e)
        //{
        //    // reset cached project artifacts
        //    _domainLayerApplicationScopeProjectArtifact = null;
        //    _domainLayerSharedScopeProjectArtifact = null;
        //}

        //protected override void OnDotNetProjectCreated(DotNetProjectArtifactCreatedEventArgs e)
        //{
        //    if (e.Layer == CodeArchitectureLayerArtifact.DOMAIN_LAYER && e.Scope == CodeArchitectureLayerArtifact.APPLICATION_SCOPE)
        //    {
        //        // store our own project artifact
        //        _domainLayerApplicationScopeProjectArtifact = e.DotNetProjectArtifact;
        //    }
        //    else if (e.Layer == CodeArchitectureLayerArtifact.DOMAIN_LAYER && e.Scope == CodeArchitectureLayerArtifact.SHARED_SCOPE)
        //    {
        //        // store shared scope project artifact
        //        _domainLayerSharedScopeProjectArtifact = e.DotNetProjectArtifact;
        //    }

        //    // AFTER both projects are created, add the project reference
        //    if (_domainLayerApplicationScopeProjectArtifact != null && _domainLayerSharedScopeProjectArtifact != null)
        //    {
        //        // add project reference from application scope to shared scope
        //        _domainLayerApplicationScopeProjectArtifact.ProjectReferences.Add(new DotNetProjectReference(_domainLayerSharedScopeProjectArtifact));
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
