// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using ResumeManagementApi.Databases.Interfaces;
using ResumeManagementApi.Models;

namespace ResumeManagementApi.Adapters
{
    public class ResumeAdapter(IResumeDatabaseClient client)
    {
        private readonly IResumeDatabaseClient _client = client;

        public async Task<string> CreateAsync(string userId)
        {
            if (!Guid.TryParse(userId, out Guid dbGuid))
            {
                throw new ArgumentException(null, nameof(userId));
            }

            await _client.CreateAsync(dbGuid);
            return userId;
        }

        public async Task<Resume?> GetAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid dbGuid))
            {
                throw new ArgumentException(null, nameof(id));
            }

            var dbItem = await _client.GetAsync(dbGuid);
            if (dbItem is null)
            {
                return null;
            }
            if (dbItem?.Id is not long dbId)
            {
                throw new InvalidDataException();
            }

            var skills = await GetSkillListAsync(dbId);
            Resume resume = new()
            {
                Id = id,
                Skills = skills
            };
            return resume;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid dbGuid))
            {
                throw new ArgumentException(null, nameof(id));
            }

            int rows = await _client.DeleteAsync(dbGuid);
            bool success = rows > 0;
            return success;
        }

        public async Task<List<Resume>> GetListAsync(string? startId, int limit)
        {
            Guid dbGuid;
            if (startId != null)
            {
                if (!Guid.TryParse(startId, out dbGuid))
                {
                    throw new ArgumentException(null, nameof(startId));
                }
            }
            else
            {
                dbGuid = Guid.Empty;
            }

            var dbItems = await _client.GetListAsync(dbGuid, limit);
            List<Resume> resumes = [];
            foreach (var dbItem in dbItems)
            {
                if (dbItem.Guid is not Guid dbItemGuid)
                {
                    throw new InvalidDataException();
                }

                Resume resume = new()
                {
                    Id = dbItemGuid.ToString()
                };
                resumes.Add(resume);
            }
            return resumes;
        }

        public async Task<long> GetTotalCountAsync()
        {
            long count = await _client.GetTotalCountAsync();
            return count;
        }

        public async Task<List<Skill>?> GetSkillListAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid dbGuid))
            {
                throw new ArgumentException(null, nameof(id));
            }

            var dbItem = await _client.GetAsync(dbGuid);
            if (dbItem is null)
            {
                return [];
            }
            if (dbItem?.Id is not long dbId)
            {
                throw new InvalidDataException();
            }

            var skills = await GetSkillListAsync(dbId);
            return skills;
        }

        public async Task<long> CreateSkillAsync(string resumeId, string skillId)
        {
            if (!Guid.TryParse(resumeId, out Guid dbGuid))
            {
                throw new ArgumentException(null, nameof(resumeId));
            }
            ArgumentException.ThrowIfNullOrEmpty(skillId);

            var dbItem = await _client.GetAsync(dbGuid);
            if (dbItem is null)
            {
                return 0;
            }
            if (dbItem?.Id is not long dbId)
            {
                throw new InvalidDataException();
            }

            long id = await _client.CreateSkillAsync(dbId, skillId);
            return id;
        }

        public async Task<Skill?> GetSkillAsync(string resumeId, string id)
        {
            if (!Guid.TryParse(resumeId, out Guid dbGuid))
            {
                throw new ArgumentException(null, nameof(resumeId));
            }
            if (!int.TryParse(id, out int dbRowId))
            {
                throw new ArgumentException(null, nameof(id));
            }

            var dbItem = await _client.GetAsync(dbGuid);
            if (dbItem is null)
            {
                return null;
            }
            if (dbItem?.Id is not long dbResumeId)
            {
                throw new InvalidDataException();
            }

            var dbSkill = await _client.GetSkillAsync(dbResumeId, dbRowId);
            if (dbSkill is null)
            {
                return null;
            }
            if ((dbSkill.Id is not long dbSkillId) ||
                string.IsNullOrEmpty(dbSkill.SkillId))
            {
                throw new InvalidDataException();
            }
            var usages = await GetUsageListAsync(dbSkillId);
            var learnings = await GetLearningListAsync(dbSkillId);
            Skill skill = new()
            {
                Id = id,
                SkillId = dbSkill.SkillId,
                Usages = usages,
                Learnings = learnings,
            };
            return skill;
        }

        public async Task<bool> SetSkillAsync(string resumeId, string id, string skillId)
        {
            if (!Guid.TryParse(resumeId, out Guid dbGuid))
            {
                throw new ArgumentException(null, nameof(resumeId));
            }
            if (!int.TryParse(id, out int dbRowId))
            {
                throw new ArgumentException(null, nameof(id));
            }

            var dbItem = await _client.GetAsync(dbGuid);
            if (dbItem is null)
            {
                return false;
            }
            if (dbItem?.Id is not long dbResumeId)
            {
                throw new InvalidDataException();
            }
            var dbSkill = await _client.GetSkillAsync(dbResumeId, dbRowId);
            if (dbSkill is null)
            {
                return false;
            }
            dbSkill.SkillId = skillId ?? dbSkill.SkillId;

            int rows = await _client.UpdateSkillAsync(dbSkill);
            bool success = rows > 0;
            return success;
        }

        public async Task<bool> DeleteSkillAsync(string resumeId, string id)
        {
            if (!Guid.TryParse(resumeId, out Guid dbGuid))
            {
                throw new ArgumentException(null, nameof(resumeId));
            }
            if (!int.TryParse(id, out int dbRowId))
            {
                throw new ArgumentException(null, nameof(id));
            }

            var dbItem = await _client.GetAsync(dbGuid);
            if (dbItem is null)
            {
                return false;
            }
            if (dbItem?.Id is not long dbResumeId)
            {
                throw new InvalidDataException();
            }
            var dbSkill = await _client.GetSkillAsync(dbResumeId, dbRowId);
            if (dbSkill?.Id is not long dbSkillId)
            {
                return false;
            }

            int rows = await _client.DeleteSkillAsync(dbSkillId);
            bool success = rows > 0;
            return success;
        }

        public async Task<List<SkillUsage>?> GetUsageListAsync(string resumeId, string id)
        {
            if (!Guid.TryParse(resumeId, out Guid dbGuid))
            {
                throw new ArgumentException(null, nameof(resumeId));
            }
            if (!int.TryParse(id, out int dbRowId))
            {
                throw new ArgumentException(null, nameof(id));
            }

            var dbItem = await _client.GetAsync(dbGuid);
            if (dbItem is null)
            {
                return null;
            }
            if (dbItem?.Id is not long dbId)
            {
                throw new InvalidDataException();
            }
            var dbSkill = await _client.GetSkillAsync(dbId, dbRowId);
            if (dbSkill is null)
            {
                return null;
            }
            if (dbSkill.Id is not long dbSkillId)
            {
                throw new InvalidDataException();
            }

            var usages = await GetUsageListAsync(dbSkillId);
            return usages;
        }

        public async Task<bool> SetUsageListAsync(string resumeId, string id, List<SkillUsageUpdate> usages)
        {
            if (!Guid.TryParse(resumeId, out Guid dbGuid))
            {
                throw new ArgumentException(null, nameof(resumeId));
            }
            if (!int.TryParse(id, out int dbRowId))
            {
                throw new ArgumentException(null, nameof(id));
            }

            var dbItem = await _client.GetAsync(dbGuid);
            if (dbItem is null)
            {
                return false;
            }
            if (dbItem?.Id is not long dbId)
            {
                throw new InvalidDataException();
            }
            var dbSkill = await _client.GetSkillAsync(dbId, dbRowId);
            if (dbSkill is null)
            {
                return false;
            }
            if (dbSkill.Id is not long dbSkillId)
            {
                throw new InvalidDataException();
            }

            List<Databases.Models.Usage> dbUsages = [];
            foreach (var usage in usages)
            {
                if (usage is null)
                {
                    throw new ArgumentException(nameof(usage));
                }
                if (!long.TryParse(usage.Id, out long dbUsageId))
                {
                    throw new ArgumentException(nameof(usage.Id));
                }
                Databases.Models.Usage dbUsage = new()
                {
                    Id = dbUsageId,
                    Description = usage.Description,
                };
                dbUsages.Add(dbUsage);
            }

            int rows = await _client.SetUsageListAsync(dbSkillId, dbUsages);
            bool success = rows > 0;
            return success;
        }

        public async Task<bool> DeleteUsageListAsync(string resumeId, string id)
        {
            if (!Guid.TryParse(resumeId, out Guid dbGuid))
            {
                throw new ArgumentException(null, nameof(resumeId));
            }
            if (!int.TryParse(id, out int dbRowId))
            {
                throw new ArgumentException(null, nameof(id));
            }

            var dbItem = await _client.GetAsync(dbGuid);
            if (dbItem is null)
            {
                return false;
            }
            if (dbItem?.Id is not long dbId)
            {
                throw new InvalidDataException();
            }
            var dbSkill = await _client.GetSkillAsync(dbId, dbRowId);
            if (dbSkill?.Id is not long dbSkillId)
            {
                return false;
            }

            int result = await _client.DeleteUsageListAsync(dbSkillId);
            bool success = result > 0;
            return success;
        }

        public async Task<List<SkillLearning>?> GetLearningListAsync(string resumeId, string id)
        {
            if (!Guid.TryParse(resumeId, out Guid dbGuid))
            {
                throw new ArgumentException(null, nameof(resumeId));
            }
            if (!int.TryParse(id, out int dbRowId))
            {
                throw new ArgumentException(null, nameof(id));
            }

            var dbItem = await _client.GetAsync(dbGuid);
            if (dbItem is null)
            {
                return null;
            }
            if (dbItem?.Id is not long dbId)
            {
                throw new InvalidDataException();
            }
            var dbSkill = await _client.GetSkillAsync(dbId, dbRowId);
            if (dbSkill is null)
            {
                return null;
            }
            if (dbSkill.Id is not long dbSkillId)
            {
                throw new InvalidDataException();
            }

            var learnings = await GetLearningListAsync(dbSkillId);
            return learnings;
        }

        public async Task<bool> SetLearningListAsync(string resumeId, string id, List<SkillLearningUpdate> learnings)
        {
            if (!Guid.TryParse(resumeId, out Guid dbGuid))
            {
                throw new ArgumentException(null, nameof(resumeId));
            }
            if (!int.TryParse(id, out int dbRowId))
            {
                throw new ArgumentException(null, nameof(id));
            }

            var dbItem = await _client.GetAsync(dbGuid);
            if (dbItem is null)
            {
                return false;
            }
            if (dbItem?.Id is not long dbId)
            {
                throw new InvalidDataException();
            }
            var dbSkill = await _client.GetSkillAsync(dbId, dbRowId);
            if (dbSkill is null)
            {
                return false;
            }
            if (dbSkill.Id is not long dbSkillId)
            {
                throw new InvalidDataException();
            }

            List<Databases.Models.Learning> dbLearnings = [];
            foreach (var learning in learnings)
            {
                if (learning is null)
                {
                    throw new ArgumentException(nameof(learning));
                }
                if (!long.TryParse(learning.Id, out long dbLearningId))
                {
                    throw new ArgumentException(nameof(learning.Id));
                }
                Databases.Models.Learning dbLearning = new()
                {
                    Id = dbLearningId,
                    Description = learning.Description,
                };
                dbLearnings.Add(dbLearning);
            }

            int rows = await _client.SetLearningListAsync(dbSkillId, dbLearnings);
            bool success = rows > 0;
            return success;
        }

        public async Task<bool> DeleteLearningListAsync(string resumeId, string id)
        {
            if (!Guid.TryParse(resumeId, out Guid dbGuid))
            {
                throw new ArgumentException(null, nameof(resumeId));
            }
            if (!int.TryParse(id, out int dbRowId))
            {
                throw new ArgumentException(null, nameof(id));
            }

            var dbItem = await _client.GetAsync(dbGuid);
            if (dbItem is null)
            {
                return false;
            }
            if (dbItem?.Id is not long dbId)
            {
                throw new InvalidDataException();
            }
            var dbSkill = await _client.GetSkillAsync(dbId, dbRowId);
            if (dbSkill is null)
            {
                return false;
            }
            if (dbSkill.Id is not long dbSkillId)
            {
                throw new InvalidDataException();
            }

            int result = await _client.DeleteLearningListAsync(dbSkillId);
            bool success = result > 0;
            return success;
        }

        private async Task<List<Skill>> GetSkillListAsync(long dbId)
        {
            var dbSkills = await _client.GetSkillListAsync(dbId);
            List<Skill> skills = [];
            foreach (var dbSkill in dbSkills)
            {
                if (dbSkill.RowId is not int dbRowId)
                {
                    throw new InvalidDataException();
                }

                Skill skill = new()
                {
                    Id = dbRowId.ToString(),
                    SkillId = dbSkill.SkillId ?? string.Empty,
                };
                skills.Add(skill);
            }
            return skills;
        }

        private async Task<List<SkillUsage>> GetUsageListAsync(long dbSkillId)
        {
            var dbUsages = await _client.GetUsageListAsync(dbSkillId);
            List<SkillUsage> usages = [];
            foreach (var dbUsage in dbUsages)
            {
                if (dbUsage.Id is not long dbUsageId)
                {
                    throw new InvalidDataException();
                }

                SkillUsage usage = new()
                {
                    Id = dbUsageId.ToString(),
                    Summary = dbUsage.Summary,
                    Description = dbUsage.Description,
                };
                usages.Add(usage);
            }
            return usages;
        }

        private async Task<List<SkillLearning>> GetLearningListAsync(long dbSkillId)
        {
            var dbLearnings = await _client.GetLearningListAsync(dbSkillId);
            List<SkillLearning> learnings = [];
            foreach (var dbLearning in dbLearnings)
            {
                if (dbLearning.Id is not long dbLearningId)
                {
                    throw new InvalidDataException();
                }

                SkillLearning learning = new()
                {
                    Id = dbLearningId.ToString(),
                    Summary = dbLearning.Summary,
                    Description = dbLearning.Description,
                };
                learnings.Add(learning);
            }
            return learnings;
        }
    }
}
