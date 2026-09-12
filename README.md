# Number to Words Converter

This is a web application that converts a number into it's currency value in English words, including dollars and cents.
The frontend uses React, TypeScript, and Material UI. The backend uses ASP.NET Core Minimal APIs and a C# conversion algorithm.

Example:
Input: 123.45
Output: ONE HUNDRED AND TWENTY-THREE DOLLARS AND FORTY-FIVE CENTS

## Project structure

Client/                                      
Server/
    NumberToWordsConverter.Api/                 
    NumberToWordsConverter.Application/         
    NumberToWordsConverter.Tests/   
docs/            

the description for each project is as follows:

Client and Server are the two main folders for frontend and backend (in order). Client contains the React application for UI and Server contains the logic of the program.

Server consists of 3 projects each handling a different task:
NumberToWordsConverter.Api manages HTTP endpoints and request handling
NumberToWordsConverter.Application contains the conversion logic and number validation
NumberToWordsConverter.Tests handles unit and API integration tests

docs is the directory containing the design document and test plan.


## How to run

1. To be able to run this program, you first need to install the .NET 10 SDK and Node.js with npm. The version of the packages used in development and testing of this application are as follows:
.NET SDK: 10.0.203
Node.js: 26.1.0
npm: 11.13.0

2. Clone the project onto your computer and open NumberToWordsConverter.slnx via an IDE like Visual Studio/Rider or open the main folder (the root that is containing the Client and the Server folders) in a code editor like Visual Studio Code.

3. For working with the program, the frontend and backend need to be running at the same time. 

First open up a terminal in the IDE and execute the backend in the repository root with the below command. The API listens at 'https://localhost:5001'.
  `dotnet run --project Server/NumberToWordsConverter.Api --launch-profile https`

Then keep this terminal running and open a new one for executing the frontend. From the root directory, head to Client/. There, run the command 'npm install' first to install the frontend dependencies and then run the React app with the below command:
  `npm run dev`

If it doesn't automatically open a browser with the React app running on it, open a browser and head to 'http://localhost:3000'

The frontend uses port 3000 and stops if that port is already occupied. The API's CORS configuration allows this frontend origin.

4. If you want to run the automated tests, execute the command below in a terminal in the root directory of this project. This will run all the tests together. The tests cover amount validation, conversion results, and API request/response behavior.
"dotnet test Server/NumberToWordsConverter.Tests/NumberToWordsConverter.Tests.csproj"

## How to interact with the app

When the web page loads, you can see the 'Convert to Words" button grayed out. It will remain like that until you type in the text field to not send a null or empty value to the API.

You can put in a value and click 'Convert To Words'. If the value is valid, the result would show in the result section. Press Clear to clear out the text field, any error messages that might've shown and the result section.  If you edit the text field after a result is shown, it will automatically reset the result section.

If you put in an invalid value or the react app can't connect to the server, it will show the corresponding error message. Invalid values include letters, zero, negative amounts, grouping commas, scientific notation and a trailing decimal point such as '23.' . 

Valid values are:
  Positive amounts from 0.01 to 18446744073709551615.99.
  Whole amounts and amounts with up to two decimal places.
  Additional trailing fractional zeros, such as 1.2300.
  Amounts below one dollar, such as .25.
  Leading and trailing whitespace is trimmed.


If you want to interact with the app without using the UI, you can simply send an http request with an app like Postman. 

POST https://localhost:5001/api/convert
Content-Type: application/json
body:
{
  "number": "123.45"
}

the response for a valid value like this would be 200 OK with a message like this: "ONE HUNDRED AND TWENTY-THREE DOLLARS AND FORTY-FIVE CENTS"

if you change the body to an invalid value like:
{
  "number": "0"
}

the response would be 400 Bad Request with 
{
  "error": "The number must be greater than zero."
}

Malformed JSON or an incorrect JSON property type also returns `400`.

(The amount is sent as a string to preserve precision for large values in JavaScript. The API parses it into a C# decimal before conversion.)

## Building the app

To build the backend from the repository root execute this command:
  `dotnet build Server/NumberToWordsConverter.Api/NumberToWordsConverter.Api.csproj`

To build the frontend, go to Client/ and after installing its dependencies (with npm install), execute this:
  `npm run build`

The frontend build is generated in Client/dist/. 

The instructions above host the application locally. To deploy elsewhere, the API URL and CORS configuration need to be updated.

## Documentation

docs/DesignDocument.md
docs/TestPlan.md