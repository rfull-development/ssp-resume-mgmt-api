// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using System.Data;
using System.Data.Common;

namespace ResumeManagementApi.Databases.Extensions
{
    public static class ConnectionExtension
    {
        public static async Task OpenIfClosedAsync(this DbConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                return;
            }
            await connection.OpenAsync();
        }
    }
}
