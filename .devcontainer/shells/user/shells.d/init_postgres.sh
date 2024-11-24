#!/bin/bash
# Copyright (c) 2024 RFull Development
# This source code is managed under the MIT license. See LICENSE in the project root.

# Generate the password file
# see https://www.postgresql.jp/docs/9.4/libpq-pgpass.html
echo "postgres:5432:*:postgres:postgres" > ~/.pgpass
chmod 600 ~/.pgpass
exit 0
