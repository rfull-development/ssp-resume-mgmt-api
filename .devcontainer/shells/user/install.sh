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

# Add the following to the .bash_profile file
cat << EOT >> ~/.bash_profile

# set PATH so it includes user's private bin if it exists
if [ -d "\$HOME/.local/bin" ] ; then
    PATH="\$HOME/.local/bin:\$PATH"
fi

# Add .NET tools to the PATH
if [ -d "\$HOME/.dotnet/tools" ]; then
    export PATH="\$PATH:\$HOME/.dotnet/tools"
fi
EOT
exit 0
