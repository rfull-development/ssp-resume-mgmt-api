# .NET Project Repository

This repository is a .NET Project.

## Overview

## Requirements

## Container image info

## How to use

### Execution on local machine

1.  Start the application
    ```bash
    dotnet run --project <project-name>
    ```

### Execution on Docker

1.  Start the application
    ```bash
    docker compose up -d
    ```

### Execution on Kubernetes

1. Register the Helm repository
    ```bash
    helm repo add rfull-development http://charts.ngv.jp
    helm repo update
    ```
1. Deploy the application
    ```bash
    helm install rfull-development/<release-name> --namespace <namespace> --create-namespace
    ```
