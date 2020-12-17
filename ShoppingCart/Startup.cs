using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using ShoppingCart.Implementation;
using ShoppingCart.Interfaces;
using ShoppingCart.Models;

namespace ShoppingCart
{
    public class Startup
    {
        private readonly ILogger _logger;

        public Startup()
        {
            _logger = Log.Logger.ForContext<Startup>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _logger.Information("Starting {ApplicationName} {ApplicationVersion}", Program.ApplicationName, Program.ApplicationVersion);

            services
                .AddMvc(opt => opt.EnableEndpointRouting = false)
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = $"{Program.ApplicationName}", Version = $"{Program.ApplicationVersion}" });
                c.EnableAnnotations();
            });

            services.AddSingleton<IRepository<Product>, ProductRepository>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
            app.UsePingEndpoint();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Program.ApplicationName} v{Program.ApplicationVersion}"));
        }
    }
}