using System;
using System.Collections.Generic;
using System.Linq;

namespace MarsRover
{
    public interface IPlateauRoverManager
    {
        int XBound { get; }
        int YBound { get; }
        IEnumerable<RoverPosition> RoverPositions { get; }

        void SetSize(int x, int y);

        IEnumerable<string> AddRovers(IEnumerable<string> rovers);

        IEnumerable<string> MoveRovers(IEnumerable<string> routes);
    }

    public class PlateauRoverManager : IPlateauRoverManager
    {
        private readonly IPlateauConfig _plateauConfig;

        public int XBound { get; private set; }
        public int YBound { get; private set; }

        private List<RoverPosition> _rovers = new List<RoverPosition>();

        public IEnumerable<RoverPosition> RoverPositions => _rovers;

        public PlateauRoverManager(IPlateauConfig plateauConfig)
        {
            _plateauConfig = plateauConfig;
        }

        public void SetSize(int x, int y)
        {
            XBound = x;
            YBound = y;
            _rovers.Clear();
        }

        public IEnumerable<string> AddRovers(IEnumerable<string> rovers)
        {
            int index = 0;
            foreach (var input in rovers)
            {
                var entrant = new RoverPosition(input);
                if (PositionOccupied(entrant.X, entrant.Y))
                    yield return DuplicateStartPositionMessage(index, entrant.X, entrant.Y);

                if (PositionOutOfBounds(entrant.X, entrant.Y, _plateauConfig.ToleranceOutsideBounds))
                    yield return OutOfBoundsStartMessage(index, entrant.X, entrant.Y);

                _rovers.Add(entrant);
                index++;
            }
        }

        public IEnumerable<string> MoveRovers(IEnumerable<string> routes)
        {
            return _plateauConfig.RoverMovementType == RoverMovementType.Sequential
                ? MoveRoversSequentially(routes)
                : MoveRoversSimultaneously(routes);
        }

        private IEnumerable<string> MoveRoversSequentially(IEnumerable<string> routes)
        {
            var roverIndex = 0;
            foreach (var route in routes)
            {
                var rover = _rovers[roverIndex];
                var routeIndex = 1;
                foreach (var c in route)
                {
                    if (PositionOccupied(rover.X, rover.Y, rover))
                        yield return PremoveCheck(roverIndex, routeIndex);

                    MoveRover(rover, c);

                    if (PositionOutOfBounds(rover.X, rover.Y, _plateauConfig.ToleranceOutsideBounds))
                        yield return OutOfBoundsMessage(roverIndex, routeIndex);

                    if (PositionOccupied(rover.X, rover.Y, rover))
                        yield return CollisionAfterMove(roverIndex, routeIndex);

                    routeIndex++;
                }
                if (!_plateauConfig.AllowFinishOutsideBounds && PositionOutOfBounds(rover.X, rover.Y))
                    yield return OutOfBoundsFinishMessage(roverIndex);

                roverIndex++;
                yield return rover.ToString();
            }
        }

        private IEnumerable<string> MoveRoversSimultaneously(IEnumerable<string> routes)
        {
            var indexedRoutes = routes.ToArray();
            for (var routeIndex = 1; routeIndex <= routes.Max(r => r.Length) + 1; routeIndex++)
            {
                for (int roverIndex = 0; roverIndex < routes.Count(); roverIndex++)
                {
                    var rover = _rovers[roverIndex];
                    if (routeIndex - 1 == indexedRoutes[roverIndex].Length)
                    {
                        yield return rover.ToString();
                        if (!_plateauConfig.AllowFinishOutsideBounds && PositionOutOfBounds(rover.X, rover.Y))
                            yield return OutOfBoundsFinishMessage(roverIndex);
                    }

                    if (routeIndex - 1 >= indexedRoutes[roverIndex].Length)
                        continue;

                    if (PositionOccupied(rover.X, rover.Y, rover))
                        yield return PremoveCheck(roverIndex, routeIndex);

                    MoveRover(rover, indexedRoutes[roverIndex][routeIndex - 1]);

                    if (PositionOutOfBounds(rover.X, rover.Y, _plateauConfig.ToleranceOutsideBounds))
                        yield return OutOfBoundsMessage(roverIndex, routeIndex);

                    if (PositionOccupied(rover.X, rover.Y, rover))
                        yield return CollisionAfterMove(roverIndex, routeIndex);
                }
            }
        }

        private bool PositionOccupied(int x, int y, RoverPosition current = null) => current == null
            ? _rovers.Any(r => r.X == x && r.Y == y)
            : _rovers.Except(new[] { current }).Any(r => r.X == x && r.Y == y);

        private bool PositionOutOfBounds(int x, int y, int tolerance = 0) =>
            x < Math.Min(0, XBound) - tolerance
            || x > Math.Max(0, XBound) + tolerance
            || y < Math.Min(0, YBound) - tolerance
            || y > Math.Max(0, YBound) + tolerance;

        private void MoveRover(RoverPosition rover, char operation)
        {
            switch (operation)
            {
                case 'L':
                    rover.SpinLeft();
                    break;

                case 'R':
                    rover.SpinRight();
                    break;

                case 'M':
                    rover.Move();
                    break;
            }
        }

        private string OutOfBoundsStartMessage(int roverIndex, int x, int y) => $"Rover #{roverIndex}: starting position {x} {y} is out of bounds";

        private string OutOfBoundsFinishMessage(int roverIndex) => $"Rover #{roverIndex}: route will end out of bounds";

        private string DuplicateStartPositionMessage(int roverIndex, int x, int y) => $"Rover #{roverIndex}: starting position {x} {y} is already occupied";

        private string OutOfBoundsMessage(int roverIndex, int routeIndex) => $"Rover #{roverIndex} is out of bounds after the {AddOrdinal(routeIndex)} step";

        private string PremoveCheck(int roverIndex, int routeIndex) => $"Rover #{roverIndex} is sharing a cell with another rover before the {AddOrdinal(routeIndex)} step";

        private string CollisionAfterMove(int roverIndex, int routeIndex) => $"Rover #{roverIndex}: route will take rover into collision with another rover after the {AddOrdinal(routeIndex)} step";

        // Credit: https://stackoverflow.com/a/20175/1007074
        private static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";

                case 2:
                    return num + "nd";

                case 3:
                    return num + "rd";

                default:
                    return num + "th";
            }
        }
    }
}