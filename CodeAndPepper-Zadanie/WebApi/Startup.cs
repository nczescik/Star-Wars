using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.DAL;
using WebApi.Extensions;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDependencyInjection()
                .AddDbContext(Configuration)
                .AddSwagger()
                .AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, StarWarsDbContext starWarsDbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                    .UseSwaggerExt();
            }
            app.UseExceptionHandler("/error")
                .UseHttpsRedirection()
                .UseRouting()
                .UseAuthorization()
                .UseEndpointsExt();

            starWarsDbContext.Database.EnsureCreated();
        }
    }
}
