# Resume Management API Component Test

This project contains the component tests for the Resume Management API.

## Overview

## Requirements

- .NET 8.0
- Kiota

## How to use

### Execution on local machine

1.  Start the application
    ```bash
    dotnet run ResumeManagementApi.csproj
    ```
1.  Run the tests
    ```bash
    dotnet test ResumeManagementApi.sln --filter TestCategory=Component
    ```

## How to maintenance

### Update a API Client

```bash
kiota update --output ./Api/Client
```

## How to development

### Generate a API Client

```bash
kiota generate --openapi ../../schemas/apis/v1.0/openapi.yaml --language csharp --namespace-name Api.Client --class-name ApiClient --output ./Api/Client
```
