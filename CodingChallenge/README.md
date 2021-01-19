# CodingChallenge
Paymentsense coding challenge

## Setup
  * Please download latest dotnet core(3 +) -> https://dotnet.microsoft.com/download/dotnet-core

# Developer task

## Prerequisite:
  * Clone the repo from https://github.com/Paymentsense/CodingChallenge and inspect the code.
  * Install npm packages for website(ensure @angular/cli is installed globally)
  * Run ng serve to run the website
  * Run the Paymentsense.Coding.Challenge.Api project.

## Provided:
  * Empty API solution,
  * Empty Website code base,
  * Source of data

## Tasks:

Task 1) Successfully connect the Website to the API to receive a green thumbs up.  
Task 2) Through the API make a call to the data source(https://restcountries.eu/). To return the list of country names to the UI.  
Task 3) Cache the call to the data source(https://restcountries.eu/) so the call is not called on every request.  
Task 4) Paginate the list of countries on the UI, displaying the country name and the flag of the country.  
Task 5) Ability to click on a country and display extra information on the country i.e.population, time zones, currencies, language, capital city and bordering countries.  
  
Please ensure that all code is completed to a standard that you would consider production ready - Feel free to change the template provided if you feel necessary  

# Report:
Tasks have been completed and it is clearly indicated where in the commit messages.  
To run the solution I have created, follow the `Prerequisite` steps above. Please ensure that the API project isn't run under IIS or the hard-coded URLs will not work.  
Swagger endpoints:  
[Swagger UI](http://localhost:5001/swagger/index.html)  
[Open Api 3.0.1 Json](http://localhost:5001/swagger/v1/swagger.json)  

## Next steps:
This is still not production ready but it's close
  * I haven't added any logging. I have indicated where it should go for now.
  * I could add serialisation attributes to the C# models. I've ensured that properties names client side match server models, but that not something that should be relied upon.
  * The website is pointing to localhost, this needs to be addressed before this is production ready
  * A generally better user experience for the front end, for example busy/wait indicators. The alert message looks pretty horrible, it could do with a more aesthetic popup (as a back end engineer, I found that part of this quite challenging)
  * Caching - we have one bulk object that holds all the text data (~250kB) This wouldn't scale though. For this type of application with a bigger data set (with potentially larger elements too), the get all countries call should probably return the summary data only. The rest of the data should be cached and then returned only when requested by the button click.
