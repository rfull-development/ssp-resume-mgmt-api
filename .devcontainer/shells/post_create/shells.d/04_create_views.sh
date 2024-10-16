#!/bin/bash
# Copyright (c) 2024 RFull Development
# This source code is managed under the MIT license. See LICENSE in the project root.

# Create the views
TARGET_FILES=$(find /workspace/schemas/views -type f -name "*.sql" | sort)
for file in $TARGET_FILES; do
    psql -h postgres -d resume -U postgres -f "$file"
    if [ $? -ne 0 ]; then
        echo "Failed to execute $file"
        exit 1
    fi
done
exit 0
