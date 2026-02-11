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
        //services.AddKeyedTransient<UserControl, BooleanField>(nameof(BooleanField));
        //services.AddKeyedTransient<UserControl, ComboboxField>(nameof(ComboboxField));
        //services.AddKeyedTransient<UserControl, DateOnlyField>(nameof(DateOnlyField));
        //services.AddKeyedTransient<UserControl, DenominationField>(nameof(DenominationField));
        //services.AddKeyedTransient<UserControl, FileField>(nameof(FileField));
        //services.AddKeyedTransient<UserControl, FolderField>(nameof(FolderField));
        //services.AddKeyedTransient<UserControl, IntegerField>(nameof(IntegerField));
        //services.AddKeyedTransient<UserControl, SingleLineTextField>(nameof(SingleLineTextField));
        //services.AddKeyedTransient<UserControl, ParameterizedStringField>(nameof(ParameterizedStringField));
        //services.AddKeyedTransient<UserControl, StringListField>(nameof(StringListField));
        //services.AddKeyedTransient<UserControl, StringDictionaryField>(nameof(StringDictionaryField));

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
        return services;
    }
}
