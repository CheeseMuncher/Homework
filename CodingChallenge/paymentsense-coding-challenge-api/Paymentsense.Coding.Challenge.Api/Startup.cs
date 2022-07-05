using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Paymentsense.Coding.Challenge.Api.Services;
using System;
using System.Net.Http;

namespace Paymentsense.Coding.Challenge.Api
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
            var apiConfig = new ApiConfig();
            Configuration.GetSection("ApiConfig").Bind(apiConfig);
            services.AddSingleton(apiConfig);

            services.TryAddTransient(s => s.GetRequiredService<IHttpClientFactory>().CreateClient(string.Empty));
            services.AddHttpClient<IRestCountriesApi, RestCountriesApi>().SetHandlerLifetime(TimeSpan.FromMinutes(5));
            services.AddSingleton<IRestCountriesApi, RestCountriesApi>();
            services.AddSingleton<ICountryService, CachedCountryService>();
            services.AddMemoryCache();
            services.AddControllers();
            services.AddHealthChecks();
            services.AddCors(options =>
            {
                options.AddPolicy("PaymentsenseCodingChallengeOriginPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = $"{Program.ApplicationName}", Version = $"{Program.ApplicationVersion}" });
                c.EnableAnnotations();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors("PaymentsenseCodingChallengeOriginPolicy");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Program.ApplicationName} v{Program.ApplicationVersion}"));
        }
    }
}