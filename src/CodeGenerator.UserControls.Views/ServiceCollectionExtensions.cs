using CodeGenerator.Shared.Models;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using CodeGenerator.UserControls.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.UserControls;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedUserControlViews(this IServiceCollection services)
    {
        services.AddKeyedTransient<UserControl, BooleanField>(nameof(BooleanField));
        services.AddKeyedTransient<UserControl, ComboboxField>(nameof(ComboboxField));
        services.AddKeyedTransient<UserControl, DateOnlyField>(nameof(DateOnlyField));
        services.AddKeyedTransient<UserControl, DenominationField>(nameof(DenominationField));
        services.AddKeyedTransient<UserControl, FileField>(nameof(FileField));
        services.AddKeyedTransient<UserControl, FolderField>(nameof(FolderField));
        services.AddKeyedTransient<UserControl, IntegerField>(nameof(IntegerField));
        services.AddKeyedTransient<UserControl, SingleLineTextField>(nameof(SingleLineTextField));
        services.AddKeyedTransient<UserControl, ParameterizedStringField>(nameof(ParameterizedStringField));
        return services;
    }
}
