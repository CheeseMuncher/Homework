using System;
using System.Collections.Generic;

namespace MarsRover
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello Mars!");

            var input = new[] { "5 5", "1 2 N", "LMLMLMLMM", "3 3 E", "MMRMMRMRRM" };
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
    }
}