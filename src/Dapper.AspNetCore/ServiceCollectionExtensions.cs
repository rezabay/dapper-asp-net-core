using System;
using Dapper.AspNetCore;
using Dapper.AspNetCore.Factories;
using Dapper.AspNetCore.Repository;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDapper(this IServiceCollection services, Action<DapperOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddOptions();
            services.Configure(setupAction);

            services.AddSingleton<IConnectionFactory, ConnectionFactory>();
            services.AddTransient<IAppRepository, AppRepository>();

            return services;
        }
    }
}
