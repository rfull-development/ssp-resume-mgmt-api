#!/bin/bash
# Copyright (c) 2024 RFull Development
# This source code is managed under the MIT license. See LICENSE in the project root.

# Remove the unused user
USER_NAME=ubuntu
UNUSE_UID=`id -u $USER_NAME`
if [ $UNUSE_UID -eq 1000 ]; then
  userdel -r $USER_NAME
fi

# Create a non-root user
USER_NAME=$1
useradd -m $USER_NAME -s /bin/bash
echo "$USER_NAME ALL=(ALL:ALL) NOPASSWD:ALL" > /etc/sudoers.d/$USER_NAME
