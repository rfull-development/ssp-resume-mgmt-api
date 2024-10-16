// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using System.Text.Json.Serialization;

namespace ResumeManagementApi.Models
{
    public record class Usage
    {
        [JsonPropertyName("id")]
        [JsonRequired]
        public required string Id { get; init; }
        [JsonPropertyName("summary")]
        public string? Summary { get; init; }
        [JsonPropertyName("description")]
        public string? Description { get; init; }
    }
}
