#!/bin/bash
# Copyright (c) 2024 RFull Development
# This source code is managed under the MIT license. See LICENSE in the project root.
LOCALE=$1
sed -i "s/^#\s*$LOCALE/$LOCALE/" /etc/locale.gen
locale-gen
exit 0
