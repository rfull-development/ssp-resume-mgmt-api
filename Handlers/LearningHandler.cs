// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using Microsoft.AspNetCore.Mvc;
using ResumeManagementApi.Adapters;
using ResumeManagementApi.Databases.Clients.Postgres;
using ResumeManagementApi.Databases.Exceptions;
using ResumeManagementApi.Models;

namespace ResumeManagementApi.Handlers
{
    public static class LearningHandler
    {
        public static void AddLearningHandler(this WebApplication app)
        {
            var learnings = app.MapGroup("/learnings");
            learnings.MapGet("/", GetLearningListAsync);
            learnings.MapPost("/", CreateLearningAsync);
            learnings.MapGet("/{id}", GetLearningAsync);
            learnings.MapPatch("/{id}", UpdateLearningAsync);
            learnings.MapDelete("/{id}", DeleteLearningAsync);
        }

        private static async Task<IResult> GetLearningListAsync(
            [FromQuery(Name = "start-id")] string? startId,
            [FromQuery(Name = "limit")] int limit = 20
            )
        {
            List<Models.Learning>? learnings;
            long totalCount;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateLearningDatabaseClient();
                LearningAdapter adapter = new(databaseClient);
                learnings = await adapter.GetListAsync(startId, limit);
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
            Models.LearningListGetResponse response = new()
            {
                TotalCount = totalCount,
                Count = learnings.Count,
                Learnings = learnings
            };
            return Results.Ok(response);
        }

        private static async Task<IResult> CreateLearningAsync(
            [FromBody] Models.LearningCreateRequest createRequest
            )
        {
            string id;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateLearningDatabaseClient();
                LearningAdapter adapter = new(databaseClient);
                id = await adapter.CreateAsync(createRequest.Summary);
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
            LearningCreateResponse response = new()
            {
                Id = id
            };
            return Results.Created($"/learnings/{id}", response);
        }

        private static async Task<IResult> GetLearningAsync(
            [FromRoute(Name = "id")] string id
            )
        {
            Models.Learning? learning;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateLearningDatabaseClient();
                LearningAdapter adapter = new(databaseClient);
                learning = await adapter.GetAsync(id);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            if (learning is null)
            {
                return Results.NotFound();
            }
            Models.LearningGetResponse response = new()
            {
                Id = learning.Id,
                Summary = learning.Summary,
                Description = learning.Description
            };
            return Results.Ok(response);
        }

        private static async Task<IResult> UpdateLearningAsync(
            [FromRoute(Name = "id")] string id,
            [FromBody] Models.LearningUpdateRequest updateRequest
            )
        {
            bool success;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateLearningDatabaseClient();
                LearningAdapter adapter = new(databaseClient);
                Models.Learning learning = new()
                {
                    Id = id,
                    Description = updateRequest.Description
                };
                success = await adapter.SetAsync(learning);
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

        private static async Task<IResult> DeleteLearningAsync(
            [FromRoute(Name = "id")] string id
            )
        {
            bool success;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateLearningDatabaseClient();
                LearningAdapter adapter = new(databaseClient);
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
    }
}
