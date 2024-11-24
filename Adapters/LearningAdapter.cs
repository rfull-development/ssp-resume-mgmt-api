// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using ResumeManagementApi.Databases.Interfaces;
using ResumeManagementApi.Models;

namespace ResumeManagementApi.Adapters
{
    public class LearningAdapter(ILearningDatabaseClient client)
    {
        private readonly ILearningDatabaseClient _client = client;

        public async Task<string> CreateAsync(string summary)
        {
            long dbId = await _client.CreateAsync(summary);
            string id = dbId.ToString();
            return id;
        }

        public async Task<Learning?> GetAsync(string id)
        {
            if (!long.TryParse(id, out long dbId))
            {
                throw new ArgumentException(null, nameof(id));
            }

            Databases.Models.Learning? dbItem = await _client.GetAsync(dbId);
            if (dbItem is null)
            {
                return null;
            }

            Learning learning = new()
            {
                Id = id,
                Summary = dbItem.Summary,
                Description = dbItem.Description,
            };
            return learning;
        }

        public async Task<bool> SetAsync(Learning learning)
        {
            if (!long.TryParse(learning.Id, out long dbId))
            {
                throw new ArgumentException(nameof(learning.Id));
            }

            Databases.Models.Learning? dbLearning = await _client.GetAsync(dbId);
            if (dbLearning is null)
            {
                return false;
            }
            dbLearning.Summary = learning.Summary ?? dbLearning.Summary;
            dbLearning.Description = learning.Description ?? dbLearning.Description;

            int rows = await _client.UpdateAsync(dbLearning);
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

        public async Task<List<Learning>> GetListAsync(string? startId, int limit)
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

            List<Databases.Models.Learning> dbLearnings = await _client.GetListAsync(dbId, limit);
            List<Learning> learnings = [];
            foreach (Databases.Models.Learning dbLearning in dbLearnings)
            {
                if (dbLearning.Id is not long dbItemId)
                {
                    throw new InvalidDataException();
                }

                string id = dbItemId.ToString();
                Learning learning = new()
                {
                    Id = id,
                    Summary = dbLearning.Summary,
                    Description = dbLearning.Description,
                };
                learnings.Add(learning);
            }
            return learnings;
        }

        public async Task<long> GetTotalCountAsync()
        {
            long count = await _client.GetTotalCountAsync();
            return count;
        }
    }
}
