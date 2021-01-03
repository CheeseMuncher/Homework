using System;
using System.Linq;

namespace MarsRover
{
    public interface IRoverPosition
    {
        Orientation Orientation { get; }
        int X { get; }
        int Y { get; }

        void SpinLeft();

        void SpinRight();

        void Move();

        string ExecuteRoute(string route);
    }

    public class RoverPosition : IRoverPosition
    {
        public Orientation Orientation { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public RoverPosition(Orientation orientation, int x, int y)
        {
            Orientation = orientation;
            X = x;
            Y = y;
        }

        public RoverPosition(string input)
        {
            var split = input.Split(" ");
            Orientation = (Orientation)Enum.Parse(typeof(Orientation), split.Last());
            X = int.Parse(split[0]);
            Y = int.Parse(split[1]);
        }

        public void SpinLeft()
        {
            Orientation = (Orientation)((int)Orientation == 0 ? 3 : (int)Orientation - 1);
        }

        public void SpinRight()
        {
            Orientation = (Orientation)((int)Orientation == 3 ? 0 : (int)Orientation + 1);
        }

        public void Move()
        {
            switch (Orientation)
            {
                case Orientation.N:
                    Y++;
                    break;

                case Orientation.E:
                    X++;
                    break;

                case Orientation.S:
                    Y--;
                    break;

                case Orientation.W:
                    X--;
                    break;
            }
        }

        public override string ToString()
        {
            return $"{X} {Y} {Orientation}";
        }

        public string ExecuteRoute(string route)
        {
            foreach (char c in route)
            {
                switch (c)
                {
                    case 'L':
                        SpinLeft();
                        break;

                    case 'R':
                        SpinRight();
                        break;

                    case 'M':
                        Move();
                        break;
                }
            }
            return ToString();
        }
    }
}