// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using System.Text.Json.Serialization;

namespace ResumeManagementApi.Models
{
    public record class LearningListGetResponse
    {
        [JsonPropertyName("totalCount")]
        [JsonRequired]
        public required long TotalCount { get; init; }
        [JsonPropertyName("count")]
        [JsonRequired]
        public required int Count { get; init; }
        [JsonPropertyName("learnings")]
        [JsonRequired]
        public required List<Learning> Learnings { get; init; }
    }
}
