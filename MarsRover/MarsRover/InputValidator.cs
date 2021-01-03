using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarsRover
{
    public interface IInputValidator
    {
        ValidationResult ValidateInput(string[] input);
    }

    public class InputValidator : IInputValidator
    {
        private const string IntegerRegex = @"^[-+]?\d+$";
        private const string RouteRegex = @"^[LMR]*$";

        public ValidationResult ValidateInput(string[] input)
        {
            if (input?.Any() != true)
                return new ValidationResult(new[] { "No input supplied" });

            var roverCount = input.Length / 2;
            var result = ValidateFirstLine(input[0]);

            for (int i = 0; i < roverCount; i++)
            {
                result = result.Combine(ValidateRoverPosition(i, input[2 * i + 1]));
                if (i == roverCount - 1 && input.Length % 2 == 0)
                    result = result.Combine(new ValidationResult(new[] { $"Rover #{i}: no route input supplied" }));
                else
                    result = result.Combine(ValidateRoverRoute(i, input[2 * (i + 1)]));
            }

            return result;
        }

        private ValidationResult ValidateFirstLine(string input)
        {
            var split = input.Split(" ");
            if (split.Count() != 2 || split.Any(s => !Regex.Match(s, IntegerRegex).Success))
                return new ValidationResult(new[] { "First line of input should consist of exactly two integers" });

            if (split.Any(s => int.Parse(s) == 0))
                return new ValidationResult(new[] { "First line of input results in one dimensional search area" });

            return new ValidationResult();
        }

        private ValidationResult ValidateRoverPosition(int count, string input)
        {
            var test = typeof(Orientation).GetEnumValues();
            var split = input.Split(" ");
            if (split.Count() == 3
                && split.Take(2).All(s => Regex.Match(s, IntegerRegex).Success)
                && Enum.IsDefined(typeof(Orientation), split.Last()))
                return new ValidationResult();

            return new ValidationResult(new[] { $"Rover #{count}: position input invalid: input should consist of exactly two integers and one cardinal point" });
        }

        private ValidationResult ValidateRoverRoute(int count, string input)
        {
            if (Regex.Match(input, RouteRegex).Success)
                return new ValidationResult();

            return new ValidationResult(new[] { $"Rover #{count}: route input invalid: input should consist of only the letters L, M and R" });
        }
    }
}