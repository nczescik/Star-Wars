using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.DAL;
using WebApi.Services.Services.Characters;
using WebAPI.DAL.Interfaces;
using WebAPI.DAL.Repositories;

namespace WebApi.Extensions
{
    public static class StartUpExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<StarWarsDbContext>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ICharacterService, CharacterService>();
            return services;
        }
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<StarWarsDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Database")));
            return services;
        }
    }
}
