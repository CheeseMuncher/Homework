using System.Collections.Generic;
using System.Linq;

namespace MarsRover
{
    public class ValidationResult
    {
        public bool IsValid => !Errors.Any();

        public IEnumerable<string> Errors { get; private set; } = new List<string>();

        public ValidationResult(string[] errors)
        {
            Errors = errors ?? new string[0];
        }

        public ValidationResult()
        {
            Errors = new string[0];
        }
    }

    public static class ValidationResultExtensions
    {
        public static ValidationResult Combine(this ValidationResult result, ValidationResult otherResult)
        {
            return otherResult == null
                ? result
                : new ValidationResult(result.Errors.Concat(otherResult.Errors).ToArray());
        }
    }
}