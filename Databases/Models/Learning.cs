// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeManagementApi.Databases.Models
{
    [Table("learn")]
    public record class Learning
    {
        [Column("id")]
        public long? Id { get; init; }
        [Column("version")]
        public int? Version { get; init; }
        [Column("summary")]
        public string? Summary { get; set; }
        [Column("description")]
        public string? Description { get; set; }
    }
}
