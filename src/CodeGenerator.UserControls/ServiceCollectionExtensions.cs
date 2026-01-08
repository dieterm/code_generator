using Microsoft.Extensions.DependencyInjection;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.UserControls;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedUserControls(this IServiceCollection services)
    {
        services.AddTransient<IFieldViewModel, BooleanFieldModel>();
        services.AddTransient<IFieldViewModel, ComboboxFieldModel>();
        services.AddTransient<IFieldViewModel, DateOnlyFieldModel>();
        services.AddTransient<IFieldViewModel, DenominationFieldModel>();
        services.AddTransient<IFieldViewModel, FileFieldModel>();
        services.AddTransient<IFieldViewModel, FolderFieldModel>();
        services.AddTransient<IFieldViewModel, IntegerFieldModel>();
        services.AddTransient<IFieldViewModel, SingleLineTextFieldModel>();
        services.AddTransient<IFieldViewModel, ParameterizedStringFieldModel>();

        return services;
    }
}
