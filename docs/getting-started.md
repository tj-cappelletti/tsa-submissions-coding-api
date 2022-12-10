# Getting Started
In order to get started working with the API, there are several perquisites you will need to ensure are setup and ready to go.
If you plan on making changes to the code or wish to contribute to the project, it is recommended that you create a fork of this repo first.

## .NET
Once you have the source code locally on your machine, we need to make sure you have the right .NET SDK installed on your machine.
You will need to make sure you have .NET 7.0 installed on your machine.
To verify this, run the following command:

```shell
# An example of how to check on a Windows based machine
PS> dotnet --list-sdks
7.0.100 [C:\Program Files\dotnet\sdk]
```

Depending on your development environment, you may have a newer patch version that listed in the example above.
As long as you have `7.0.x` installed, you should have no issues with running the code.

## Self-signed Certificates
This project uses IdentityServer to manage user authentication and authorization.
Due to the security requirements of IdentityServer, this means all traffic must be done via TLS.
This requirement only effects two of the containers; IdentityServer and Web API.
If you use the default .NET self-signed certificate for TLS between the two containers, you will run into a certificate error because the container does not trust the self-signed certificate.
The solution to this is to generate our own root CA certificate and generate certificates from that.

In the `scripts` folder is a PowerShell script that will handle this activity. Please note that while this script may work on Linux or MacOS, it has only been tested on Windows at this time. To generate the certificates, navigate to root of the repository and run the following as an administrator:

```shell
PS> ./scripts/New-SelfSignedTsaCertificates.ps1
```

This script generates a root CA certificate with the common name of `tsa.localdev.me` and attempts to store in the current users's local machine certificates. The intent here is to have it trusted for development, but this does not currently work as expected. Don't worry, this does not cause any issues when running the application. Once the root CA is created, it will generate three TLS certificates

- `api.tsa.localdev.me` - Certificate for the Web API application
- `identity.tsa.localdev.me` - Certificate for the IdentityServer
- `submissions.tsa.localdev.me` - Certificate for the UI (not currently implemented yet)

These certificates are then placed in the `src/docker/certs` folder.
This folder is mounted to the IdentityServer and Web API containers at the `/https` with read and write operations.
When the containers start up, they look for the environment variable `DOCKER_CONTAINER` set to `Y` and if detected, it spans a background process to run `update-ca-certificates` on the container.
This will scan the `/https` directory and import any root CA certificates discovered there.
Now our self-signed certificates are trusted by the containers.

Take not that since these certificates are stored locally and not in the Git repository, this means that anytime you clone this repo or run a command like `git clean -fdx`, you will need to execute the PowerShell script to ensure the certifcates are setup locally prior to running the application.

## Docker
While not necessarily required, Docker Desktop will help make your initial setup easier.
If you opt to not use Docker Desktop and wish to run the API in a container or run any of the dependencies in a container, you will need an alternative that provides replacements for `docker build` and `docker-compose up`.

This project uses the `localdev.me` domain to serve all of the content.
This domain is registered with root DNS servers by Amazon AWS and points to your local host IP address, `127.0.0.1`.
This allows us to expose all of the containers in the Docker Compose project via a URL and specific port.
The following services are exposed in the Docker Compose:

- `db.tsa.localdev.me:1433` - This is the SQL Server used by IdentityServer
- `identity.tsa.localdev.me:44301` - This is the IdentityServer used for authentication and authorization
- `mongodb.tsa.localdev.me:27017` - This is the MongoDB server used to store data for the application
- `api.tsa.localdev.me:44300` - This is the Web API project

Setting up this loopback allows for the host headers defined in the Docker Compose files to work both inside and outside the Docker environment.
Port binding is then used to ensure that all services are properly accessible.
This is done to minimize the complexity of the setup and allow the solution to be debuged in a variety of environments and situations.

## Running the API
The developer exeprience is optimized to work with Visual Studio 2019 or later, but any tooling that supports .NET development will work with this project.

### Visual Studio
There are launch profiles defined to allow you to simply hit `F5` to launch the application and being debugging.
The profiles allow for running the API from a console application, IIS Express, or Docker.
If you run from Docker, you will need to make sure that all dependant containers are running as well.
For this reason, there is a Docker Compose project that is set as the default to launch the application.

### Visual Studio Code
*TODO: Write instructions for this IDE*

### CLI
*TODO: Write instructions for CLI*

## Making Your First Call
This project has been setup with Swagger and Swagger UI to allow.
The Swagger UI has been configured to make use of OAuth2 authentication against the TSA IdentityServer container.
Simply launch the Docker Compose and navigate to the Swagger API endpoint (as defined in the `src\Tsa.Submissions.Coding.WebApi\Startup.cs` file).
Simply click the "Authorize" button and select all scopes before logging in.
When prompted, enter credientials that are provided by the TSA IdentityServer or by adding your own.