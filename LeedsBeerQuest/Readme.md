## The Challenge
The core data for this challenge is based on pubs around Leeds. The raw data can be found [here](https://drive.google.com/file/d/1o5JTtFUHcBAjH47z4i_eZrFdyXvSzY_S/view?usp=sharing) and is based around the [Leeds Beer Quest](https://datamillnorth.org/dataset/leeds-beer-quest)
The employer would like to build an application using this data, that presents information about pubs in and around Leeds to their staff so that they can choose an appropriate post-work watering hole based on their location, rating and services offered.

This challenge can be attempted in one of 3 ways, depending on core discipline.
• Front end focussed: Use [the API provided] (https://datamillnorth.org/dataset/leeds-beer-quest) (this wraps the above dataset) to build a front end application to present this dataset in a searchable manner to end users
• Back end focussed: Using the above dataset, build an API that can be used by front end applications.
• Infrastructure focussed: Given a backend and frontend (that we can provide), build infrastructure, tooling and/or automation approaches for deploying & managing this system.

Principal Engineers (and senior engineers looking for extra kudos) should attempt aspects of all three challenges in order to demonstrate breadth of knowledge across the disciplines.

## How to use the application:
The application provides functionality for filtering and sorting the raw data as desired
This has been tested on Ubuntu Mate 20.04 only, but it should work on any platform

### Pre-requisites: 
- A computer with git and .net core 6 or above installed. 
- A reasonable amount of computer literacy
- Nice to have: [Postman](https://www.postman.com) application installed or a Postman web client

### Setup & Checks:
- Clone the repository and checkout the leeds-beer-quest branch
- Open a terminal and cd to `/Homework/LeedsBeerQuest`
- Compile: run command `dotnet build`
- Run unit tests: run command `dotnet test`

### Run the application: 
- cd down one more level to `LeedsBeerQuest` (full relative path should be `/Homework/LeedsBeerQuest/LeedsBeerQuest`)
- run command `dotnet run`
- console output should indicate which port the application is running on. Specifically check if it is 7080 or not (if not you'll need to update the steps below)

### Testing the application: 
- Import the postman collection, a json file in the root folder of the LeedsBeerQuest Application. Optionally update the port value in the requests from 7080 to whatever yours is using
- Check the status enpoint returns a 200 response or browse to https://localhost:7080/status and ensure it returns the value "Online"
- Browse to the Swagger endpoint at https://localhost:7080/swagger/index.html to get an idea of how to query the API (not available in release mode)
- Use the other Postman endpoints to check happy / unhappy paths or send requests freestyle to the https://localhost:7080/venues endpoint

## Notes, Design Decisions and Proposed Next Steps:

### General
- I'm treating this as a MVP. I have tried to design it so that it's easy to maintain and add to
- It's a run-of-the-mill http web API that can be queried by anything. If someone is dedicated enough to drinking in Leeds to build a mobile app for that, they can consume this API
- I've tried to make the API's filter and sort functionality relevant to users (always a subjective measure) some of my choices are explained below

### Data:
- The original datasource is obviously no longer being updated. The data is fairly old. As a consequence I have decided to treat it as static data (hard-coded)
- I'm also ignoring the time component in the review dates, this does limit the accuracy of sorting by date although that isn't currently supported

### Errors:
- Any invalid request results in a response that should make clear all the reasons why the result is invalid
- Note on tags: instead of returning a 404 or an empty result if an invalid tag is supplied in the request, I have chosen to identify the culpable tag(s) and return a list of valid tag values. I believe this is a basic courtesy that APIs should provide to clients unless there are reasons not to, e.g. confidentiality concerns

### Results:
- The api allows clients to filter by star values as a straight numeric inclusive comparison: a request for minimum stars of 3 will return all venues that have 3 stars or more
- The api will filter out closed venues by default based on how this challenge was specified
- Tag filtering is strict: if the api gets a request for a jukebox and a dance floor, then it will return only results with both. Future work could include optional looser filtering, which, in the example above, would return any venue with either a jukebox or a dance floor (or both)
- Default coordinates are a location relevant to you. They are only used to evaluate distance (as the crow flies) to venues. Next steps could involve querying Google APIs to get actual walking distance (or driving if you're not planning on drinking)
- Star values are sorted in descending order assuming clients want to visit venues that are "better" by some measure. Venues can also be sorted by name (alphabetically) or by distance (closest first). By default, venues are sorted by Beer Stars. This is a Beer Quest after all!

### Immediate Next Steps:
- Logging/Observability: at this point the only reasons to log are to catch run time errors if the application returns a 500 and to get an idea of how much traffic the api is getting
- In the event of a 500 the application exposes implementation details like stack traces. This should be addressed
- Other people could start maintaining the original data. The RawData respository should be re-purposed to fetch & map (and potentially cache) the raw data, preferably asynchronously