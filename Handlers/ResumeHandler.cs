// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using Microsoft.AspNetCore.Mvc;
using ResumeManagementApi.Adapters;
using ResumeManagementApi.Databases.Clients.Postgres;
using ResumeManagementApi.Databases.Exceptions;
using ResumeManagementApi.Models;

namespace ResumeManagementApi.Handlers
{
    public static class ResumeHandler
    {
        public static void AddResumeHandler(this WebApplication app)
        {
            var resumes = app.MapGroup("/resumes");
            resumes.MapGet("/", GetResumeListAsync);
            resumes.MapPost("/", CreateResumeAsync);
            resumes.MapGet("/{id}", GetResumeAsync);
            resumes.MapDelete("/{id}", DeleteResumeAsync);

            var skills = app.MapGroup("/resumes/{id}/skills");
            skills.MapGet("/", GetSkillListAsync);
            skills.MapPost("/", CreateSkillAsync);
            skills.MapGet("/{skillId}", GetSkillAsync);
            skills.MapPatch("/{skillId}", UpdateSkillAsync);
            skills.MapDelete("/{skillId}", DeleteSkillAsync);

            var usages = app.MapGroup("/resumes/{id}/skills/{skillId}/usages");
            usages.MapGet("/", GetUsageListAsync);
            usages.MapPut("/", UpdateUsageListAsync);
            usages.MapDelete("/", DeleteUsageListAsync);

            var learnings = app.MapGroup("/resumes/{id}/skills/{skillId}/learnings");
            learnings.MapGet("/", GetLearningListAsync);
            learnings.MapPut("/", UpdateLearningListAsync);
            learnings.MapDelete("/", DeleteLearningListAsync);
        }

        private static async Task<IResult> GetResumeListAsync(
            [FromQuery(Name = "start-id")] string? startId,
            [FromQuery(Name = "limit")] int limit = 20
            )
        {
            List<Models.Resume>? resumes;
            long totalCount;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                resumes = await adapter.GetListAsync(startId, limit);
                totalCount = await adapter.GetTotalCountAsync();
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            Models.ResumeListGetResponse response = new()
            {
                TotalCount = totalCount,
                Count = resumes.Count,
                Resumes = resumes
            };
            return Results.Ok(response);
        }

        private static async Task<IResult> CreateResumeAsync(
            [FromBody] Models.ResumeCreateRequest createRequest
            )
        {
            string id;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                id = await adapter.CreateAsync(createRequest.UserId);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseConflictException)
            {
                return Results.Conflict();
            }
            return Results.Created($"/resumes/{id}", null);
        }

        private static async Task<IResult> GetResumeAsync(
            [FromRoute(Name = "id")] string id
            )
        {
            Models.Resume? resume;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                resume = await adapter.GetAsync(id);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            if (resume is null)
            {
                return Results.NotFound();
            }
            Models.ResumeGetResponse response = new()
            {
                Id = resume.Id,
                Skills = resume.Skills
            };
            return Results.Ok(response);
        }

        private static async Task<IResult> DeleteResumeAsync(
            [FromRoute(Name = "id")] string id
            )
        {
            bool success;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                success = await adapter.DeleteAsync(id);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            if (!success)
            {
                return Results.NotFound();
            }
            return Results.Accepted();
        }

        private static async Task<IResult> GetSkillListAsync(
            [FromRoute(Name = "id")] string id,
            [FromQuery(Name = "start-id")] string? startId,
            [FromQuery(Name = "limit")] int limit = 20
            )
        {
            List<Models.Skill>? skills;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                skills = await adapter.GetSkillListAsync(id);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            if (skills is null)
            {
                return Results.NotFound();
            }
            Models.SkillListGetResponse response = new()
            {
                Count = skills.Count,
                Skills = skills
            };
            return Results.Ok(response);
        }

        private static async Task<IResult> CreateSkillAsync(
            [FromRoute(Name = "id")] string id,
            [FromBody] Models.SkillCreateRequest createRequest
            )
        {
            long skillId;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                skillId = await adapter.CreateSkillAsync(id, createRequest.SkillId);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseConflictException)
            {
                return Results.Conflict();
            }

