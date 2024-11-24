// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
namespace ResumeManagementApi.Databases.Models
{
    public record class Config
    {
        public required string Host { get; init; }
        public required int Port { get; init; }
        public required string SslMode { get; init; }
        public PoolingSection? Pooling { get; init; }
        public required AccountSection Account { get; init; }
        public required DatabaseSection Database { get; init; }

        public record class PoolingSection
        {
            public required int MinSize { get; init; }
            public required int MaxSize { get; init; }
        }

        public record class AccountSection
        {
            public required string Username { get; init; }
            public required string Password { get; init; }
        }

        public record class DatabaseSection
        {
            public required string Name { get; init; }
            public required string Schema { get; init; }
        }
    }
}
