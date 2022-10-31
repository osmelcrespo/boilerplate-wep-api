using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class Program
    {
        public static IServiceCollection AddInfraConnection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(GetConnectionString(configuration)));
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(GetConnectionString(configuration)));

            services.AddDefaultIdentity<ApplicationUser>()
                    .AddEntityFrameworkStores<AuthDbContext>()
                    .AddDefaultTokenProviders();

            services.AddTransient<SignInManager<ApplicationUser>>();
            services.AddTransient<UserManager<ApplicationUser>>();

            return services;
        }

        private static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AppCS");

            var server = Environment.GetEnvironmentVariable("SERVER");
            var database = Environment.GetEnvironmentVariable("DATABASE");
            var user = Environment.GetEnvironmentVariable("USER");
            var password = Environment.GetEnvironmentVariable("PASSWORD");

            connectionString = connectionString.Replace(@"{{SERVER}}", server).Replace(@"{{DATABASE}}", database).Replace(@"{{USER}}", user).Replace(@"{{PASSWORD}}", password);

            return connectionString;
        }

        public static IApplicationBuilder UpdateDatabase(this IApplicationBuilder applicationBuilder, AppDbContext appDbContext)
        {
            appDbContext.Database.Migrate();
            return applicationBuilder;
        }
    }
}
