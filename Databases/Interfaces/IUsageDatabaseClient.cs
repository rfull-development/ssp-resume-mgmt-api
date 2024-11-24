// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using ResumeManagementApi.Databases.Models;

namespace ResumeManagementApi.Databases.Interfaces
{
    public interface IUsageDatabaseClient : IDisposable
    {
        public Task<long> CreateAsync(string summary);
        public Task<Usage?> GetAsync(long id);
        public Task<int> UpdateAsync(Usage usage);
        public Task<int> DeleteAsync(long id);
        public Task<List<Usage>> GetListAsync(long id, int limit);
        public Task<long> GetTotalCountAsync();
    }
}
