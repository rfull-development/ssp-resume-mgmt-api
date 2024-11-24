// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using ResumeManagementApi.Databases.Models;

namespace ResumeManagementApi.Databases.Interfaces
{
    public interface IResumeDatabaseClient : IDisposable
    {
        public Task<long> CreateAsync(Guid guid);
        public Task<Item?> GetAsync(Guid guid);
        public Task<int> DeleteAsync(Guid guid);
        public Task<List<ListItem>> GetListAsync(Guid guid, int limit);
        public Task<long> GetTotalCountAsync();
        public Task<int> CreateSkillAsync(long itemId, string skillId);
        public Task<Skill?> GetSkillAsync(long itemId, int rowId);
        public Task<int> UpdateSkillAsync(Skill skill);
        public Task<int> DeleteSkillAsync(long id);
        public Task<List<Skill>> GetSkillListAsync(long itemId);
        public Task<List<Usage>> GetUsageListAsync(long skillId);
        public Task<int> SetUsageListAsync(long skillId, List<Usage> usages);
        public Task<int> DeleteUsageListAsync(long skillId);
        public Task<List<Learning>> GetLearningListAsync(long skillId);
        public Task<int> SetLearningListAsync(long skillId, List<Learning> learnings);
        public Task<int> DeleteLearningListAsync(long skillId);
    }
}
