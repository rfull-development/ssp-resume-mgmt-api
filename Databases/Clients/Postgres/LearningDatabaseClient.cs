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
    public class LearningDatabaseClient(string connectionString, string schema) : DatabaseClient(schema), ILearningDatabaseClient, IDisposable
    {
        private readonly NpgsqlConnection _connection = new(connectionString);

        static LearningDatabaseClient()
        {
            SqlMapper.SetTypeMap(typeof(Learning), new CustomPropertyTypeMap(typeof(Learning), CustomMap));
        }

        public async Task<long> CreateAsync(string summary)
        {
            await _connection.OpenIfClosedAsync();
            using var transaction = await _connection.BeginTransactionAsync();

            object? result;
            try
            {
                string table = GetTableName<Learning>();
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
                throw new DatabaseException("Failed to create learning.", e);
            }
            if (result is not long)
            {
                throw new DatabaseException("Failed to create learning.");
            }
            long id = (long)result;
            return id;
        }

        public async Task<Learning?> GetAsync(long id)
        {
            await _connection.OpenIfClosedAsync();

            Learning? learning;
            try
            {
                string table = GetTableName<Learning>();
                string columns = GenerateColumnListQuery<Learning>();
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
                learning = await _connection.QueryFirstOrDefaultAsync<Learning>(query, parameters);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to get learning.", e);
            }
            return learning;
        }

        public async Task<int> UpdateAsync(Learning learning)
        {
            await _connection.OpenIfClosedAsync();
            using var transaction = await _connection.BeginTransactionAsync();

            int rows;
            try
            {
                string table = GetTableName<Learning>();
                string updateSet = GenerateUpdateSetListQuery(learning, [
                    nameof(Learning.Id),
                    nameof(Learning.Version)
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
                parameters.AddDynamicParams(learning);
                rows = await _connection.ExecuteAsync(query, parameters, transaction);
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to update learning.", e);
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
                string table = GetTableName<Learning>();
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
                throw new DatabaseException("Failed to delete learning.", e);
            }
            return rows;
        }

        public async Task<List<Learning>> GetListAsync(long id, int limit)
        {
            limit = Math.Min(Math.Max(limit, 1), 128);

            await _connection.OpenIfClosedAsync();

            List<Learning> learnings;
            try
            {
                string cte = GetTableName<Learning>();
                string table = GetTableName<Learning>();
                string columns = GenerateColumnListQuery<Learning>();
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
                var results = await _connection.QueryAsync<Learning>(query, parameters);
                learnings = results.AsList();
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to get learnings.", e);
            }
            return learnings;
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
                    relname = 'learn';
                """;
                object? result = await _connection.ExecuteScalarAsync(query);
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
