# Atlas Patient

Atlas Patient is a project designed to check if a patient is registered, retrieve patient data, and register new patients.
Additionally, it fetches data from an external source and ingests it into the local database.
In case of failures during data retrieval, it retries the process multiple times before moving the instance to an error queue.

## Tools

- Visual Studio 17
- .NET Core 7
- RabbitMQ 3
- Docker
- MS SQL Server

## Installation

1. Install MS SQL Server and load the `PatientDB.sql` file.
2. Install Visual Studio and open the project. Build the solution.
3. Install Docker Desktop.
4. Run RabbitMQ via Docker:
    ```bash
    docker run -d --hostname my-rabbitmq --name some-rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management
    ```
5. Update the connection string and RabbitMQ configuration in `appsettings.json` accordingly.

## Usage

1. Run the application.
2. Load the API Swagger endpoint and interact with the application.

Enjoy using the app! 😊