            SkillCreateResponse response = new()
            {
                Id = skillId.ToString()
            };
            return Results.Created($"/resumes/{id}/skills/{skillId}", response);
        }

        private static async Task<IResult> GetSkillAsync(
            [FromRoute(Name = "id")] string id,
            [FromRoute(Name = "skillId")] string skillId
            )
        {
            Models.Skill? skill;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                skill = await adapter.GetSkillAsync(id, skillId);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            if (skill is null)
            {
                return Results.NotFound();
            }
            SkillGetResponse response = new()
            {
                Id = skill.Id,
                SkillId = skill.SkillId,
                Usages = skill.Usages,
                Learnings = skill.Learnings
            };
            return Results.Ok(response);
        }

        private static async Task<IResult> UpdateSkillAsync(
            [FromRoute(Name = "id")] string id,
            [FromRoute(Name = "skillId")] string skillId,
            [FromBody] Models.SkillUpdateRequest updateRequest
            )
        {
            bool success;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                success = await adapter.SetSkillAsync(id, skillId, updateRequest.SkillId);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseConflictException)
            {
                return Results.Conflict();
            }
            if (!success)
            {
                return Results.NotFound();
            }
            return Results.Accepted();
        }

        private static async Task<IResult> DeleteSkillAsync(
            [FromRoute(Name = "id")] string id,
            [FromRoute(Name = "skillId")] string skillId
            )
        {
            bool success;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                success = await adapter.DeleteSkillAsync(id, skillId);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            if (!success)
            {
                return Results.NotFound();
            }
            return Results.Accepted();
        }

        private static async Task<IResult> GetUsageListAsync(
            [FromRoute(Name = "id")] string id,
            [FromRoute(Name = "skillId")] string skillId,
            [FromQuery(Name = "start-id")] string? startId,
            [FromQuery(Name = "limit")] int limit = 20
            )
        {
            List<SkillUsage>? usages;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                usages = await adapter.GetUsageListAsync(id, skillId);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            if (usages is null)
            {
                return Results.NotFound();
            }
            SkillUsageListGetResponse response = new()
            {
                Count = usages.Count,
                Usages = usages,
            };
            return Results.Ok(response);
        }

        private static async Task<IResult> UpdateUsageListAsync(
            [FromRoute(Name = "id")] string id,
            [FromRoute(Name = "skillId")] string skillId,
            [FromBody] Models.SkillUsageListUpdateRequest updateRequest
            )
        {
            bool success;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                success = await adapter.SetUsageListAsync(id, skillId, updateRequest.Usages);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseConflictException)
            {
                return Results.Conflict();
            }
            if (!success)
            {
                return Results.NotFound();
            }
            return Results.Accepted();
        }

        private static async Task<IResult> DeleteUsageListAsync(
            [FromRoute(Name = "id")] string id,
            [FromRoute(Name = "skillId")] string skillId
            )
        {
            bool success;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                success = await adapter.DeleteUsageListAsync(id, skillId);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            if (!success)
            {
                return Results.NotFound();
            }
            return Results.Accepted();
        }

        private static async Task<IResult> GetLearningListAsync(
            [FromRoute(Name = "id")] string id,
            [FromRoute(Name = "skillId")] string skillId,
            [FromQuery(Name = "start-id")] string? startId,
            [FromQuery(Name = "limit")] int limit = 20
            )
        {
            List<SkillLearning>? learnings;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                learnings = await adapter.GetLearningListAsync(id, skillId);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            if (learnings is null)
            {
                return Results.NotFound();
            }
            SkillLearningListGetResponse response = new()
            {
                Count = learnings.Count,
                Learnings = learnings
            };
            return Results.Ok(response);
        }

        private static async Task<IResult> UpdateLearningListAsync(
            [FromRoute(Name = "id")] string id,
            [FromRoute(Name = "skillId")] string skillId,
            [FromBody] Models.SkillLearningListUpdateRequest updateRequest
            )
        {
            bool success;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                success = await adapter.SetLearningListAsync(id, skillId, updateRequest.Learnings);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseConflictException)
            {
                return Results.Conflict();
            }
            if (!success)
            {
                return Results.NotFound();
            }
            return Results.Accepted();
        }

        private static async Task<IResult> DeleteLearningListAsync(
            [FromRoute(Name = "id")] string id,
            [FromRoute(Name = "skillId")] string skillId
            )
        {
            bool success;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateResumeDatabaseClient();
                ResumeAdapter adapter = new(databaseClient);
                success = await adapter.DeleteLearningListAsync(id, skillId);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            if (!success)
            {
                return Results.NotFound();
            }
            return Results.Accepted();
        }
    }
}
