# TSA Submissions Coding API
This project is part of a larger solution built for the Technology Student Association (TSA) [High School Coding][tsa-hs-competitions] competition.
This API project is designed to be the heart of the solution to automate the testing and evaluation of solutions to coding problems presented to the students.

## Status Badges
| **Badge**          | **Description**                          |
|--------------------| -----------------------------------------|
| ![ci-badge]        | Latest Continuous Integration Run        |
| ![ci-main-badge] | `main` Branch Continuous Integration Run |
| ![codeql-badge]    | Latest `CodeQL` Run                      |
| ![codecove-badge]  | Codecov                                  |
| ![sc-qg-badge]     | SonarCloud Quality Gate - C#             |
| ![sc-m-badge]      | SonarCloud Mantainablity Grade           |
| ![sc-r-badge]      | SonarCloud Reliablity Grade              |
| ![sc-s-badge]      | SonarCloud Security Grade                |
| ![sc-loc-badge]    | Lines of Code                            |

![codecov-graph]

## Project Description
This API project exposes core functionality to both the UI and to tools/utilities that test and evaluate submissions by students.
It is a .NET 7.0 based project designed to run various environments such as Docker, IIS, Kubernetes, and/or Linux.
The project also uses MongoDB for document/database storage and RabbitMQ to send messages for submissions that need to be evaluated.

### Features
*TODO: Fill in as features are implemented* 

## Getting Started
To get up and running with this project, there is a [Getting Started][gs] guide in the `/docs` folder of the repo.
Read this to learn how to quickly get up and running with the application as there are some intricacies to it.

## Contributing
We welcome anyone that would like to volunteer their time and contribute to this project.
In order to contibute, we do require you to adhere to our [Code of Conduct][cod]. Please take a moment to read over this as it is strictly enforced.

Once you agree to our Code of Conduct, take a look at our [Contributing Guidelines][cg].
This will help explain our coding style, testing practices, and process for integrating your changes into our repo and master branch.

## How to Start Contributing
*TODO: Define a way for external contributions*

## Related Projects
- [TSA Identity Server][tsa-identity-server] - The identity server that manages logins across all TSA projects
- [TSA Submissions - Coding UI][tsa-submissions-coding-ui] - The UI that interacts with this API

<!-- BADGES -->
[ci-badge]: https://github.com/tj-cappelletti/tsa-submissions-coding-api/actions/workflows/continuous-integration-workflow.yml/badge.svg "CI status"
[ci-main-badge]: https://github.com/tj-cappelletti/tsa-submissions-coding-api/actions/workflows/continuous-integration-workflow.yml/badge.svg?branch=main "main branch status"
[codeql-badge]: https://github.com/tj-cappelletti/tsa-submissions-coding-api/workflows/CodeQL/badge.svg "current status"

[codecove-badge]: https://codecov.io/gh/tj-cappelletti/tsa-submissions-coding-api/branch/main/graph/badge.svg?token=WDBdfphI5L
[codecov-graph]: https://codecov.io/gh/tj-cappelletti/tsa-submissions-coding-api/branch/main/graphs/sunburst.svg?token=WDBdfphI5L "Code Coverage Graph"

[sc-qg-badge]: https://sonarcloud.io/api/project_badges/measure?project=tj-cappelletti_tsa-submissions-coding-api&metric=alert_status "quality gate status"
[sc-m-badge]: https://sonarcloud.io/api/project_badges/measure?project=tj-cappelletti_tsa-submissions-coding-api&metric=sqale_rating "maintainablity"
[sc-r-badge]: https://sonarcloud.io/api/project_badges/measure?project=tj-cappelletti_tsa-submissions-coding-api&metric=reliability_rating "reliablity"
[sc-s-badge]: https://sonarcloud.io/api/project_badges/measure?project=tj-cappelletti_tsa-submissions-coding-api&metric=security_rating "security"
[sc-loc-badge]: https://sonarcloud.io/api/project_badges/measure?project=tj-cappelletti_tsa-submissions-coding-api&metric=ncloc "lines of code"

<!-- LINKS -->
[cg]: CONTRIBUTING.md "Contributing Guidelines"
[cod]: CODE_OF_CONDUCT.md "Code of Conduct"
[gs]: ./docs/getting-started.md "Getting Started"
[tsa-hs-competitions]: https://tsaweb.org/competitions-programs/tsa/high-school-competitions "TSA High School Competitions"
[tsa-identity-server]: https://github.com/tj-cappelletti/tsa-identity-server "TSA Identity Server"
[tsa-submissions-coding-ui]: https://github.com/tj-cappelletti/tsa-submissions-coding-ui "TSA Submissions - Coding UI"
