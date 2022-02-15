# TSA Submissions Coding API
This project is part of a larger solution built for the Technology Student Association (TSA) [High School Coding][tsa-hs-competitions] competition.
This API project is designed to be the heart of the solution to automate the testing and evaluation of solutions to coding problems presented to the students.

## Workflow Status

CI/CD Workflow| |
--------|-|
Latest Run|![cicd-badge]
Latest PR|![cicd-pr-badge]
`main` Branch|![cicd-main-badge]

## Project Description
This API project exposes core functionality to both the UI and to tools/utilities that test and evaluate submissions by students.
It is a .NET 6.0 based project designed to run various environments such as Docker, IIS, Kubernetes, and/or Linux.
The project also uses MongoDB for document/database storage and RabbitMQ to send messages for submissions that need to be evaluated.

### Features
*TODO: Fill in as features are implemented* 

## Getting Started
In order to get started working with the API, there are several perquisites you will need to ensure are setup and ready to go.

### .NET
The firs step, as with any project like this is to clone the repo locally.
It is recommended that you fork this repo if you itend on making changes to the code.
As stated earlier, this project is build on .NET 6.0 which means you need to ensure the SDK installed in your development environment.

```shell
# An example of how to check on a Windows based machine
PS> dotnet --list-sdks
6.0.101 [C:\Program Files\dotnet\sdk]
```

### Docker
While not necessarily required, Docker Desktop will help make your initial setup easier.
If you opt to not use Docker Desktop and wish to run the API in a container or run any of the dependencies in a container, you will need an alternative that provide replacements for `docker build` and `docker-compose up`.
If you run the application using Docker Compose, you will need to ensure you have values added to your local `hosts` file or have another solution in place to provide look-ups for the following DNS records:
```
127.0.0.1 api.tsa.local
127.0.0.1 identity.tsa.local
```
Setting up this loopback allows for the host headers defined in the Docker Compose files to work both inside and outside the Docker environment.
Port binding is then used to ensure that all services are properly accessible.
This is done to minimize the complexity of the setup and allow the solution to be debuged in a variety of environments and situations.

### SSL/TLS Certificates
When running the API within a container, the SSL/TLS certificate used is the ASP.NET development certificate.
This is often trusted on your local machine during the first time you launch an ASP.NET application.
This issue arises when you try to access the container using a host header (i.e., `api.tsa.local`) that is not on the certificate itsef.
Since custom host headers are used in the Docker Compose project, this will cause SSL/TLS errors when you try to call the API and when the API tries to verify the JWT that is sent to it during API call.

To address this issue, there is a PowerShell script in the repo call `scripts/New-SelfSignedTsaCertificates.ps1` which will generate self signed certifacts with the correct host headers.
It exports the certificates into the correct formate for the Linux containers to use them.
The Docker Compose YAML files then mount these certificates to the correct location with in the container to make use of them.
Logic was added at the start of the API project to update the Linux container's trusted certificates.
This allows SSL/TLS to work without any errors and thus minimizing "special code" makding its way to production that could introduce security risks.

This means that anytime you clone this repo or run a command like `git clean -fdx`, you will need to execute the PowerShell script to ensure the certifcates are setup locally prior to running the application. Also note, this solution only works for Windows at this time.

```shell
# This only works on Windows and must be run as administrator
PS> ./scripts/New-SelfSignedTsaCertificates.ps1
```

### Running the API
The developer exeprience is optimized to work with Visual Studio 2019 or later, but any tooling that supports .NET development will work with this project.

#### Visual Studio
There are launch profiles defined to allow you to simply hit `F5` to launch the application and being debugging.
The profiles allow for running the API from a console application, IIS Express, or Docker.
If you run from Docker, you will need to make sure that all dependant containers are running as well.
For this reason, there is a Docker Compose project that is set as the default to launch the application.

#### Visual Studio Code
*TODO: Write instructions for this IDE*

#### CLI
*TODO: Write instructions for CLI*

### Making Your First Call
Making your first API call can be a bit of challenge if you are unfamiliar with how to get a JWT from an identity provider.
This API uses the [TSA Identity Server][tsa-identity-server] as the identity provider which is built on IdentityServer4.
Further details on this solution can be found by navigating to the repo that houses the solution.

First step is to ensure you have the [TSA Identity Server][tsa-identity-server] container running, either via `docker run` or `docker-compose up`. If you are running from Visual Studio 2019 or later, lauching the `docker-compose` project will handle this.

Next, you must retrieve a JWT from the identity provider.
The easiest way to achieve this is by using Postman.
Details of how this can be achieved can be found in their documentation [Authorizing requests][postman-auth].

Since the values used by Postman to retrieve your JWT are dependant upon the configuration of the [TSA Identity Server][tsa-identity-server], refer to its documenation for the specific values to use. 

# Contributing
We welcome anyone that would like to volunteer their time and contribute to this project.
In order to contibute, we do require you to adhere to our [Code of Conduct][cod]. Please take a moment to read over this as it is strictly enforced.

Once you agree to our Code of Conduct, take a look at our [Contributing Guidelines][cg].
This will help explain our coding style, testing practices, and process for integrating your changes into our repo and master branch.

## How to Start Contributing
*TODO: Define a way for external contributions*

# Related Projects
- [TSA Identity Server][tsa-identity-server] - The identity server that manages logins across all TSA projects
- [TSA Submissions - Coding UI][tsa-submissions-coding-ui] - The UI that interacts with this API

<!-- BADGES -->
[cicd-badge]: https://github.com/tj-cappelletti/tsa-submissions-coding-api/workflows/ci-cd-workflow/badge.svg "current status"
[cicd-main-badge]: https://github.com/tj-cappelletti/tsa-submissions-coding-api/workflows/ci-cd-workflow/badge.svg?branch=main "main branch status"
[cicd-pr-badge]: https://github.com/tj-cappelletti/tsa-submissions-coding-api/workflows/ci-cd-workflow/badge.svg?event=pull_request "pull request status"

<!-- LINKS -->
[cg]: CONTRIBUTING.md "Contributing Guidelines"
[cod]: CODE_OF_CONDUCT.md "Code of Conduct"
[postman-auth]: https://learning.postman.com/docs/sending-requests/authorization/ "Authorizing requests in Postman"
[tsa-hs-competitions]: https://tsaweb.org/competitions-programs/tsa/high-school-competitions "TSA High School Competitions"
[tsa-identity-server]: https://github.com/tj-cappelletti/tsa-identity-server "TSA Identity Server"
[tsa-submissions-coding-ui]: https://github.com/tj-cappelletti/tsa-submissions-coding-ui "TSA Submissions - Coding UI"
