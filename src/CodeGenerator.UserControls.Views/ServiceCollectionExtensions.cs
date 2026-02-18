using CodeGenerator.Shared.Models;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using CodeGenerator.UserControls.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.UserControls;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedUserControlViews(this IServiceCollection services)
    {
        services.AddTransient<IView<BooleanFieldModel>, BooleanField>();
        services.AddTransient<IView<CheckboxFieldModel>, CheckboxField>();
        services.AddTransient<IView<ComboboxFieldModel>, ComboboxField>();
        services.AddTransient<IView<DateOnlyFieldModel>, DateOnlyField>();
        services.AddTransient<IView<DenominationFieldModel>, DenominationField>();
        services.AddTransient<IView<FileFieldModel>, FileField>();
        services.AddTransient<IView<FolderFieldModel>, FolderField>();
        services.AddTransient<IView<IntegerFieldModel>, IntegerField>();
        services.AddTransient<IView<SingleLineTextFieldModel>, SingleLineTextField>();
        services.AddTransient<IView<ParameterizedStringFieldModel>, ParameterizedStringField>();
        services.AddTransient<IView<StringListFieldModel>, StringListField>();
        services.AddTransient<IView<StringDictionaryFieldModel>, StringDictionaryField>();
        services.AddTransient<IView<FieldCollectionModel>, FieldCollection>();
        services.AddTransient<IView<MultiSelectFieldModel>, MultiSelectField>();
        services.AddTransient<IView<MultiLineTextFieldModel>, MultiLineTextField>();
        services.AddTransient<IView<LabelFieldModel>, LabelField>();
        return services;
    }
}
