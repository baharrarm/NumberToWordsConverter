# Test Plan

This document explains how the application was tested to check that valid numbers are converted correctly, invalid values are rejected, and the frontend displays the results and error messages correctly.

## Automated tests

The project uses xUnit for unit tests and ASP.NET Core's test host for API integration tests. To run all the tests together, execute this command in a terminal in the root directory:

`dotnet test Server/NumberToWordsConverter.Tests/NumberToWordsConverter.Tests.csproj`

The tests are divided into three groups:
1. Number validation tests
2. Convert service tests
3. API integrarion tests.

### Number validation tests

These tests check whether a number can be accepted by the convert service.

Valid values include positive numbers within the supported range (0.01 to 18446744073709551615.99), whole numbers, number below one, and values with trailing fractional zeros such as 1.2300.

The tests also check that zero, negative numbers, numbers above the supported maximum, and values with more than two meaningful decimal places are rejected. Decimal.MinValue and Decimal.MaxValue are included to check that extreme values return an error without causing an exception.

### Convert service tests

These tests check the result of what the convert service returns.

They cover dollars and cents together, dollars-only and cents-only amounts, singular and plural currency labels, and the use of hyphens and “AND”.  “AND” is used within hundreds, between dollars and cents, and before a final group below one hundred when larger groups exist. It is not added between every number group. For example, 5000001 produces “FIVE MILLION AND ONE DOLLARS”, while 100050000 produces “ONE HUNDRED MILLION FIFTY THOUSAND DOLLARS”.

Small numbers and large numbers are tested, including a number whose integer part is UInt64.MaxValue. The tests also check that zero groups (each 3 digit is a group) are skipped without changing the positions of the other groups.

For invalid numbers, the service should return a failed result containing the corresponding error message.

### API integration tests

These tests send HTTP requests to the API through a test host and check the response status and body.

Missing or null input should return 400 Bad Request with “Number is required.”

Malformed JSON, empty or whitespace-only strings, letters, grouping commas, boolean values, and numbers outside C# decimal's range should return 400. These tests check the status without expecting the application's error message format.

Zero, negative numbers, numbers above the supported maximum, and values with more than two meaningful decimal places should return 400 with the corresponding validation message.

Valid requests send the number as a JSON number and should return 200 OK with the expected words. They include ordinary numbers, numbers below one dollar, trailing fractional zeros, and the maximum supported number. The request bodies are written directly as JSON text to preserve all digits.

## Testing the frontend

To check that the frontend builds successfully, run the following command from the Client directory:
    
`npm run build`

To check the frontend for lint errors, run:

`npm run lint`

Both commands completed successfully during verification.

## Manual testing

### Frontend interaction

The frontend was also tested manually through the browser.

The following checks were completed:
1. Convert remains disabled while the text field is empty or contains only whitespace.
2. Entering a valid amount and clicking Convert displays the expected result.
3. Entering an invalid value displays an error message.
4. The frontend format check was tested with letters, grouping commas, scientific notation, and a trailing decimal point such as "23.". These inputs displayed a validation error message before an API request was sent. Valid formats such as .25 and 1.2300 were accepted.
5. The frontend maximum-value check was tested with 18446744073709551615.99, which was accepted, and 18446744073709551616, which displayed “The number is too big.” before a request was sent.
6. Editing the input clears the previous result and error message.
7. Clicking Clear resets the input, result, and error message.
8. While a request is being processed, the button shows “Converting...” and both buttons and text field are disabled.
9. If the API isn't running, a connection error is displayed and the button and text field get enabled again.
10. When the input is not empty and no request is already running, pressing Enter in the text field starts the conversion.
11. Tab moves between the enabled controls, and Enter activates the selected button.

### Direct API checks

Malformed JSON and a non-numeric value were tested through Postman while the API was running in Development. Both responses contained plain-text exception details from ASP.NET Core rather than the application's JSON error message format.


## Responsive layout testing

The layout was checked using the browser's responsive device toolbar.

The widths 375px, 599px, 600px, and 1024px were checked at a height of 608px.

The following sizes were also checked:
- 320 × 568
- 667 × 375
- 1024 × 400
- 1920 × 1080
- 2560 × 1440
- 3840 × 2160
- 3440 × 1440

On small screens, the interface fills the page instead of appearing as a separate card. On larger screens, the card stays centered and keeps a limited width.

In these checks, buttons wrapped when necessary, error messages remained readable, and long results stayed inside the result section. When the content was taller than the screen, it could be reached by scrolling vertically.

The page was also checked at 200% browser zoom. Text and controls remained readable, and keyboard navigation worked.

## Results

The latest recorded backend test run completed with 59 tests passed and no failures.

The frontend build and lint checks completed successfully. The manual functional checks and responsive checks described above also passed.

## Limitations

Responsive testing was done through browser emulation rather than on different physical devices.

Frontend interactions were tested manually. There are no automated frontend tests.

