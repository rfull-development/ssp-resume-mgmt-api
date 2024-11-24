// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using Dapper;
using Npgsql;
using ResumeManagementApi.Databases.Exceptions;
using ResumeManagementApi.Databases.Extensions;
using ResumeManagementApi.Databases.Interfaces;
using ResumeManagementApi.Databases.Models;
using System.Text;

namespace ResumeManagementApi.Databases.Clients.Postgres
{
    public class ResumeDatabaseClient(string connectionString, string schema) : DatabaseClient(schema), IResumeDatabaseClient, IDisposable
    {
        private readonly NpgsqlConnection _connection = new(connectionString);

        static ResumeDatabaseClient()
        {
            SqlMapper.SetTypeMap(typeof(Item), new CustomPropertyTypeMap(typeof(Item), CustomMap));
            SqlMapper.SetTypeMap(typeof(ListItem), new CustomPropertyTypeMap(typeof(ListItem), CustomMap));
            SqlMapper.SetTypeMap(typeof(Skill), new CustomPropertyTypeMap(typeof(Skill), CustomMap));
            SqlMapper.SetTypeMap(typeof(Usage), new CustomPropertyTypeMap(typeof(Usage), CustomMap));
            SqlMapper.SetTypeMap(typeof(UsageAlloc), new CustomPropertyTypeMap(typeof(UsageAlloc), CustomMap));
            SqlMapper.SetTypeMap(typeof(UsageAllocListItem), new CustomPropertyTypeMap(typeof(UsageAllocListItem), CustomMap));
            SqlMapper.SetTypeMap(typeof(UsageAllocSegment), new CustomPropertyTypeMap(typeof(UsageAllocSegment), CustomMap));
            SqlMapper.SetTypeMap(typeof(Learning), new CustomPropertyTypeMap(typeof(Learning), CustomMap));
            SqlMapper.SetTypeMap(typeof(LearningAlloc), new CustomPropertyTypeMap(typeof(LearningAlloc), CustomMap));
            SqlMapper.SetTypeMap(typeof(LearningAllocListItem), new CustomPropertyTypeMap(typeof(LearningAllocListItem), CustomMap));
            SqlMapper.SetTypeMap(typeof(LearningAllocSegment), new CustomPropertyTypeMap(typeof(LearningAllocSegment), CustomMap));
        }

        public async Task<long> CreateAsync(Guid guid)
        {
            await _connection.OpenIfClosedAsync();
            using var transaction = await _connection.BeginTransactionAsync();

            object? result;
            try
            {
                string table = GetTableName<Item>();
                string query = $"""
                INSERT INTO
                    {table} ("guid")
                VALUES
                    (@Guid)
                ON CONFLICT DO NOTHING
                RETURNING
                    "id";
                """;
                DynamicParameters parameters = new();
                parameters.Add("Guid", guid);
                result = await _connection.ExecuteScalarAsync(query, parameters, transaction: transaction);
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to create item.", e);
            }
            if (result is not long)
            {
                throw new DatabaseConflictException();
            }
            long id = (long)result;
            return id;
        }

        public async Task<Item?> GetAsync(Guid guid)
        {
            await _connection.OpenIfClosedAsync();

            Item? item;
            try
            {
                string table = GetTableName<Item>();
                string columns = GenerateColumnListQuery<Item>();
                string query = $"""
                SELECT
                    {columns}
                FROM
                    {table}
                WHERE
                    "guid" = @Guid;
                """;
                DynamicParameters parameters = new();
                parameters.Add("Guid", guid);
                item = await _connection.QueryFirstOrDefaultAsync<Item>(query, parameters);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to get item.", e);
            }
            return item;
        }

