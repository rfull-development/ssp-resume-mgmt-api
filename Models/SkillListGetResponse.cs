// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using System.Text.Json.Serialization;

namespace ResumeManagementApi.Models
{
    public record class SkillListGetResponse
    {
        [JsonPropertyName("count")]
        [JsonRequired]
        public required int Count { get; init; }
        [JsonPropertyName("skills")]
        [JsonRequired]
        public required List<Skill> Skills { get; init; }
    }
}
