# Design Document

This document explains how the application is structured, how the conversion works and why this approach was chosen over the alternative options.

## Approach

The application uses a React + TypeScript frontend and an ASP.NET Core backend. The backend is divided into API, Application, and Tests projects.

The structure uses Clean Architecture but only with the layers needed for this application. A database and persistence layer aren't needed as there is no database and no data needs to be stored. This architecture was chosen for it's separation of concerns feature.

Separating HTTP request handling from the conversion rules makes the core functionality easier to test and maintain. Changes to request handling can remain in the API project, while changes to the algorithm can remain in the Application project.

This separation reduces coupling, but changes to validation or conversion results can still affect API behavior and need corresponding tests.

## Project structure

### Client

The Client folder contains the React application, written using TypeScript and Material UI.

It handles the input field, Convert and Clear buttons, converted result, validation messages, and loading state.

React provides a straightforward way to update the interface when these values change. TypeScript adds checks during development for how values and components are used.

A plain HTML and JavaScript interface would also have been sufficient. React introduces additional dependencies and a build step, but provides a clear structure for managing the interface state.

### NumberToWordsConverter.Api

The API project contains the convert endpoint and its request model. It also configures dependency injection, CORS, and OpenAPI with Swagger UI.

A Minimal API was chosen because the application has one endpoint and does not need the additional structure of a controller.

The endpoint is kept in a separate file so that Program.cs stays focused on application configuration. This also makes the request-handling code easier to find without placing the conversion logic inside it.

The endpoint checks for input availability, calls the convert service, and maps its result to an HTTP response.

### NumberToWordsConverter.Application

The Application project contains the convert service, number validator, word constants, and conversion result model.

It does not depend on the API project or HTTP response types. This allows the conversion logic to be tested independently and reused by another caller, such as a command-line application, without requiring an HTTP request.

The API receives the service through its interface using dependency injection. This keeps the endpoint dependent on the service's contract rather than constructing the implementation itself.

The validator is a small concrete class created by the service because that's where conditions of the number related to the business logic are checked. 

### NumberToWordsConverter.Tests

The Tests project contains unit tests for number validation and conversion and API integration tests.

The unit tests check the application's rules and returned words directly. The API tests check request handling, HTTP status codes, and response bodies.

Some invalid inputs appear in both groups because the tests check different responsibilities. A validator test checks whether a rule rejects the number. An API test checks whether that failure is returned to the caller correctly.

## Request and response flow

1. The user enters a number in the frontend.
2. The frontend checks its format, positivity, and supported maximum.
3. The validated text is formatted as a JSON number and sent to POST /api/convert.
4. ASP.NET Core reads the JSON number into the request model's nullable decimal property.
5. The endpoint checks for missing or null input and calls the convert service.
6. The service validates the number and converts it if it is valid.
7. The endpoint returns the converted words or a validation error.
8. The frontend displays the response and clears the loading state.

## Number representation

### Supported range

The task did not specify a maximum supported number, so the limit was selected based on the numeric type used by the convert algorithm.

UInt64 is used for the integer part. Its maximum value is 18446744073709551615, giving a maximum accepted number of 18446744073709551615.99 when cents are included.

The scale constants include names up to quintillion because they cover this range. This is a limit of the selected implementation rather than a universal limit on number conversion. If the program's maximum value needs to change, the type of the input for some of the convert service functions has to be changed and the NotationsMap be adjusted accordingly. 

### Decimal API input

C# decimal was chosen because it can represent the supported numbers and their cents precisely.

Using a decimal request property also allows ASP.NET Core to handle numeric deserialization. Malformed JSON and values that cannot be read as a decimal are rejected before the endpoint runs. 
Whereas if string was used there would be a need for a regular expression and manual decimal parsing in the endpoint.

The property uses decimal? rather than decimal so that missing or null input can be distinguished from an explicitly supplied zero.

Missing input produces “Number is required.” Zero reaches the Application validator and produces “The number must be greater than zero.”

A decimal property does not enforce the business rules by itself. Zero, negative numbers, numbers above the converter's maximum 
and numbers with more than two meaningful decimal places still need validation.

### Preserving values in the frontend

The frontend keeps the entered value as text because JavaScript's ordinary number type cannot preserve all digits of the largest supported numbers.

After validation, the frontend formats the text as a valid JSON number. For example, .25 becomes 0.25 and 001.25 becomes 1.25.

The request body is then constructed from the validated text without converting the number to a JavaScript number. This preserves the digits until ASP.NET Core reads them into a decimal.

The JSON request is constructed only after validation, so only valid numeric text is included.

## Validation

Validation is handled at different points because each has a different purpose.

### Frontend validation

The frontend checks the input format using a regular expression. It rejects letters, grouping commas, scientific notation, a trailing decimal point, and nonzero fractional digits beyond two decimal places.

It also rejects zero, negative numbers, and numbers above the supported maximum. These checks provide immediate feedback and avoid sending requests that are already known to be invalid.

The maximum check uses the normalized integer text. It compares digit counts first and compares the strings when their lengths match. This avoids losing precision because of conversion to a JavaScript number.

Trailing fractional zeros are accepted because they do not change the number. For example, 1.2300 represents the same value as 1.23.

### API input handling

ASP.NET Core handles JSON deserialization. The endpoint then checks whether the number is missing or null.

The endpoint does not repeat the Application validator's business rules. It passes the decimal to the service and maps a failed result to a 400 response.

### Application validation

The convert service calls NumberValidator before performing the conversion.

The validator rejects:

- Zero and negative numbers.
- Numbers whose integer part exceeds UInt64.MaxValue.
- Numbers with more than two meaningful decimal places.

