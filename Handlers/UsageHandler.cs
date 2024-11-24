// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using Microsoft.AspNetCore.Mvc;
using ResumeManagementApi.Adapters;
using ResumeManagementApi.Databases.Clients.Postgres;
using ResumeManagementApi.Databases.Exceptions;
using ResumeManagementApi.Models;

namespace ResumeManagementApi.Handlers
{
    public static class UsageHandler
    {
        public static void AddUsageHandler(this WebApplication app)
        {
            var usages = app.MapGroup("/usages");
            usages.MapGet("/", GetUsageListAsync);
            usages.MapPost("/", CreateUsageAsync);
            usages.MapGet("/{id}", GetUsageAsync);
            usages.MapPatch("/{id}", UpdateUsageAsync);
            usages.MapDelete("/{id}", DeleteUsageAsync);
        }

        private static async Task<IResult> GetUsageListAsync(
            [FromQuery(Name = "start-id")] string? startId,
            [FromQuery(Name = "limit")] int limit = 20
            )
        {
            List<Models.Usage>? usages;
            long totalCount;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateUsageDatabaseClient();
                UsageAdapter adapter = new(databaseClient);
                usages = await adapter.GetListAsync(startId, limit);
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
            Models.UsageListGetResponse response = new()
            {
                TotalCount = totalCount,
                Count = usages.Count,
                Usages = usages
            };
            return Results.Ok(response);
        }

        private static async Task<IResult> CreateUsageAsync(
            [FromBody] Models.UsageCreateRequest createRequest
            )
        {
            string id;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateUsageDatabaseClient();
                UsageAdapter adapter = new(databaseClient);
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
            UsageCreateResponse response = new()
            {
                Id = id
            };
            return Results.Created($"/usages/{id}", response);
        }

        private static async Task<IResult> GetUsageAsync(
            [FromRoute(Name = "id")] string id
            )
        {
            Models.Usage? usage;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateUsageDatabaseClient();
                UsageAdapter adapter = new(databaseClient);
                usage = await adapter.GetAsync(id);
            }
            catch (ArgumentException)
            {
                return Results.BadRequest();
            }
            catch (DatabaseParameterException)
            {
                return Results.BadRequest();
            }
            if (usage is null)
            {
                return Results.NotFound();
            }
            Models.UsageGetResponse response = new()
            {
                Id = usage.Id,
                Summary = usage.Summary,
                Description = usage.Description
            };
            return Results.Ok(response);
        }

        private static async Task<IResult> UpdateUsageAsync(
            [FromRoute(Name = "id")] string id,
            [FromBody] Models.UsageUpdateRequest updateRequest
            )
        {
            bool success;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateUsageDatabaseClient();
                UsageAdapter adapter = new(databaseClient);
                Models.Usage usage = new()
                {
                    Id = id,
                    Description = updateRequest.Description
                };
                success = await adapter.SetAsync(usage);
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

        private static async Task<IResult> DeleteUsageAsync(
            [FromRoute(Name = "id")] string id
            )
        {
            bool success;
            try
            {
                DatabaseClientFactory databaseClientFactory = new();
                using var databaseClient = databaseClientFactory.CreateUsageDatabaseClient();
                UsageAdapter adapter = new(databaseClient);
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
