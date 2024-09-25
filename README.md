WeathercontrolAPI - README

This is a RESTful web service built using ASP.NET Core 8.0 MVC that fetches weather data from a public API and provides responses to clients based on specific cities and dates. The service allows clients to request weather data from external sources such as OpenWeatherMap and delivers it in a user-friendly format.

Features
Fetch Current Weather: Retrieves current weather data from public weather APIs like OpenWeatherMap.
Request Weather by City and Date: Clients can query weather data for a specific city on a specific date.
Flexible API: Designed to be extensible for additional data sources or endpoints.
Error Handling: Provides meaningful error messages for invalid inputs or unavailable data.
Requirements
To run this project, ensure you have the following:

.NET Core SDK 8.0 or later.
API Key for a weather data provider (e.g., OpenWeatherMap).
NuGet Packages for required components such as Serilog for logging, Swagger for API documentation, and any third-party libraries you use.
How It Works
1. Fetching Weather Data
The service calls a public weather API (e.g., OpenWeatherMap) to fetch real-time weather data. This data is then processed and made available through the service’s endpoints.

2. API Endpoints
GET /api/Weather/{city}/{date}: Fetches weather data for a specific city and date. The date can be in the past or the next few days (within the range supported by the weather API).
Parameters:
city: The name of the city for which weather data is requested (e.g., Belgrade).
date: A specific date in ISO-8601 format (e.g., 2024-09-25T08:44:42Z).
Example:
bash
Copy code
GET /api/Weather/Belgrade/2024-09-25T08:44:42Z

Responses:
200 OK: Returns the weather data for the requested city and date.
400 Bad Request: If the city name is invalid or the date format is incorrect.
404 Not Found: If no weather data is available for the given city/date.
3. Using Swagger for API Documentation
The project integrates Swagger for API documentation and testing. You can test endpoints directly via the Swagger UI.

Access Swagger UI:

https://localhost:port/swagger/index.html
Allows users to interact with the API in a browser-friendly interface.
Displays all available endpoints, methods, and expected request/response formats.
4. Logging with Serilog
The service uses Serilog for structured logging to track API calls and handle error logging efficiently. Logs can be viewed in the console or saved to a file based on configuration.

Setup Instructions
Clone the repository:

git clone https://github.com/dzvezdanovic/weathercontrolAPI.git

Set up your API Key:

Obtain an API key from OpenWeatherMap or any other weather provider.
Store the API key in appsettings.json or as an environment variable.

Access the service:

API base URL: https://localhost:{port}/api/Weather/{city}/{date}.
Swagger UI: https://localhost:{port}/swagger/index.html.
Sample Response


{
  "city": "Belgrade",
  "date": "2024-09-26T08:44:42Z",
  "temperature": 22.3,
  "description": "Clear sky"
}


Error Handling
The service handles various errors, including:

400 Bad Request: For invalid inputs such as wrong city name or incorrectly formatted date.
404 Not Found: If the requested weather data is unavailable.
500 Internal Server Error: For any unexpected errors during API calls or data processing.
Technologies Used
ASP.NET Core 8.0 MVC: For building the REST service.
Serilog: For logging.
Swagger: For API documentation and testing.
OpenWeatherMap API: As the external weather data provider.
Future Improvements
Historical Data: Add support for querying historical weather data.
Additional API Integrations: Support other weather data providers like Weatherstack.
Caching: Implement caching for frequent requests to improve performance.
Authentication: Add API key-based authentication to control access.
License
This project is licensed under the MIT License. See the LICENSE file for details.

Contributions
Contributions, issues, and feature requests are welcome!

