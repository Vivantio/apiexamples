# Vivantio API Samples ![Build Status](https://github.com/Vivantio/apiexamples/actions/workflows/dotnet.yml/badge.svg)

This is the home for API code samples for Vivantio. We currently have two main Visual Studio solutions:

- `VivantioApiWeb.sln` - an ASP.NET Core 9 MVC full demo application (see the live application [here](https://vivantioapisampleapp.azurewebsites.net/)).
- `VivantioApiConsole.sln` - a .NET 9 Console Application that demonstrates various data access techniques using `HttpClient`.

## License

All our API samples are published under the MIT Public License; see [License.txt](https://github.com/Vivantio/apisamples/blob/master/License.txt) for further information.

## Getting Started with the API Samples

### Downloading the Samples

You can download the samples as a zip file, or clone the Git repo - whatever works best for you.

### Running the samples

Before you can run the samples, you'll need to create credentials as User environment variables. The settings you need are available from the Vivantio Admin Area under Admin > Integration & API > Downloads. One way of setting User environment variables would be to use this PowerShell script with the appropriate values:

```powershell
[System.Environment]::SetEnvironmentVariable("VIVANTIO_PLATFORM_URL", "<platform url starting with https:// and ending with a trailing />", "User")
[System.Environment]::SetEnvironmentVariable("VIVANTIO_USERNAME", "<username>", "User")
[System.Environment]::SetEnvironmentVariable("VIVANTIO_PASSWORD", "<password>", "User")
```

Once those values are set you should be able to "Run" the samples from within Visual Studio! (Note you may need to restart Visual Studio for the values to be recognised.)

## Additional Resources

- See our [Tutorials repo](https://github.com/Vivantio/apitutorials) that explains how to test our APIs with Postman.
- See our [API Reference](https://webservices-na01.vivantio.com/Help) pages for a catalogue of supported APIs.
