using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarsRover
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello Mars!");

            var serviceProvider = ConfigureServiceProvider();

            var input = args.Any() ? args : new[] { "5 5", "1 2 N", "LMLMLMLMM", "3 3 E", "MMRMMRMRRM" };

            var validator = serviceProvider.GetService<IInputValidator>();
            var result = validator.ValidateInput(input);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                    Console.WriteLine(error);

                return;
            }

            var output = new List<string>();
            var index = 0;
            RoverPosition currentRover = null;
            Console.WriteLine("Input:");
            foreach (var line in input)
            {
                Console.WriteLine(line);
                if (index == 0)
                {
                    index++;
                    continue;
                }

                if (index % 2 == 1)
                    currentRover = new RoverPosition(line);

                if (index % 2 == 0)
                    output.Add(currentRover.ExecuteRoute(line));

                index++;
            }

            Console.WriteLine("Output:");
            foreach (var line in output)
                Console.WriteLine(line);
        }

        private static ServiceProvider ConfigureServiceProvider()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            var plateauConfig = new PlateauConfig();
            configuration.GetSection("PlateauConfig").Bind(plateauConfig);

            services
                .AddSingleton(configuration)
                .AddSingleton<IPlateauConfig>(plateauConfig)
                .AddSingleton<IInputValidator, InputValidator>();

            return services.BuildServiceProvider();
        }
    }
}