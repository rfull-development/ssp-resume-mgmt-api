#!/bin/bash
# Copyright (c) 2024 RFull Development
# This source code is managed under the MIT license. See LICENSE in the project root.

# Execute all scripts in the shells.d directory
TARGET_FILES=$(find ./shells.d -type f -name "*.sh" | sort)
for file in $TARGET_FILES; do
    bash "$file"
    if [ $? -ne 0 ]; then
        echo "Failed to execute $file"
        exit 1
    fi
done
exit 0
