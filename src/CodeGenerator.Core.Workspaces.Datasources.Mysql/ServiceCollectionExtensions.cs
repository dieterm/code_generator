using CodeGenerator.Core.Workspaces.Datasources.Mysql.Services;
using CodeGenerator.Core.Workspaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.Workspaces.Datasources.Mysql
{
    /// <summary>
    /// Extension methods for registering MySQL datasource services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register MySQL datasource services
        /// </summary>
        public static IServiceCollection AddMysqlDatasourceServices(
            this IServiceCollection services,
            IConfiguration? configuration = null)
        {
            // Register the datasource provider
            services.AddSingleton<IDatasourceProvider, MysqlDatasourceProvider>();

            // Register the schema reader
            services.AddTransient<MysqlSchemaReader>();

            return services;
        }
    }
}
