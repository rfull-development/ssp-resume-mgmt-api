// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using ResumeManagementApi.Databases.Models;

namespace ResumeManagementApi.Databases.Clients
{
    public static class ConfigLoader
    {
        private const string ROOT_PREFIX = "RESUME_MGMT_DB_";

        public static readonly Config Config;

        static ConfigLoader()
        {
            Config = LoadConfig();
        }

        private static Config LoadConfig()
        {
            ConfigurationBuilder builder = new();
            builder.AddEnvironmentVariables(ROOT_PREFIX);
            var configuration = builder.Build();

            string host = configuration["HOST"] ?? string.Empty;
            string portRaw = configuration["PORT"] ?? string.Empty;
            int port = int.Parse(portRaw);
            string sslMode = configuration["SSL_MODE"] ?? string.Empty;
            var pooling = LoadPoolingSection(configuration);
            var account = LoadAccountSection(configuration);
            var database = LoadDatabaseSection(configuration);
            Config config = new()
            {
                Host = host,
                Port = port,
                SslMode = sslMode,
                Pooling = pooling,
                Account = account,
                Database = database
            };
            return config;
        }

        private static Config.PoolingSection? LoadPoolingSection(IConfigurationRoot configuration)
        {
            string minSizeRaw = configuration["POOLING_MIN_SIZE"] ?? string.Empty;
            string maxSizeRaw = configuration["POOLING_MAX_SIZE"] ?? string.Empty;
            if (string.IsNullOrEmpty(minSizeRaw) &&
                string.IsNullOrEmpty(maxSizeRaw))
            {
                return null;
            }
            int minSize = int.Parse(minSizeRaw);
            int maxSize = int.Parse(maxSizeRaw);
            Config.PoolingSection pooling = new()
            {
                MinSize = minSize,
                MaxSize = maxSize
            };
            return pooling;
        }

        private static Config.AccountSection LoadAccountSection(IConfigurationRoot section)
        {
            string username = section["USERNAME"] ?? string.Empty;
            string password = section["PASSWORD"] ?? string.Empty;
            Config.AccountSection account = new()
            {
                Username = username,
                Password = password
            };
            return account;
        }

        private static Config.DatabaseSection LoadDatabaseSection(IConfigurationRoot section)
        {
            string name = section["NAME"] ?? string.Empty;
            string schema = section["SCHEMA"] ?? string.Empty;
            Config.DatabaseSection database = new()
            {
                Name = name,
                Schema = schema
            };
            return database;
        }
    }
}