Keeping these rules in the Application project protects the service when it is called independently of the API.

Some frontend checks intentionally overlap with these rules. Frontend validation improves the user experience, while Application validation remains necessary because direct API requests can bypass the frontend.

## Convert algorithm

The service separates the number into its integer and fractional parts, representing dollars and cents. Both parts use the same number-to-words helpers, avoiding separate algorithms for each currency unit.

### Grouping the number

The integer is divided into groups of up to three digits using remainder and division by 1000.

For example, 1234567 is stored as the groups 567, 234, and 1.

This matches the way English scale names progress through thousand, million, billion, and larger groups. It also allows the same small-group conversion logic to handle the full supported range.

Groups are processed from smallest to largest, and their words are inserted at the beginning of the result to restore the original numerical order.

Zero groups are skipped while their positions are retained when selecting scale names. This prevents missing groups from shifting the remaining scale labels.

Arithmetic grouping was chosen instead of formatting the number with separators and splitting the formatted string. It does not depend on the computer's culture settings or choice of grouping separator and doesn't need an unneccessary type change, only to be changed back to integers when being processed.

### Converting each group

A helper converts each group into hundreds, tens, and units, then adds the appropriate scale label.

Word arrays contain the units, numbers below twenty, tens, and scale names. Keeping these words together avoids repeating them throughout the algorithm and makes the available wording easy to review.

Currency labels are also kept together for consistency. Changing the labels is straightforward, although supporting another currency could require additional changes to fractional units and wording rules.

### Output wording

Hyphens are used for numbers such as TWENTY-THREE.

“AND” is used:

- Within hundreds, such as ONE HUNDRED AND FIVE.
- Between dollars and cents.
- Before a final group below one hundred when larger groups exist, such as FIVE MILLION AND ONE.

It is not added between every number group. These wording choices are also in the expected test results to keep the output consistent.

Currency labels are singular for one and plural for larger numbers. Values below one show cents only, and whole numbers show dollars only.

The final result is returned in uppercase.

## Conversion results and error handling

The service returns a ConversionResult containing:

- IsSuccess, indicating whether conversion was successful.
- Words, containing the converted number if successful.
- Error, containing the validation message if unsuccessful.

A structured result was chosen instead of returning null to let the caller know why conversion failed. It also avoids using exceptions for ordinary validation failures.

The result contains no HTTP-specific information. The endpoint decides how to represent it to an API caller.

Successful conversions return 200 OK with the converted words. Missing input and failed validation return 400 Bad Request with an error message.

Requests rejected during JSON deserialization can have a different response body. The frontend tries to read a specific error message and otherwise displays “The number could not be converted.”

Connection failures display a separate message asking the user to try again later.

The loading state is cleared in a finally block so that the controls become available after a request succeeds or fails.

## User interface decisions

The user interface is only focused on entering a number and reading its conversion.

Convert is disabled when the input is empty or while a request is being processed. The input and Clear button are also disabled during a request, preventing the displayed input from changing while its result is being calculated.

Editing the input clears the previous result and error message. This avoids showing converted words that belong to an earlier number.

Clear resets the input, result, and error message. Pressing Enter in the input field starts conversion when Convert is available.

On small screens, the interface fills the page to make use of the available space. On larger screens, it appears as a centered card with a limited width so that the controls do not stretch across the screen.

Long results wrap inside the result section. Vertical scrolling keeps the content accessible on shorter screens.

## API documentation and local configuration

ASP.NET Core generates the OpenAPI document. Swagger UI displays the available endpoint and its input model and allows requests to be tested from the browser.

Swagger UI is enabled in Development. It provides a way to inspect and test the backend without starting the React frontend.

The frontend and API use separate local ports because they run as separate servers. CORS allows the frontend's origin to call the API from the browser.

The frontend uses a fixed port with strictPort enabled. If that port is occupied, it stops rather than silently selecting another port that does not match the API's CORS configuration.

## Alternatives considered

### Keeping all backend code in the API project

This would require fewer projects and would be reasonable for a small application.

Separate API and Application projects were chosen to make the dependency boundary explicit and keep the convert logic independent of HTTP handling. The additional structure provides clearer responsibilities and makes the service easier to reuse.

### Using controllers

Controllers provide a familiar structure for grouping related actions and would support the same separation between request handling and convert logic.

A Minimal API was chosen because there is only one endpoint. Keeping it in a separate file provides organization without requiring a controller.

### Adding more architectural layers

Additional layers and abstractions could have been added, but the application has no database, external service integration or complex domain model.

The architecture was kept limited to the responsibilities currently needed. Additional layers can be added if the requirements provide a concrete reason for them.

### Accepting a string in the API

A string request would preserve the original input text and allow strict checks on its notation before parsing.

Decimal input was chosen to reduce manual parsing in the endpoint and use ASP.NET Core's numeric deserialization. Business validation remains in the Application project.

A string request would not necessarily duplicate business validation, but it would require additional API code to check and parse the text.

### Using FluentValidation

FluentValidation was considered because it was a familiar option for adding validation rules.

However, the task restricts libraries for the core requirements, and it was unclear whether using it for input validation would fall under that restriction. Plain C# validation was chosen to avoid that ambiguity.

The small number of rules also made the validator straightforward to implement without an additional dependency.

## Limitations

The application supports English wording with dollars and cents. Other currencies or languages could require changes beyond replacing the labels.

The projects are organized separately but run as one backend application. Independently deploying or scaling the convert service would require additional work.

The API address and CORS configuration are set for local development and need to be adjusted when deploying elsewhere.