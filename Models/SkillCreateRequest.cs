// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using System.Text.Json.Serialization;

namespace ResumeManagementApi.Models
{
    public record class SkillCreateRequest
    {
        [JsonPropertyName("skillId")]
        [JsonRequired]
        public required string SkillId { get; init; }
    }
}
