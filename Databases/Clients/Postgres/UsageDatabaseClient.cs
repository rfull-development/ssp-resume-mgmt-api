// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using Dapper;
using Npgsql;
using ResumeManagementApi.Databases.Exceptions;
using ResumeManagementApi.Databases.Extensions;
using ResumeManagementApi.Databases.Interfaces;
using ResumeManagementApi.Databases.Models;

namespace ResumeManagementApi.Databases.Clients.Postgres
{
    public class UsageDatabaseClient(string connectionString, string schema) : DatabaseClient(schema), IUsageDatabaseClient, IDisposable
    {
        private readonly NpgsqlConnection _connection = new(connectionString);

        static UsageDatabaseClient()
        {
            SqlMapper.SetTypeMap(typeof(Usage), new CustomPropertyTypeMap(typeof(Usage), CustomMap));
        }

        public async Task<long> CreateAsync(string summary)
        {
            await _connection.OpenIfClosedAsync();
            using var transaction = await _connection.BeginTransactionAsync();

            object? result;
            try
            {
                string table = GetTableName<Usage>();
                string query = $"""
                INSERT INTO
                    {table} ("summary")
                VALUES
                    (@Summary)
                RETURNING
                    "id";
                """;
                DynamicParameters parameters = new();
                parameters.Add("Summary", summary);
                result = await _connection.ExecuteScalarAsync(query, parameters, transaction);
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to create usage.", e);
            }
            if (result is not long)
            {
                throw new DatabaseException("Failed to create usage.");
            }
            long id = (long)result;
            return id;
        }

        public async Task<Usage?> GetAsync(long id)
        {
            await _connection.OpenIfClosedAsync();

            Usage? usage;
            try
            {
                string table = GetTableName<Usage>();
                string columns = GenerateColumnListQuery<Usage>();
                string query = $"""
                SELECT
                    {columns}
                FROM
                    {table}
                WHERE
                    "id" = @Id;
                """;
                DynamicParameters parameters = new();
                parameters.Add("Id", id);
                usage = await _connection.QueryFirstOrDefaultAsync<Usage>(query, parameters);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to get usage.", e);
            }
            return usage;
        }

        public async Task<int> UpdateAsync(Usage usage)
        {
            await _connection.OpenIfClosedAsync();
            using var transaction = await _connection.BeginTransactionAsync();

            int rows;
            try
            {
                string table = GetTableName<Usage>();
                string updateSet = GenerateUpdateSetListQuery(usage, [
                    nameof(Usage.Id),
                    nameof(Usage.Version)
                    ]);
                string query = $"""
                UPDATE {table}
                SET
                    {updateSet},
                    "version" = "version" + 1
                WHERE
                    "id" = @Id
                    AND "version" = @Version;
                """;
                DynamicParameters parameters = new();
                parameters.AddDynamicParams(usage);
                rows = await _connection.ExecuteAsync(query, parameters, transaction);
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to update usage.", e);
            }
            if (rows < 1)
            {
                throw new DatabaseConflictException();
            }
            return rows;
        }

        public async Task<int> DeleteAsync(long id)
        {
            await _connection.OpenIfClosedAsync();
            var transaction = await _connection.BeginTransactionAsync();

            int rows;
            try
            {
                string table = GetTableName<Usage>();
                string query = $"""
                DELETE FROM {table}
                WHERE
                    "id" = @Id;
                """;
                DynamicParameters parameters = new();
                parameters.Add("Id", id);
                rows = await _connection.ExecuteAsync(query, parameters, transaction);
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to delete usage.", e);
            }
            return rows;
        }

        public async Task<List<Usage>> GetListAsync(long id, int limit)
        {
            limit = Math.Min(Math.Max(limit, 1), 128);

            await _connection.OpenIfClosedAsync();

            List<Usage> usages;
            try
            {
                string cte = GetTableName<Usage>();
                string table = GetTableName<Usage>();
                string columns = GenerateColumnListQuery<Usage>();
                string condition;
                if (id != default)
                {
                    condition = """
                        "id" >= (
                            SELECT
                                "id"
                            FROM
                                id_cte
                        )
                    """;
                }
                else
                {
                    condition = "TRUE";
                }
                string query = $"""
                WITH
                    id_cte AS (
                        SELECT
                            "id"
                        FROM
                            {cte}
                        WHERE
                            "id" = @Id
                    )
                SELECT
                    {columns}
                FROM
                    {table}
                WHERE
                    {condition}
                LIMIT
                    @Limit;
                """;
                DynamicParameters parameters = new();
                parameters.Add("Id", id);
                parameters.Add("Limit", limit);
                usages = (await _connection.QueryAsync<Usage>(query, parameters)).ToList();
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to get usages.", e);
            }
            return usages;
        }

        public async Task<long> GetTotalCountAsync()
        {
            await _connection.OpenIfClosedAsync();

            long count;
            try
            {
                string query = $"""
                SELECT
                    n_live_tup
                FROM
                    pg_catalog.pg_stat_user_tables
                WHERE
                    relname = 'usage';
                """;
                DynamicParameters parameters = new();
                object? result = await _connection.ExecuteScalarAsync(query, parameters);
                if (result is not long)
                {
                    throw new DatabaseException("Failed to get total count.");
                }
                count = (long)result;
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to get total count.", e);
            }
            return count;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection.Dispose();
            }
        }
    }
}
