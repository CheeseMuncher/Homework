using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Reflection;

namespace Paymentsense.Coding.Challenge.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static string ApplicationName =>
            Environment.GetEnvironmentVariable("ASPNETCORE_APPLICATIONNAME") ?? "Countries API";

        public static string ApplicationVersion =>
            typeof(Program)
                .GetTypeInfo()
                .Assembly
                .GetCustomAttributes<AssemblyInformationalVersionAttribute>()
                .FirstOrDefault()?
                .InformationalVersion ?? "1.0.0";
    }
}