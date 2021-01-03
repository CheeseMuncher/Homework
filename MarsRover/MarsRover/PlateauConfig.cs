namespace MarsRover
{
    public interface IPlateauConfig
    {
        /// <summary>
        /// Indicates if simulator will report out of bounds start positions
        /// </summary>
        bool AllowStartOutsideBounds { get; }

        /// <summary>
        /// Indicates if simulator will report out of bounds finish positions
        /// </summary>
        bool AllowFinishOutsideBounds { get; }

        /// <summary>
        /// Indicates how far outside the 2D map rovers can travel without out of bounds being reported
        /// </summary>
        int ToleranceOutsideBounds { get; }

        /// <summary>
        /// Indicates if Rover routes should be executed sequentially or simultaneously
        /// </summary>
        RoverMovementType RoverMovementType { get; }
    }

    public class PlateauConfig : IPlateauConfig
    {
        public bool AllowStartOutsideBounds { get; set; }
        public bool AllowFinishOutsideBounds { get; set; }
        public int ToleranceOutsideBounds { get; set; }
        public RoverMovementType RoverMovementType { get; set; }
    }
}