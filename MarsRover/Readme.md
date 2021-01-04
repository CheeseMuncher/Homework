Coding challenge
Mars Rover technical Challenge
MARS ROVERS
Input:
Output:

Expected Output:
Mars Rover technical Challenge
The problem below requires some kind of input. You are free to implement any mechanism for feeding input into your solution (for example, using
hard coded data within a unit test). You should provide sufficient evidence that your solution is complete by, as a minimum, indicating that it works
correctly against the supplied test data.
We highly recommend using a unit testing framework. Even if you have not used it before, it is simple to learn and incredibly useful.
The code you write should be of production quality, and most importantly, it should be code you are proud of.
MARS ROVERS
A squad of robotic rovers are to be landed by NASA on a plateau on Mars.
This plateau, which is curiously rectangular, must be navigated by the rovers so that their on board cameras can get a complete view of the
surrounding terrain to send back to Earth.
A rover's position is represented by a combination of an x and y co-ordinates and a letter representing one of the four cardinal compass points.
The plateau is divided up into a grid to simplify navigation. An example position might be 0, 0, N, which means the rover is in the bottom left
corner and facing North.
In order to control a rover, NASA sends a simple string of letters. The possible letters are 'L', 'R' and 'M'. 'L' and 'R' makes the rover spin 90
degrees left or right respectively, without moving from its current spot.
'M' means move forward one grid point, and maintain the same heading.
Assume that the square directly North from (x, y) is (x, y+1).
Input:
The first line of input is the upper-right coordinates of the plateau, the lower-left coordinates are assumed to be 0,0.
The rest of the input is information pertaining to the rovers that have been deployed. Each rover has two lines of input. The first line gives the
rover's position, and the second line is a series of instructions telling the rover how to explore the plateau.
The position is made up of two integers and a letter separated by spaces, corresponding to the x and y co-ordinates and the rover's orientation.
Each rover will be finished sequentially, which means that the second rover won't start to move until the first one has finished moving.
Output:
The output for each rover should be its final co-ordinates and heading.
Test Input:
5 5
1 2 N
LMLMLMLMM
3 3 E
MMRMMRMRRM
Expected Output:
1 3 N
5 1 E

Arguably the task is done after this commit. However:
We're not doing anything with the first line of input, the dimensions of the plateau, apart from ignoring it
We're told this is for rovers on Mars, but so far all we're doing is navigating on a 2D grid.
In addition, we know is that the objective is to photograph the area and that it'll be on a plateau on Mars
We are also told that the rovers will move sequentially but we're not explicitly asked to check for collisions. 

I have made the following assumptions based on this: 
The primary objective is to take pictures ultimately the robots are expendable. 
However, the robots are expensive to make and deploy. Therefore it would be nice if we didn't lose any, say by driving off the edge of a cliff or through a collision.
It would also be better use of resources if they could all move and take photographs simultaneously
With that in mind, I've added some of my own requirements, which will be configurable conditions to the solution:
• Do we allow Rovers to start outside the plateau?
• Do we allow them to travel outside the plateau at all? (maybe they need to take photographs of the target area from just outside)
• Do we allow Rovers to finish outside the plateau? (maybe it's bounded by mountains and they need a clear view of the sky to transmit photo data)
• Can we avoid collisions?
• Can we get them to move simultaneously?

In order to run and test this application:
• Clone the git repository.
• Compile the application.
• Run the executable in the bin directory. If command line arguments are supplied, these will be used otherwise the application will use the data supplied in the brief.
• For example, run this from the command line: `MarsRover "5 5" "1 2 N" "LMLMLMLMM"`,.
• The output will include notifications about rovers straying out of the defined plateau area.
• To experiment with the other settings I have mentioned above, edit the `appSettings.json` file
• To run the "FourRoverFullReportScenario" from the unit tests, set the `RoverMovementType` to `Simultaneous`, compile and run `MarsRover "4 4" "1 2 E" "MMMLM" "5 2 N" "MLMLMRM" "2 0 N" "MMRM" "2 0 N" "RRM"` from the command line

Other assumptions and comments:
• We are using integer coordinates only for a small plateau. Otherwise, spherical polar coordinates would be more appropriate (and that would be challenging!)
• Rover fits well within one grid cell, otherwise more sophisticated collision logic is required
• Input will remain in the same format. A non-primitive input would justify doing away with the ValidationResult and using a dedicated validation framework, e.g. FluentValidation
• I have allowed negatives in the plateau size input, assuming that the origin of the co-ordinate system is an arbitrary corner of the search area
• Rover identifiers are zero indexed, but the steps are 1 indexed. This is to make messages more user friendly, but it does result in the odd +/- 1 here and there. This is less than ideal but as ever, the usability of the application trumps maintainability.
• On reflection the use of `yield return` may not be the best way to go as it means a lot of ToList calls are needed. The logical next step is to use `async` and `await` but this felt like overkill here. 
• I am failing on one of the key requirements here, which is to accept multi-line input. I weighed up the convenience of using a lightweight console app against parsing carriage returns. It should be straight forward enough to split the input by newline characters instead of accepting a collection of strings.
• When running simultaneously, results are reported in the order that rovers finish moving, not the same order as the input. Once again, this is a slight deviation from the requirements. I'm making the assumption that it would be better for a rover to report its position as soon as it has finished moving, so it can accept new route instructions sooner. This would be easy enough to address if the requirement is non-negotiable.
• Once the route execution is added to the PlateauRoverManager, we have some duplication of the code. Although the route execution on the RoverPosition object is no longer used it's still of potential value for this object to be able to evaluate its own navigation, for example if we wanted to upgrade the rovers to communicate with each other as part of a mesh network