This Cinema Booking solution is developed using C# language.
Therefore it needs some pre requirements about environment where this application would be ran.

Prerequisites

Environment:
- .net core installed
- version: .NetCore 8.0
- C#12


NuGetPackages used:
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Logging
- Microsoft.Extensions.Configuration
- NLog


Design
- I have make use of .net core console and class library projects.
- Made the application modular to have benefits of Dependency Injection
- Modular and layer of projects made application loosely coupled and testable
- Usual communication between projects is as below
APP => Service => Domain

Projects
App: console project
- this project is the entry point of application
- used CinemaController to start application to mimic microservice project so later solution can be extended as webApi

Domain: Library project
- This projects includes domain model classes
- Made Cinema as singleton object so entire application would have single instance of cinema
- CinemaAccessor is the unique entry point for application to deal with Cinema object
- Included static Utility and Validator classes to be used across other projects

Service: Library Project
- This projects has service classes which implements the core business logic of application

Testing.UnitTests
- This unit test project test all service and controller classes of application.


Steps to Build and Run application
1. First copy CinemaBookingApplication.zip file which is present inside folder "Application Source Code"
and paste to any local directory of your computer and unzip.
2. Traverse to CinemaBookingApplication folder where you would see App, Domain, Service, UnitTests, .sln and other files files
3. open cmd prompt or powershell 7.0 at and cd to CinemaBookingApplication folder
4. run command "dotnet clean && dotnet build" -> this would build application with no error and warnings
5. Then traverse to "App" folder by using cd App
5. then run command "dotnet run" -> this would start application and ask for user inputs as per the assessment statement

Validation:
- Application has taken care of functional validations
- Also some non specified business functional validations have been added to avoid crashing of application.
- In case of exception application would crash and exception logs are written in log file. To detailed logs, refer Logging section below.


Testing
Unit tests has been written for this application, however i could not finish writing tests for couple of service classes.
- You can run the unit tests by traversing to UnitTests project by traversing to path CinemaBookingApplication\UnitTests
using cd CinemaBookingApplication\Unit Tests
- run command dotnet test
- 55 tests should pass and 3 tests should fail.
- suggest to open unittest project in rider or visual studio and run those failed tests individually
those failed tests should pass again. 

Logging
- Logging of application has been done using NLog library
- Application write logs into file at path "C:\CinemaApplication\Application.log"


Sanity Testing note:
Sanity testing has been done to ensure working of application as per requirements specified in assessment statement.
Gifs proofs of same have been placed under folder "SanityTesting".
Please refer gif "TestingProof.gif" placed at folder "SanityTesting" in case any issues encoutner.