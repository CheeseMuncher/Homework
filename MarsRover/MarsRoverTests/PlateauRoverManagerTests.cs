using MarsRover;
using Moq;
using Shouldly;
using System.Linq;
using Xunit;

namespace MarsRoverTests
{
    public class PlateauRoverManagerTests
    {
        private Mock<IPlateauConfig> _mockPlateauConfig = new Mock<IPlateauConfig>();
        private IPlateauRoverManager _sut;

        public PlateauRoverManagerTests()
        {
            _sut = new PlateauRoverManager(_mockPlateauConfig.Object);
        }

        [Fact]
        public void SetSize_SetsBoundariesCorrectly()
        {
            var x = 123;
            var y = 456;

            _sut.SetSize(x, y);

            _sut.XBound.ShouldBe(x);
            _sut.YBound.ShouldBe(y);
        }

        [Fact]
        public void AddRovers_SetsSingleRoverCorrectly()
        {
            var input = "1 2 N";
            _sut.SetSize(5, 5);

            _sut.AddRovers(new[] { input }).ToList();

            var rovers = _sut.RoverPositions;
            rovers.Count().ShouldBe(1);
            rovers.Single().X.ShouldBe(1);
            rovers.Single().Y.ShouldBe(2);
            rovers.Single().Orientation.ShouldBe(Orientation.N);
        }

        [Fact]
        public void AddRovers_SetsMultipleRoversCorrectly()
        {
            var input = new[] { "1 2 N", "3 3 E" };
            _sut.SetSize(5, 5);

            _sut.AddRovers(input).ToList();

            var rovers = _sut.RoverPositions;
            rovers.Count().ShouldBe(2);
            rovers.Last().X.ShouldBe(3);
            rovers.Last().Y.ShouldBe(3);
            rovers.Last().Orientation.ShouldBe(Orientation.E);
        }

        [Fact]
        public void SetSize_ClearsRoverList()
        {
            var input = "1 2 N";
            _sut.AddRovers(new[] { input });

            _sut.SetSize(2, 2);

            var rovers = _sut.RoverPositions;
            rovers.Count().ShouldBe(0);
        }

        [Fact]
        public void AddRovers_ReportsEntryCollisionsWithinBatch()
        {
            var input = new[] { "1 2 N", "1 2 N" };
            _sut.SetSize(5, 5);

            var result = _sut.AddRovers(input).ToList();

            result.Count().ShouldBe(1);
            result.Single().ShouldBe("Rover #1: starting position 1 2 is already occupied");
        }

        [Fact]
        public void AddRovers_ReportsCollisionsBetweenBatches()
        {
            var input = new[] { "1 2 N", "3 3 E" };
            _sut.SetSize(5, 5);

            var firstResult = _sut.AddRovers(input).ToList();
            var secondResult = _sut.AddRovers(input).ToList();

            firstResult.Count().ShouldBe(0);
            secondResult.Count().ShouldBe(2);
            secondResult.ShouldContain(m => m == "Rover #0: starting position 1 2 is already occupied");
            secondResult.ShouldContain(m => m == "Rover #1: starting position 3 3 is already occupied");
        }

        [Fact]
        public void AddRovers_ReportsOutOfBoundsStartPosition_IfAllowStartOutsideBoundsFalse()
        {
            _mockPlateauConfig.Setup(c => c.AllowStartOutsideBounds).Returns(false);
            _sut.SetSize(1, 1);
            var input = new[] { "1 1 E", "2 2 N" };

            var result = _sut.AddRovers(input).ToList();

            _sut.RoverPositions.Count().ShouldBe(2);
            result.Count().ShouldBe(1);
            result.Single().ShouldBe("Rover #1: starting position 2 2 is out of bounds");
        }

        [Fact]
        public void AddRovers_SetsRoverPosition_IfAllowStartOutsideBoundsTrue()
        {
            _mockPlateauConfig.Setup(c => c.AllowStartOutsideBounds).Returns(true);
            _sut.SetSize(1, 1);
            var input = new[] { "2 2 N" };

            _sut.AddRovers(input).ToList();

            _sut.RoverPositions.Count().ShouldBe(1);
            _sut.RoverPositions.Single().X.ShouldBe(2);
            _sut.RoverPositions.Single().Y.ShouldBe(2);
        }

