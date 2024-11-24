// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using System.Text.Json.Serialization;

namespace ResumeManagementApi.Models
{
    public record class Skill
    {
        [JsonPropertyName("id")]
        [JsonRequired]
        public required string Id { get; init; }

        [JsonPropertyName("skillId")]
        [JsonRequired]
        public required string SkillId { get; init; }
        [JsonPropertyName("usages")]
        public List<SkillUsage>? Usages { get; init; }
        [JsonPropertyName("learnings")]
        public List<SkillLearning>? Learnings { get; init; }
    }
}
