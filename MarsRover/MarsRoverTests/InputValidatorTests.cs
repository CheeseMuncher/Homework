using MarsRover;
using Shouldly;
using System.Linq;
using Xunit;

namespace MarsRoverTests
{
    public class InputValidatorTests
    {
        private IInputValidator _sut = new InputValidator();

        [Fact]
        public void Validate_RetunsValidResult_IfInputValid()
        {
            var input = GetValidInput();

            var result = _sut.ValidateInput(input);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Validate_RetunsInvalidResult_IfInputNull()
        {
            var result = _sut.ValidateInput(null);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count().ShouldBe(1);
            result.Errors.Single().ShouldBe("No input supplied");
        }

        [Fact]
        public void Validate_RetunsInvalidResult_IfInputEmpty()
        {
            var result = _sut.ValidateInput(new string[0]);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count().ShouldBe(1);
            result.Errors.Single().ShouldBe("No input supplied");
        }

        [Theory]
        [InlineData("abc")]
        [InlineData(".#")]
        [InlineData("12-3")]
        [InlineData("12.1 3")]
        [InlineData("12 b")]
        [InlineData("12 13 14")]
        public void Validate_RetunsInvalidResult_IfFirstInputLineNotTwoIntegers(string firstInput)
        {
            var input = GetValidInput();
            input[0] = firstInput;

            var result = _sut.ValidateInput(input);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count().ShouldBe(1);
            result.Errors.Single().ShouldBe("First line of input should consist of exactly two integers");
        }

        [Theory]
        [InlineData("5 0")]
        [InlineData("0 5")]
        public void Validate_RetunsInvalidResult_IfFirstInputLineContainsZeros(string firstInput)
        {
            var input = GetValidInput();
            input[0] = firstInput;

            var result = _sut.ValidateInput(input);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count().ShouldBe(1);
            result.Errors.Single().ShouldBe("First line of input results in one dimensional search area");
        }

        [Theory]
        [InlineData("abc")]
        [InlineData(".#")]
        [InlineData("12-3")]
        [InlineData("12.1 3")]
        [InlineData("12 b")]
        [InlineData("12 13 H")]
        public void Validate_RetunsInvalidResult_IfFirstRoverPositionLineInvalidExpression(string positionInput)
        {
            var input = GetValidInput();
            input[1] = positionInput;

            var result = _sut.ValidateInput(input);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count().ShouldBe(1);
            result.Errors.Single().ShouldBe("Rover #0: position input invalid: input should consist of exactly two integers and one cardinal point");
        }

        [Theory]
        [InlineData("abc")]
        [InlineData(".#")]
        [InlineData("123")]
        [InlineData("mlr")]
        [InlineData("MLR12")]
        [InlineData("MMR MLM")]
        public void Validate_RetunsInvalidResult_IfFirstRoverRouteInvalid(string routeInput)
        {
            var input = GetValidInput();
            input[2] = routeInput;

            var result = _sut.ValidateInput(input);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count().ShouldBe(1);
            result.Errors.Single().ShouldBe("Rover #0: route input invalid: input should consist of only the letters L, M and R");
        }

        [Fact]
        public void Validate_RetunsInvalidResult_LastRouteNotFound()
        {
            var input = GetValidInput();
            input = input.Concat(new[] { "4 4 E" }).ToArray();

            var result = _sut.ValidateInput(input);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count().ShouldBe(1);
            result.Errors.Single().ShouldBe("Rover #2: no route input supplied");
        }

        [Fact]
        public void Validate_ChainsMultipleErrors()
        {
            var input = GetValidInput();
            input[0] = "A";
            input[3] = "B";
            input[4] = "C";
            input = input.Concat(new[] { "1 1 N" }).ToArray();

            var result = _sut.ValidateInput(input);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count().ShouldBe(4);
            result.Errors.ShouldContain(e => e == "First line of input should consist of exactly two integers");
            result.Errors.ShouldContain(e => e == "Rover #1: position input invalid: input should consist of exactly two integers and one cardinal point");
            result.Errors.ShouldContain(e => e == "Rover #1: route input invalid: input should consist of only the letters L, M and R");
            result.Errors.ShouldContain(e => e == "Rover #2: no route input supplied");
        }

        private string[] GetValidInput()
        {
            return new[] { "5 5", "1 2 N", "LMLMLMLMM", "3 3 E", "MMRMMRMRRM" };
        }
    }
}