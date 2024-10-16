// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using System.Text.Json.Serialization;

namespace ResumeManagementApi.Models
{
    public record class ResumeCreateRequest
    {
        [JsonPropertyName("userId")]
        [JsonRequired]
        public required string UserId { get; init; }
    }
}