        [Theory]
        [InlineData(RoverMovementType.Sequential)]
        [InlineData(RoverMovementType.Simultaneous)]
        public void MoveRovers_PerformsMovesCorrectly(RoverMovementType movementType)
        {
            _mockPlateauConfig.Setup(c => c.RoverMovementType).Returns(movementType);
            _sut.SetSize(5, 5);
            _sut.AddRovers(new[] { "1 2 N", "3 3 E" }).ToList();
            var routes = new[] { "LMLMLMLMM", "MMRMMRMRRM" };

            var results = _sut.MoveRovers(routes).ToList();

            results.Count().ShouldBe(2);
            results.First().ShouldBe("1 3 N");
            results.Last().ShouldBe("5 1 E");
        }

        [Theory]
        [InlineData(RoverMovementType.Sequential)]
        [InlineData(RoverMovementType.Simultaneous)]
        public void MoveRovers_ReportsOutOfBoundsInRoute(RoverMovementType movementType)
        {
            _mockPlateauConfig.Setup(c => c.RoverMovementType).Returns(movementType);
            _sut.SetSize(5, 5);
            _sut.AddRovers(new[] { "1 1 N", "3 3 E" }).ToList();
            var routes = new[] { "LMMRMRM", "MMRMMRMRRM" };

            var results = _sut.MoveRovers(routes).ToList();

            results.Count().ShouldBe(6);
            results.ShouldContain(m => m == "Rover #0 is out of bounds after the 3rd step");
            results.ShouldContain(m => m == "Rover #0 is out of bounds after the 4th step");
            results.ShouldContain(m => m == "Rover #0 is out of bounds after the 5th step");
            results.ShouldContain(m => m == "Rover #0 is out of bounds after the 6th step");
        }

        [Theory]
        [InlineData(2, 2, Orientation.E)]
        [InlineData(-2, 2, Orientation.N)]
        [InlineData(2, -2, Orientation.S)]
        [InlineData(-2, -2, Orientation.W)]
        public void MoveRovers_ReportsOutOfBoundsCorrectly_WithNegativePlateauInputs(int x, int y, Orientation orientation)
        {
            _mockPlateauConfig.Setup(c => c.RoverMovementType).Returns(RoverMovementType.Sequential);
            _sut.SetSize(x, y);
            _sut.AddRovers(new[] { $"0 0 {orientation}" }).ToList();
            var routes = new[] { "MLMLMMLMMLMMLMLMLL" };

            var results = _sut.MoveRovers(routes).ToList();

            results.Count().ShouldBe(9);
            results.ShouldContain(m => m == "Rover #0 is out of bounds after the 6th step");
            results.ShouldContain(m => m == "Rover #0 is out of bounds after the 7th step");
            results.ShouldContain(m => m == "Rover #0 is out of bounds after the 8th step");
            results.ShouldContain(m => m == "Rover #0 is out of bounds after the 9th step");
            results.ShouldContain(m => m == "Rover #0 is out of bounds after the 10th step");
            results.ShouldContain(m => m == "Rover #0 is out of bounds after the 11th step");
            results.ShouldContain(m => m == "Rover #0 is out of bounds after the 12th step");
            results.ShouldContain(m => m == "Rover #0 is out of bounds after the 13th step");
            results.Last().ShouldBe($"0 0 {orientation}");
        }

        [Theory]
        [InlineData(RoverMovementType.Sequential)]
        [InlineData(RoverMovementType.Simultaneous)]
        public void MoveRovers_ReportsOutOfBoundsInRouteWithTolerance(RoverMovementType movementType)
        {
            _mockPlateauConfig.Setup(c => c.RoverMovementType).Returns(movementType);
            _mockPlateauConfig.Setup(c => c.ToleranceOutsideBounds).Returns(1);
            _sut.SetSize(5, 5);
            _sut.AddRovers(new[] { "3 3 E", "1 1 N" }).ToList();
            var routes = new[] { "MM", "LMMMRRMM" };

            var results = _sut.MoveRovers(routes).ToList();

            results.Count().ShouldBe(5);
            results.ShouldContain(m => m == "Rover #1 is out of bounds after the 4th step");
            results.ShouldContain(m => m == "Rover #1 is out of bounds after the 5th step");
            results.ShouldContain(m => m == "Rover #1 is out of bounds after the 6th step");
        }

