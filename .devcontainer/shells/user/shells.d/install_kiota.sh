#!/bin/bash
# Copyright (c) 2024 RFull Development
# This source code is managed under the MIT license. See LICENSE in the project root.

# We'll only build amd64 until we can use Docker Hub Automated Builds to build containers using other architectures.
ARCH=$(dpkg --print-architecture)
if [[ "$ARCH" != "amd64" ]]; then
    echo "Kiota is not available"
    exit 0
fi

# Install development tools
# See https://learn.microsoft.com/en-us/openapi/kiota/install?tabs=bash#install-as-net-tool
dotnet tool install --global Microsoft.OpenApi.Kiota
exit 0