        public async Task<int> DeleteAsync(Guid guid)
        {
            await _connection.OpenIfClosedAsync();
            var transaction = await _connection.BeginTransactionAsync();

            int rows;
            try
            {
                string table = GetTableName<Item>();
                string query = $"""
                DELETE FROM {table}
                WHERE
                    "guid" = @Guid;
                """;
                DynamicParameters parameters = new();
                parameters.Add("Guid", guid);
                rows = await _connection.ExecuteAsync(query, parameters, transaction: transaction);
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to delete item.", e);
            }
            return rows;
        }

        public async Task<List<ListItem>> GetListAsync(Guid guid, int limit)
        {
            limit = Math.Min(Math.Max(limit, 1), 128);

            await _connection.OpenIfClosedAsync();

            List<ListItem> items;
            try
            {
                string cte = GetTableName<Item>();
                string table = GetTableName<ListItem>();
                string columns = GenerateColumnListQuery<ListItem>();
                string condition;
                if (guid != Guid.Empty)
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
                            "guid" = @Guid
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
                parameters.Add("Guid", guid);
                parameters.Add("Limit", limit);
                var results = await _connection.QueryAsync<ListItem>(query, parameters);
                items = results.AsList();
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to get resumes.", e);
            }
            return items;
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
                    relname = 'item';
                """;
                count = await _connection.ExecuteScalarAsync<long?>(query) ?? 0;
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to get total count.", e);
            }
            return count;
        }

        public async Task<int> CreateSkillAsync(long itemId, string skillId)
        {
            await _connection.OpenIfClosedAsync();
            using var transaction = await _connection.BeginTransactionAsync();

            object? result;
            try
            {
                string table = GetTableName<Skill>();
                string query = $"""
                INSERT INTO
                    {table} ("item_id", "row_id", "skill_id")
                VALUES
                    (
                        @ItemId,
                        COALESCE(
                            (
                                SELECT
                                    MAX(row_id) + 1
                                FROM
                                    public.skill
                                WHERE
                                    "item_id" = @ItemId
                            ),
                            1
                        ),
                        @SkillId
                    )
                ON CONFLICT DO NOTHING
                RETURNING
                    "row_id";
                """;
                DynamicParameters parameters = new();
                parameters.Add("ItemId", itemId);
                parameters.Add("SkillId", skillId);
                result = await _connection.ExecuteScalarAsync(query, parameters, transaction: transaction);
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to create skill.", e);
            }
            if (result is not int)
            {
                throw new DatabaseConflictException();
            }
            int rowId = (int)result;
            return rowId;
        }

        public async Task<Skill?> GetSkillAsync(long itemId, int rowId)
        {
            await _connection.OpenIfClosedAsync();

            Skill? skill;
            try
            {
                string table = GetTableName<Skill>();
                string columns = GenerateColumnListQuery<Skill>();
                string query = $"""
                SELECT
                    {columns}
                FROM
                    {table}
                WHERE
                "item_id" = @ItemId
                AND "row_id" = @RowId;
                """;
                DynamicParameters parameters = new();
                parameters.Add("ItemId", itemId);
                parameters.Add("RowId", rowId);
                skill = await _connection.QueryFirstOrDefaultAsync<Skill>(query, parameters);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to get skill.", e);
            }
            return skill;
        }

        public async Task<int> UpdateSkillAsync(Skill skill)
        {
            string? skillId = skill.SkillId;
            if ((skill.Id is not long id) ||
                string.IsNullOrEmpty(skillId))
            {
                throw new DatabaseParameterException();
            }

            await _connection.OpenIfClosedAsync();
            using var transaction = await _connection.BeginTransactionAsync();

            int rows;
            try
            {
                string table = GetTableName<Skill>();
                string query = $"""
                UPDATE
                    {table}
                SET
                    "skill_id" = @SkillId
                WHERE
                    "id" = @Id;
                """;
                DynamicParameters parameters = new();
                parameters.Add("Id", id);
                parameters.Add("SkillId", skillId);
                rows = await _connection.ExecuteAsync(query, parameters, transaction: transaction);
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to update skill.", e);
            }
            return rows;
        }

        public async Task<int> DeleteSkillAsync(long id)
        {
            await _connection.OpenIfClosedAsync();
            var transaction = await _connection.BeginTransactionAsync();

            int rows;
            try
            {
                string table = GetTableName<Skill>();
                string query = $"""
                DELETE FROM {table}
                WHERE
                    "id" = @Id;
                """;
                DynamicParameters parameters = new();
                parameters.Add("Id", id);
                rows = await _connection.ExecuteAsync(query, parameters, transaction: transaction);
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to delete skill.", e);
            }
            return rows;
        }

        public async Task<List<Skill>> GetSkillListAsync(long itemId)
        {
            await _connection.OpenIfClosedAsync();

            List<Skill> skills;
            try
            {
                string table = GetTableName<Skill>();
                string columns = GenerateColumnListQuery<Skill>();
                string query = $"""
                SELECT
                    {columns}
                FROM
                    {table}
                WHERE
                    "item_id" = @ItemId
                ORDER BY
                    "id" ASC;
                """;
                DynamicParameters parameters = new();
                parameters.Add("ItemId", itemId);
                var results = await _connection.QueryAsync<Skill>(query, parameters);
                skills = results.AsList();
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to get skills.", e);
            }
            return skills;
        }

        public async Task<List<Usage>> GetUsageListAsync(long skillId)
        {
            await _connection.OpenIfClosedAsync();

            List<Usage> usages;
            try
            {
                string table = GetTableName<UsageAllocListItem>();
                string columns = GenerateColumnListQuery<Usage>([
                    nameof(Usage.Version)
                    ]);
                string query = $"""
                SELECT
                    {columns}
                FROM
                    {table}
                WHERE
                    "skill_id" = @SkillId
                ORDER BY
                    "id" ASC;
                """;
                DynamicParameters parameters = new();
                parameters.Add("SkillId", skillId);
                var results = await _connection.QueryAsync<Usage>(query, parameters);
                usages = results.AsList();
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to get usages.", e);
            }
            return usages;
        }

        public async Task<int> SetUsageListAsync(long skillId, List<Usage> usages)
        {
            await _connection.OpenIfClosedAsync();
            using var transaction = await _connection.BeginTransactionAsync();

            UsageAllocSegment? segment;
            try
            {
                segment = await CreateUsageAllocSegmentIfNotExistsAsync(skillId, transaction);
                if (segment?.Version is not int)
                {
                    throw new DatabaseException();
                }
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to get usage segment.", e);
            }

            try
            {
                await DeleteUsageAllocListAsync(skillId, transaction);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to delete old usages.", e);
            }

            int rows;
            if (usages.Count > 0)
            {
                try
                {
                    string table = GetTableName<UsageAlloc>();
                    string segmentTable = GetTableName<UsageAllocSegment>();
                    string columns = GenerateColumnListQuery<UsageAlloc>();
                    StringBuilder builder = new();
                    for (int index = 0; index < usages.Count; index++)
                    {
                        builder.Append($"(@SkillId,@UsageId_{index},@Description_{index}),");
                    }
                    string values = builder.ToString().TrimEnd(',');
                    string query = $"""
                    INSERT INTO
                        {table} ({columns})
                    SELECT
                        l.skill_id,
                        l.usage_id,
                        l.description
                    FROM
                        (
                            VALUES
                                {values}
                        ) AS l (skill_id, usage_id, description)
                        LEFT JOIN {segmentTable} AS ls ON l.skill_id = ls.skill_id
                    WHERE
                        ls.version = @Version;
                    """;
                    DynamicParameters parameters = new();
                    parameters.Add("SkillId", skillId);
                    parameters.Add("Version", segment.Version);
                    for (int index = 0; index < usages.Count; index++)
                    {
                        var usage = usages[index];
                        parameters.Add("SkillId", skillId);
                        parameters.Add($"UsageId_{index}", usage.Id ?? throw new DatabaseParameterException());
                        object? value = (object?)usage.Description ?? DBNull.Value;
                        parameters.Add($"Description_{index}", value);
                    }
                    rows = await _connection.ExecuteAsync(query, parameters, transaction: transaction);
                    if (rows < 1)
                    {
                        throw new DatabaseConflictException();
                    }
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw new DatabaseException("Failed to set usage allocs.", e);
                }
            }
            else
            {
                rows = 0;
            }

            try
            {
                int segmentRows = await UpdateUsageAllocSegmentAsync(segment, transaction);
                if (segmentRows < 1)
                {
                    throw new DatabaseConflictException();
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to update usage alloc segment.", e);
            }
            return rows;
        }

        public async Task<int> DeleteUsageListAsync(long skillId)
        {
            await _connection.OpenIfClosedAsync();
            using var transaction = await _connection.BeginTransactionAsync();

            UsageAllocSegment? segment;
            try
            {
                segment = await GetUsageAllocSegmentAsync(skillId, transaction);
                if (segment == null)
                {
                    return 0;
                }
                if (segment?.Version is not int)
                {
                    throw new DatabaseException();
                }
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to get usage segment.", e);
            }

            int rows;
            try
            {
                rows = await DeleteUsageAllocListAsync(skillId, transaction);
                if (rows < 1)
                {
                    throw new DatabaseConflictException();
                }
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to delete usages.", e);
            }

            try
            {
                int segmentRows = await UpdateUsageAllocSegmentAsync(segment, transaction);
                if (segmentRows < 1)
                {
                    throw new DatabaseConflictException();
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to update usage segment.", e);
            }
            return rows;
        }

        public async Task<List<Learning>> GetLearningListAsync(long skillId)
        {
            await _connection.OpenIfClosedAsync();

            List<Learning> learnings;
            try
            {
                string table = GetTableName<LearningAllocListItem>();
                string columns = GenerateColumnListQuery<Learning>([
                    nameof(Learning.Version)
                    ]);
                string query = $"""
                SELECT
                    {columns}
                FROM
                    {table}
                WHERE
                    "skill_id" = @SkillId
                ORDER BY
                    "id" ASC;
                """;
                DynamicParameters parameters = new();
                parameters.Add("SkillId", skillId);
                var results = await _connection.QueryAsync<Learning>(query, parameters);
                learnings = results.AsList();
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to get learnings.", e);
            }
            return learnings;
        }

        public async Task<int> SetLearningListAsync(long skillId, List<Learning> learnings)
        {
            await _connection.OpenIfClosedAsync();
            using var transaction = await _connection.BeginTransactionAsync();

            LearningAllocSegment? segment;
            try
            {
                segment = await CreateLearningAllocSegmentIfNotExistsAsync(skillId, transaction);
                if (segment?.Version is not int)
                {
                    throw new DatabaseException();
                }
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to get learning segment.", e);
            }

            try
            {
                await DeleteLearningAllocListAsync(skillId, transaction);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to delete old learnings.", e);
            }

            int rows;
            if (learnings.Count > 0)
            {
                try
                {
                    string table = GetTableName<LearningAlloc>();
                    string segmentTable = GetTableName<LearningAllocSegment>();
                    string columns = GenerateColumnListQuery<LearningAlloc>();
                    StringBuilder builder = new();
                    for (int index = 0; index < learnings.Count; index++)
                    {
                        builder.Append($"(@SkillId,@LearningId_{index},@Description_{index}),");
                    }
                    string values = builder.ToString().TrimEnd(',');
                    string query = $"""
                    INSERT INTO
                        {table} ({columns})
                    SELECT
                        l.skill_id,
                        l.learn_id,
                        l.description
                    FROM
                        (
                            VALUES
                                {values}
                        ) AS l (skill_id, learn_id, description)
                        LEFT JOIN {segmentTable} AS ls ON l.skill_id = ls.skill_id
                    WHERE
                        ls.version = @Version;
                    """;

                    DynamicParameters parameters = new();
                    parameters.Add("SkillId", skillId);
                    parameters.Add("Version", segment.Version);
                    for (int index = 0; index < learnings.Count; index++)
                    {
                        var learning = learnings[index];
                        parameters.Add("SkillId", skillId);
                        parameters.Add($"LearningId_{index}", learning.Id ?? throw new DatabaseParameterException());
                        object? value = (object?)learning.Description ?? DBNull.Value;
                        parameters.Add($"Description_{index}", value);
                    }
                    rows = await _connection.ExecuteAsync(query, parameters, transaction: transaction);
                    if (rows < 1)
                    {
                        throw new DatabaseConflictException();
                    }
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw new DatabaseException("Failed to set learningallocs.", e);
                }
            }
            else
            {
                rows = 0;
            }

            try
            {
                int segmentRows = await UpdateLearningAllocSegmentAsync(segment, transaction);
                if (segmentRows < 1)
                {
                    throw new DatabaseConflictException();
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to update learningalloc segment.", e);
            }
            return rows;
        }

        public async Task<int> DeleteLearningListAsync(long skillId)
        {
            await _connection.OpenIfClosedAsync();
            using var transaction = await _connection.BeginTransactionAsync();

            LearningAllocSegment? segment;
            try
            {
                segment = await GetLearningAllocSegmentAsync(skillId, transaction);
                if (segment == null)
                {
                    return 0;
                }
                if (segment?.Version is not int)
                {
                    throw new DatabaseException();
                }
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to get learning segment.", e);
            }

            int rows;
            try
            {
                rows = await DeleteLearningAllocListAsync(skillId, transaction);
                if (rows < 1)
                {
                    throw new DatabaseConflictException();
                }
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to delete learnings.", e);
            }

            try
            {
                int segmentRows = await UpdateLearningAllocSegmentAsync(segment, transaction);
                if (segmentRows < 1)
                {
                    throw new DatabaseConflictException();
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new DatabaseException("Failed to update learning segment.", e);
            }
            return rows;
        }

        private async Task<int> DeleteUsageAllocListAsync(long skillId, NpgsqlTransaction transaction)
        {
            int rows;
            try
            {
                string table = GetTableName<UsageAlloc>();
                string query = $"""
                DELETE FROM {table}
                WHERE
                    "skill_id" = @SkillId;
                """;
                DynamicParameters parameters = new();
                parameters.Add("SkillId", skillId);
                rows = await _connection.ExecuteAsync(query, parameters, transaction: transaction);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to delete usage allocs.", e);
            }
            return rows;
        }

        private async Task<UsageAllocSegment> CreateUsageAllocSegmentIfNotExistsAsync(long skillId, NpgsqlTransaction transaction)
        {
            UsageAllocSegment segment;
            try
            {
                string table = GetTableName<UsageAllocSegment>();
                string query = $"""
                INSERT INTO
                    {table} ("skill_id")
                VALUES
                    (@SkillId)
                ON CONFLICT ("skill_id") DO
                UPDATE
                SET
                    "skill_id" = @SkillId
                RETURNING
                    "version";
                """;
                DynamicParameters parameters = new();
                parameters.Add("SkillId", skillId);
                object? result = await _connection.ExecuteScalarAsync(query, parameters, transaction: transaction);
                if (result is not int version)
                {
                    throw new DatabaseException();
                }
                segment = new UsageAllocSegment
                {
                    SkillId = skillId,
                    Version = version,
                };
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to create usage alloc segment.", e);
            }
            return segment;
        }

        private async Task<UsageAllocSegment?> GetUsageAllocSegmentAsync(long skillId, NpgsqlTransaction transaction)
        {
            await _connection.OpenIfClosedAsync();

            UsageAllocSegment? segment;
            try
            {
                string table = GetTableName<UsageAllocSegment>();
                string columns = GenerateColumnListQuery<UsageAllocSegment>();
                string query = $"""
                SELECT
                    {columns}
                FROM
                    {table}
                WHERE
                    "skill_id" = @SkillId;
                """;
                DynamicParameters parameters = new();
                parameters.Add("SkillId", skillId);
                segment = await _connection.QueryFirstOrDefaultAsync<UsageAllocSegment>(query, parameters, transaction: transaction);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to get usage alloc segment.", e);
            }
            return segment;
        }

        private async Task<int> UpdateUsageAllocSegmentAsync(UsageAllocSegment segment, NpgsqlTransaction transaction)
        {
            int rows;
            try
            {
                string table = GetTableName<UsageAllocSegment>();
                string query = $"""
                UPDATE {table}
                SET
                    "version" = "version" + 1
                WHERE
                    "skill_id" = @SkillId
                    AND "version" = @Version;
                """;
                DynamicParameters parameters = new();
                parameters.AddDynamicParams(segment);
                rows = await _connection.ExecuteAsync(query, parameters, transaction: transaction);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to update usage alloc segment.", e);
            }
            return rows;
        }

        private async Task<int> DeleteLearningAllocListAsync(long skillId, NpgsqlTransaction transaction)
        {
            int rows;
            try
            {
                string table = GetTableName<LearningAlloc>();
                string query = $"""
                DELETE FROM {table}
                WHERE
                    "skill_id" = @SkillId;
                """;
                DynamicParameters parameters = new();
                parameters.Add("SkillId", skillId);
                rows = await _connection.ExecuteAsync(query, parameters, transaction: transaction);

            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to delete learning allocs.", e);
            }
            return rows;
        }

        private async Task<LearningAllocSegment> CreateLearningAllocSegmentIfNotExistsAsync(long skillId, NpgsqlTransaction transaction)
        {
            LearningAllocSegment segment;
            try
            {
                string table = GetTableName<LearningAllocSegment>();
                string query = $"""
                INSERT INTO
                    {table} ("skill_id")
                VALUES
                    (@SkillId)
                ON CONFLICT ("skill_id") DO
                UPDATE
                SET
                    "skill_id" = @SkillId
                RETURNING
                    "version";
                """;
                DynamicParameters parameters = new();
                parameters.Add("SkillId", skillId);
                object? result = await _connection.ExecuteScalarAsync(query, parameters, transaction: transaction);
                if (result is not int version)
                {
                    throw new DatabaseException();
                }
                segment = new LearningAllocSegment
                {
                    SkillId = skillId,
                    Version = version,
                };
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to create learning alloc segment.", e);
            }
            return segment;
        }

        private async Task<LearningAllocSegment?> GetLearningAllocSegmentAsync(long skillId, NpgsqlTransaction transaction)
        {
            await _connection.OpenIfClosedAsync();

            LearningAllocSegment? segment;
            try
            {
                string table = GetTableName<LearningAllocSegment>();
                string columns = GenerateColumnListQuery<LearningAllocSegment>();
                string query = $"""
                SELECT
                    {columns}
                FROM
                    {table}
                WHERE
                    "skill_id" = @SkillId;
                """;
                DynamicParameters parameters = new();
                parameters.Add("SkillId", skillId);
                segment = await _connection.QueryFirstOrDefaultAsync<LearningAllocSegment>(query, parameters, transaction: transaction);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to get learning alloc segment.", e);
            }
            return segment;
        }

        private async Task<int> UpdateLearningAllocSegmentAsync(LearningAllocSegment segment, NpgsqlTransaction transaction)
        {
            int rows;
            try
            {
                string table = GetTableName<LearningAllocSegment>();
                string query = $"""
                UPDATE {table}
                SET
                    "version" = "version" + 1
                WHERE
                    "skill_id" = @SkillId
                    AND "version" = @Version;
                """;
                DynamicParameters parameters = new();
                parameters.AddDynamicParams(segment);
                rows = await _connection.ExecuteAsync(query, parameters, transaction: transaction);
            }
            catch (Exception e)
            {
                throw new DatabaseException("Failed to update learning alloc segment.", e);
            }
            return rows;
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
