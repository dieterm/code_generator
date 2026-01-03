using Microsoft.Extensions.DependencyInjection;
using ProjectXYZ.Shared.ViewModels;
using ProjectXYZ.UserControls.ViewModels;
using ProjectXYZ.UserControls.Views;

namespace ProjectXYZ.UserControls;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedUserControls(this IServiceCollection services)
    {
        services.AddKeyedTransient<UserControl, BooleanField>(nameof(BooleanField));
        services.AddKeyedTransient<UserControl, ComboboxField>(nameof(ComboboxField));
        services.AddKeyedTransient<UserControl, DateOnlyField>(nameof(DateOnlyField));
        services.AddKeyedTransient<UserControl, DenominationField>(nameof(DenominationField));
        services.AddKeyedTransient<UserControl, FileField>(nameof(FileField));
        services.AddKeyedTransient<UserControl, FolderField>(nameof(FolderField));
        services.AddKeyedTransient<UserControl, IntegerField>(nameof(IntegerField));
        services.AddKeyedTransient<UserControl, SingleLineTextField>(nameof(SingleLineTextField));

        services.AddTransient<IFieldViewModel, BooleanFieldModel>();
        services.AddTransient<IFieldViewModel, ComboboxFieldModel>();
        services.AddTransient<IFieldViewModel, DateOnlyFieldModel>();
        services.AddTransient<IFieldViewModel, DenominationFieldModel>();
        services.AddTransient<IFieldViewModel, FileFieldModel>();
        services.AddTransient<IFieldViewModel, FolderFieldModel>();
        services.AddTransient<IFieldViewModel, IntegerFieldModel>();
        services.AddTransient<IFieldViewModel, SingleLineTextFieldModel>();

        return services;
    }
}
