// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using ResumeManagementApi.Databases.Models;

namespace ResumeManagementApi.Databases.Interfaces
{
    public interface ILearningDatabaseClient : IDisposable
    {
        public Task<long> CreateAsync(string summary);
        public Task<Learning?> GetAsync(long id);
        public Task<int> UpdateAsync(Learning learning);
        public Task<int> DeleteAsync(long id);
        public Task<List<Learning>> GetListAsync(long id, int limit);
        public Task<long> GetTotalCountAsync();
    }
}
