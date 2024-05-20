# UserSyncingApp

## Overview

UserSyncingApp is a C# self-hosted web application with REST endpoints designed to synchronize user data between a remote API and a local database. It integrates with the API at `https://jsonplaceholder.typicode.com/users` and manages user data with additional synchronization and update functionalities.

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Installation](#installation)
- [Usage](#usage)
- [Configuration](#configuration)
- [Testing](#testing)
- [Contributing](#contributing)
- [License](#license)

## Features

- Synchronize user data from a remote API to a local database
- Handle data insertion and updates
- Generate and display HTTP calls for synchronization
- Predefined actions for updating and deleting user data
- Comprehensive test suite

## Architecture

The project is divided into several key components:

- **UserSyncingApp.Data**: Handles data access and storage.
- **UserSyncingApp.ServiceInterface**: Defines the interfaces for the services.
- **UserSyncingApp.ServiceInterface.Tests**: Contains unit tests for the service interfaces.
- **UserSyncingApp.ServiceModel**: Defines the data models used across the application.
- **UserSyncingApp.ServiceStackServices**: Implements the services using ServiceStack.
- **UserSyncingApp.ServiceStackServices.Tests**: Contains unit tests for the ServiceStack services.

## Getting Started

### Prerequisites

- .NET SDK 8.0
- Visual Studio or JetBrains Rider
- SQL Server, SQLite, or another compatible database

### Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/RDobelis/UserSyncingApp.git
    cd UserSyncingApp
    ```

2. Restore dependencies:
    ```bash
    dotnet restore
    ```

### Configuration

Update the `appsettings.json` file with the correct database connection string and other settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=../UserSyncingApp.Data/users.db"
  },
  "UserDataEndpoint": "https://jsonplaceholder.typicode.com/users",
  "AllowedHosts": "*"
}
```

### Database Setup

Apply migrations to set up the database schema:
```bash
dotnet ef database update
```

## Usage

1. Build the solution:
    ```bash
    dotnet build
    ```

2. Run the application:
    ```bash
    dotnet run --project UserSyncingApp
    ```

## Functionalities

### Sync Remote -> Local

- Reads and saves entries from the API into the local database.
- Adds a column for each entry with a value formatted as `#first letter of firstname# + #lastname# + #@ibsat.com#`.
- Inserts new entries or updates existing ones.

### Sync Local -> Remote

- Reads entries from the local database and checks them against the API.
- Prepares insert and update HTTP calls, which are displayed in the console.

### Update User Data

- Predefined action to update a field in user data, demonstrating differences from API values.

### Remove User

- Predefined action to remove a user from the local database, showcasing a user present in the API but missing locally.

## Testing

The solution includes unit tests for different modules. To run the tests:

```bash
dotnet test
```

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a feature branch: `git checkout -b feature-name`.
3. Commit your changes: `git commit -m 'Add some feature'`.
4. Push to the branch: `git push origin feature-name`.
5. Open a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

