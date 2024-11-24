// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using ResumeManagementApi.Databases.Exceptions;
using ResumeManagementApi.Databases.Helpers;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace ResumeManagementApi.Databases.Clients.Postgres
{
    public abstract class DatabaseClient(string schema)
    {
        private readonly string _schema = schema;

        protected string GetTableName<T>()
        {
            string table = QueryHelper.GetTableName<T>();
            string name = $"{_schema}.{table}";
            return name;
        }

        protected string GenerateColumnListQuery<T>(List<string>? excludeColumns = null)
        {
            List<string> names = QueryHelper.GetColumnNameList<T>(excludeColumns);
            List<string> paramNames = names.Select(name => $"\"{name}\"").ToList();
            string query = string.Join(',', paramNames);
            return query;
        }

        protected string GenerateColumnListQuery<T>(T model, List<string>? excludeColumns = null)
        {
            List<string> names = QueryHelper.GetColumnNameList<T>(model, excludeColumns);
            List<string> paramNames = names.Select(name => $"\"{name}\"").ToList();
            string query = string.Join(',', paramNames);
            return query;
        }

        protected string GenerateInsertValueListQuery<T>(T model, List<string>? excludeColumns = null)
        {
            List<string> names = QueryHelper.GetPropertyNameList(model, excludeColumns);
            List<string> paramNames = names.Select(name => $"@{name}").ToList();
            string query = string.Join(',', paramNames);
            return query;
        }

        protected string GenerateUpdateSetListQuery<T>(T model, List<string>? excludeColumns = null)
        {
            List<KeyValuePair<string, string>> items = QueryHelper.GetColumnList(model, excludeColumns);
            List<string> paramItems = items.Select(item => $"\"{item.Key}\" = @{item.Value}").ToList();
            string query = string.Join(',', paramItems);
            return query;
        }

        protected static PropertyInfo CustomMap(Type type, string columnName)
        {
            var properties = type.GetProperties();
            var property = properties.FirstOrDefault(property =>

            {
                ColumnAttribute? column = property.GetCustomAttribute<ColumnAttribute>();
                bool target = column?.Name == columnName;
                return target;
            });
            return property ?? throw new DatabaseException(columnName);
        }
    }
}
