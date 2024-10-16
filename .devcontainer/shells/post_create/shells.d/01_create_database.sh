#!/bin/bash
# Copyright (c) 2024 RFull Development
# This source code is managed under the MIT license. See LICENSE in the project root.

# Create the database
psql -h postgres -U postgres -f /workspace/schemas/database/resume.sql
