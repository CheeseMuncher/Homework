using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ShoppingCart
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                    builder
                        .UseSerilog(((context, config) =>
                        {
                            config
                                .ReadFrom.Configuration(context.Configuration)
                                .Enrich.FromLogContext()
                                .MinimumLevel.Information()
                                .WriteTo.Console()
                                .WriteTo.File(new RenderedCompactJsonFormatter(),
                                    Path.Join("logs", $"log-{DateTime.UtcNow.ToString("yyyy-MM-dd")}.json"));
                        }))
                        .UseStartup<Startup>());
        }

        public static string ApplicationName =>
            Environment.GetEnvironmentVariable("ASPNETCORE_APPLICATIONNAME") ?? "Shopping Cart";

        public static string ApplicationVersion =>
            typeof(Program)
                .GetTypeInfo()
                .Assembly
                .GetCustomAttributes<AssemblyInformationalVersionAttribute>()
                .FirstOrDefault()?
                .InformationalVersion ?? "1.0.0";
    }
}