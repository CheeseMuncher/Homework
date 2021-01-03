using MarsRover;
using Shouldly;
using Xunit;

namespace MarsRoverTests
{
    public class RoverPositionTests
    {
        [Theory]
        [InlineData(Orientation.N, Orientation.W)]
        [InlineData(Orientation.W, Orientation.S)]
        [InlineData(Orientation.S, Orientation.E)]
        [InlineData(Orientation.E, Orientation.N)]
        public void SpinLeft_WorkCorrectly(Orientation input, Orientation expected)
        {
            var sut = new RoverPosition(input, 2, 2);

            sut.SpinLeft();

            sut.Orientation.ShouldBe(expected);
        }

        [Theory]
        [InlineData(Orientation.N, Orientation.E)]
        [InlineData(Orientation.E, Orientation.S)]
        [InlineData(Orientation.S, Orientation.W)]
        [InlineData(Orientation.W, Orientation.N)]
        public void SpinRight_WorkCorrectly(Orientation input, Orientation expected)
        {
            var sut = new RoverPosition(input, 2, 2);

            sut.SpinRight();

            sut.Orientation.ShouldBe(expected);
        }

        [Theory]
        [InlineData(Orientation.N, 2, 3)]
        [InlineData(Orientation.E, 3, 2)]
        [InlineData(Orientation.S, 2, 1)]
        [InlineData(Orientation.W, 1, 2)]
        public void Move_WorkCorrectly(Orientation input, int expectedX, int expectedY)
        {
            var sut = new RoverPosition(input, 2, 2);

            sut.Move();

            sut.X.ShouldBe(expectedX);
            sut.Y.ShouldBe(expectedY);
        }

        [Theory]
        [InlineData("1 2 N", 1, 2, Orientation.N)]
        [InlineData("3 3 E", 3, 3, Orientation.E)]
        public void ConstructorOverload_CreatesRoverPositionCorrectly(string input, int expectedX, int expectedY, Orientation expectedOrientation)
        {
            var sut = new RoverPosition(input);

            sut.X.ShouldBe(expectedX);
            sut.Y.ShouldBe(expectedY);
            sut.Orientation.ShouldBe(expectedOrientation);
        }

        [Fact]
        public void ToStringOverride_ReturnsPositionOutput()
        {
            var sut = new RoverPosition(Orientation.N, 1, 2);

            var result = sut.ToString();

            result.ShouldBe("1 2 N");
        }

        [Theory]
        [InlineData("1 2 N", "LMLMLMLMM", "1 3 N")]
        [InlineData("3 3 E", "MMRMMRMRRM", "5 1 E")]
        public void ExecuteRoute_ReturnsExpectedOutput(string inputPosition, string inputRoute, string expectedOutput)
        {
            var sut = new RoverPosition(inputPosition);

            var result = sut.ExecuteRoute(inputRoute);

            result.ShouldBe(expectedOutput);
        }
    }
}