        [Theory]
        [InlineData(RoverMovementType.Sequential)]
        [InlineData(RoverMovementType.Simultaneous)]
        public void MoveRovers_ReportsOutOfBoundsEndPosition_IfAllowFinishOutsideBoundsFalse(RoverMovementType movementType)
        {
            _mockPlateauConfig.Setup(c => c.RoverMovementType).Returns(movementType);
            _mockPlateauConfig.Setup(c => c.ToleranceOutsideBounds).Returns(1);
            _mockPlateauConfig.Setup(c => c.AllowFinishOutsideBounds).Returns(false);
            _sut.SetSize(5, 5);
            _sut.AddRovers(new[] { "1 1 N" }).ToList();
            var routes = new[] { "LMMRMM" };

            var results = _sut.MoveRovers(routes).ToList();

            results.Count().ShouldBe(2);
            results.ShouldContain(m => m == "Rover #0: route will end out of bounds");
        }

        [Theory]
        [InlineData(RoverMovementType.Sequential)]
        [InlineData(RoverMovementType.Simultaneous)]
        public void MoveRovers_DoesNotReportOutOfBoundsEndPosition_IfAllowFinishOutsideBoundsTrue(RoverMovementType movementType)
        {
            _mockPlateauConfig.Setup(c => c.RoverMovementType).Returns(movementType);
            _mockPlateauConfig.Setup(c => c.AllowFinishOutsideBounds).Returns(true);
            _sut.SetSize(5, 5);
            _sut.AddRovers(new[] { "1 1 N" }).ToList();
            var routes = new[] { "LMM" };

            var results = _sut.MoveRovers(routes).ToList();

            results.ShouldContain(m => m == "-1 1 W");
            results.ShouldNotContain(m => m == "Rover #0: route will end out of bounds");
        }

        [Fact]
        public void MoveRovers_ReportsCollision1_InSequentialConfiguration()
        {
            _mockPlateauConfig.Setup(c => c.RoverMovementType).Returns(RoverMovementType.Sequential);
            _sut.SetSize(5, 5);
            _sut.AddRovers(new[] { "1 1 N", "3 3 E" }).ToList();
            var routes = new[] { "MMRMM", "MMRMM" };

            var results = _sut.MoveRovers(routes).ToList();

            results.ShouldContain(m => m == "Rover #0: route will take rover into collision with another rover after the 5th step");
        }

        [Fact]
        public void MoveRovers_ReportsCollision2_InSequentialConfiguration()
        {
            _sut.SetSize(5, 5);
            _sut.AddRovers(new[] { "1 1 N", "3 3 E" }).ToList();
            var routes = new[] { "MM", "RRMM" };

            var results = _sut.MoveRovers(routes).ToList();

            results.ShouldContain(m => m == "Rover #1: route will take rover into collision with another rover after the 4th step");
        }

        [Fact]
        public void MoveRovers_ReportsCollision_InSimultaneousConfiguration()
        {
            _mockPlateauConfig.Setup(c => c.RoverMovementType).Returns(RoverMovementType.Simultaneous);
            _sut.SetSize(5, 5);
            _sut.AddRovers(new[] { "1 1 N", "1 5 S" }).ToList();
            var routes = new[] { "MMR", "MML" };

            var results = _sut.MoveRovers(routes).ToList();

            results.ShouldContain(m => m.Contains("Rover #0: route will take rover into collision with another rover"));
            results.ShouldContain(m => m.Contains("Rover #1: route will take rover into collision with another rover"));
        }

        [Fact]
        public void MoveRovers_FourRoverFullReportScenario()
        {
            _mockPlateauConfig.Setup(c => c.RoverMovementType).Returns(RoverMovementType.Simultaneous);
            _sut.SetSize(4, 4);
            _sut.AddRovers(new[] { "1 2 E", "5 2 N", "2 0 N", "2 0 N" }).ToList();
            var routes = new[] { "MMMLM", "MLMLMRM", "MMRM", "RRM" };

            var results = _sut.MoveRovers(routes).ToList();

            results.Count().ShouldBe(12);
            results.ShouldContain(m => m == "Rover #2 is sharing a cell with another rover before the 1st step");
            results.ShouldContain(m => m == "Rover #0: route will take rover into collision with another rover after the 5th step");
            results.ShouldContain(m => m == "Rover #1 is sharing a cell with another rover before the 5th step");
            results.ShouldContain(m => m == "Rover #1: route will take rover into collision with another rover after the 7th step");
        }
    }
}