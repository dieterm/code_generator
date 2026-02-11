using CodeGenerator.Application.Controllers.CodeElements;
using CodeGenerator.Application.Controllers.Copilot;
using CodeGenerator.Core.CodeElements.Controllers;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Core.CodeElements.ViewModels.Statements;
using CodeGenerator.Core.CodeElements.Views;
using CodeGenerator.Core.CodeElements.Views.Statements;
using CodeGenerator.Shared.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.CodeElements
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Code Elements services in the DI container
        /// </summary>
        public static IServiceCollection AddCodeElementsServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            //configuration.GetSection("CodeElements").Bind(CodeElementsSettings.Instance);
            services.AddSingleton<CodeElementsController>();
            services.AddSingleton<ICodeElementsController>((sp) => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<CodeElementsController>(sp));
            services.AddSingleton<CodeElementsTreeViewController>();
            
            services.AddTransient<IView<CodeElementsTreeViewModel>, CodeElementsTreeView>();

            // Artifact Controllers
            services.AddSingleton<ICodeElementArtifactController, CodeFileElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, NamespacesUsingsContainerArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, CodeFileUsingsContainerArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, UsingElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, NamespacesContainerArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, NamespaceElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, TypesContainerArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, ClassElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, InterfaceElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, StructElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, EnumElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, EnumMembersContainerArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, EnumMemberElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, DelegateElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, ConstructorsContainerArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, ConstructorElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, MethodsContainerArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, MethodElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, PropertiesContainerArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, PropertyElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, FieldsContainerArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, FieldElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, EventsContainerArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, EventElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, ParametersContainerArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, ParameterElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, AttributesContainerArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, AttributeElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, IndexersContainerArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, IndexerElementArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, OperatorsContainerArtifactController>();
            services.AddSingleton<ICodeElementArtifactController, OperatorElementArtifactController>();

            services.AddTransient<IView<CodeElementArtifactDetailsViewModel>, CodeElementArtifactDetailsView>();
            
            // ViewModels
            services.AddSingleton<CodeElementArtifactDetailsViewModel>();
            services.AddSingleton<CodeElementsEditorViewModel>();
            services.AddSingleton<CodeElementsTreeViewModel>();

            // Code Element Edit Views
            services.AddTransient<IView<CodeFileElementEditViewModel>, CodeFileElementEditView>();
            services.AddTransient<IView<ClassElementEditViewModel>, ClassElementEditView>();
            services.AddTransient<IView<DelegateElementEditViewModel>, DelegateElementEditView>();
            services.AddTransient<IView<EnumElementEditViewModel>, EnumElementEditView>();
            services.AddTransient<IView<InterfaceElementEditViewModel>, InterfaceElementEditView>();
            services.AddTransient<IView<NamespaceElementEditViewModel>, NamespaceElementEditView>();
            services.AddTransient<IView<StructElementEditViewModel>, StructElementEditView>();
            services.AddTransient<IView<UsingElementEditViewModel>, UsingElementEditView>();
            services.AddTransient<IView<AttributeElementEditViewModel>, AttributeElementEditView>();
            services.AddTransient<IView<OperatorElementEditViewModel>, OperatorElementEditView>();
            services.AddTransient<IView<ParameterElementEditViewModel>, ParameterElementEditView>();
            services.AddTransient<IView<IndexerElementEditViewModel>, IndexerElementEditView>();
            services.AddTransient<IView<EventElementEditViewModel>, EventElementEditView>();
            services.AddTransient<IView<MethodElementEditViewModel>, MethodElementEditView>();
            services.AddTransient<IView<ConstructorElementEditViewModel>, ConstructorElementEditView>();
            services.AddTransient<IView<PropertyElementEditViewModel>, PropertyElementEditView>();
            services.AddTransient<IView<FieldElementEditViewModel>, FieldElementEditView>();

            // Statement Edit Views
            services.AddTransient<IView<RawStatementEditViewModel>, RawStatementEditView>();
            services.AddTransient<IView<CommentStatementEditViewModel>, CommentStatementEditView>();
            services.AddTransient<IView<CompositeStatementEditViewModel>, CompositeStatementEditView>();
            services.AddTransient<IView<AssignmentStatementEditViewModel>, AssignmentStatementEditView>();
            services.AddTransient<IView<CatchBlockEditViewModel>, CatchBlockEditView>();
            services.AddTransient<IView<ElseIfBranchEditViewModel>, ElseIfBranchEditView>();
            services.AddTransient<IView<ForEachStatementEditViewModel>, ForEachStatementEditView>();
            services.AddTransient<IView<ForStatementEditViewModel>, ForStatementEditView>();
            services.AddTransient<IView<IfStatementEditViewModel>, IfStatementEditView>();
            services.AddTransient<IView<ReturnStatementEditViewModel>, ReturnStatementEditView>();
            services.AddTransient<IView<SwitchCaseEditViewModel>, SwitchCaseEditView>();
            services.AddTransient<IView<SwitchStatementEditViewModel>, SwitchStatementEditView>();
            services.AddTransient<IView<ThrowStatementEditViewModel>, ThrowStatementEditView>();
            services.AddTransient<IView<TryCatchStatementEditViewModel>, TryCatchStatementEditView>();
            services.AddTransient<IView<UsingStatementEditViewModel>, UsingStatementEditView>();
            services.AddTransient<IView<WhileStatementEditViewModel>, WhileStatementEditView>();

            return services;
        }
    }
}
