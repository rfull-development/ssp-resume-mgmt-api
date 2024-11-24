// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using ResumeManagementApi.Databases.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace ResumeManagementApi.Databases.Helpers
{
    public static class QueryHelper
    {
        public static string GetTableName<T>()
        {
            var type = typeof(T);
            var table = type.GetCustomAttribute<TableAttribute>();
            string name = table?.Name ?? throw new DatabaseException();
            return name;
        }

        public static List<string> GetPropertyNameList<T>(T model, List<string>? excludeColumns = null)
        {
            excludeColumns ??= [];
            List<string> names = [];
            Type type = typeof(T);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                string name = property.Name;
                if (excludeColumns.Contains(name))
                {
                    continue;
                }
                object? value = property.GetValue(model);
                if (value is null)
                {
                    continue;
                }
                names.Add(name);
            }
            return names;
        }

        public static List<string> GetPropertyNameList<T>(List<string>? excludeColumns = null)
        {
            excludeColumns ??= [];
            List<string> names = [];
            Type type = typeof(T);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                string name = property.Name;
                if (excludeColumns.Contains(name))
                {
                    continue;
                }
                names.Add(name);
            }
            return names;
        }

        public static List<string> GetColumnNameList<T>(T model, List<string>? excludeColumns = null)
        {
            excludeColumns ??= [];
            List<string> names = [];
            Type type = typeof(T);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                string name = property.Name;
                if (excludeColumns.Contains(name))
                {
                    continue;
                }
                object? value = property.GetValue(model);
                if (value is null)
                {
                    continue;
                }
                var column = property.GetCustomAttribute<ColumnAttribute>();
                string columnName = column?.Name ?? throw new DatabaseException();
                names.Add(columnName);
            }
            return names;
        }

        public static List<string> GetColumnNameList<T>(List<string>? excludeColumns = null)
        {
            excludeColumns ??= [];
            List<string> names = [];
            Type type = typeof(T);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                string name = property.Name;
                if (excludeColumns.Contains(name))
                {
                    continue;
                }
                var column = property.GetCustomAttribute<ColumnAttribute>();
                string columnName = column?.Name ?? throw new DatabaseException();
                names.Add(columnName);
            }
            return names;
        }

        public static List<KeyValuePair<string, string>> GetColumnList<T>(T model, List<string>? excludeColumns = null)
        {
            excludeColumns ??= [];
            List<KeyValuePair<string, string>> items = [];
            Type type = typeof(T);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                string name = property.Name;
                if (excludeColumns.Contains(name))
                {
                    continue;
                }
                object? value = property.GetValue(model);
                if (value is null)
                {
                    continue;
                }
                var column = property.GetCustomAttribute<ColumnAttribute>();
                string columnName = column?.Name ?? throw new DatabaseException();
                items.Add(new KeyValuePair<string, string>(columnName, name));
            }
            return items;
        }
    }
}
