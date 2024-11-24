// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using ResumeManagementApi.Databases.Interfaces;
using ResumeManagementApi.Models;

namespace ResumeManagementApi.Adapters
{
    public class UsageAdapter(IUsageDatabaseClient client)
    {
        private readonly IUsageDatabaseClient _client = client;

        public async Task<string> CreateAsync(string summary)
        {
            long dbId = await _client.CreateAsync(summary);
            string id = dbId.ToString();
            return id;
        }

        public async Task<Usage?> GetAsync(string id)
        {
            if (!long.TryParse(id, out long dbId))
            {
                throw new ArgumentException(null, nameof(id));
            }

            Databases.Models.Usage? dbItem = await _client.GetAsync(dbId);
            if (dbItem is null)
            {
                return null;
            }

            Usage usage = new()
            {
                Id = id,
                Summary = dbItem.Summary,
                Description = dbItem.Description,
            };
            return usage;
        }

        public async Task<bool> SetAsync(Usage usage)
        {
            if (!long.TryParse(usage.Id, out long dbId))
            {
                throw new ArgumentException(nameof(usage.Id));
            }

            Databases.Models.Usage? dbUsage = await _client.GetAsync(dbId);
            if (dbUsage is null)
            {
                return false;
            }
            dbUsage.Summary = usage.Summary ?? dbUsage.Summary;
            dbUsage.Description = usage.Description ?? dbUsage.Description;

            int rows = await _client.UpdateAsync(dbUsage);
            bool success = rows > 0;
            return success;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (!long.TryParse(id, out long dbId))
            {
                throw new ArgumentException(null, nameof(id));
            }

            int rows = await _client.DeleteAsync(dbId);
            bool success = rows > 0;
            return success;
        }

        public async Task<List<Usage>> GetListAsync(string? startId, int limit)
        {
            long dbId;
            if (startId != null)
            {
                if (!long.TryParse(startId, out dbId))
                {
                    throw new ArgumentException(null, nameof(startId));
                }
            }
            else
            {
                dbId = default;
            }

            List<Databases.Models.Usage> dbUsages = await _client.GetListAsync(dbId, limit);
            List<Usage> usages = [];
            foreach (Databases.Models.Usage dbUsage in dbUsages)
            {
                if (dbUsage.Id is not long dbItemId)
                {
                    throw new InvalidDataException();
                }

                string id = dbItemId.ToString();
                Usage usage = new()
                {
                    Id = id,
                    Summary = dbUsage.Summary,
                    Description = dbUsage.Description,
                };
                usages.Add(usage);
            }
            return usages;
        }

        public async Task<long> GetTotalCountAsync()
        {
            long count = await _client.GetTotalCountAsync();
            return count;
        }
    }
}
