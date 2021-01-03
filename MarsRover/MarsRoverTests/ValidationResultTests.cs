using MarsRover;
using Shouldly;
using System.Linq;
using Xunit;

namespace MarsRoverTests
{
    public class ValidationResultTests
    {
        [Fact]
        public void ValidationResult_HandlesNullConstructorInput()
        {
            var sut = new ValidationResult(null);

            var _ = sut.IsValid;

            // test passes by virtue of no NullReferenceException being thrown
        }

        [Fact]
        public void ValidationResult_IsValid_IfNoErrors()
        {
            var sut = new ValidationResult();

            var result = sut.IsValid;

            result.ShouldBeTrue();
        }

        [Fact]
        public void ValidationResult_IsValid_IfItContainsErrors()
        {
            var sut = new ValidationResult(new[] { "Testing" });

            var result = sut.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void ValidationResult_ExposesAllErrors()
        {
            var sut = new ValidationResult(new[] { "Testing1", "Testing2" });

            var result = sut.Errors;

            result.Count().ShouldBe(2);
        }

        [Fact]
        public void Combine_ShouldMergeErrorCollections()
        {
            var result1 = new ValidationResult(new[] { "Testing1", "Testing2" });
            var result2 = new ValidationResult(new[] { "Testing3" });

            var result = result1.Combine(result2);

            result.Errors.Count().ShouldBe(3);
        }

        [Fact]
        public void Combine_ShouldHandleNullInput()
        {
            var result1 = new ValidationResult(new[] { "Testing1", "Testing2" });

            var result = result1.Combine(null);

            result.Errors.Count().ShouldBe(2);
        }
    }
}