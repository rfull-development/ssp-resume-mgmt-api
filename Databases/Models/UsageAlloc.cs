// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeManagementApi.Databases.Models
{
    [Table("usage_alloc")]
    public record class UsageAlloc
    {
        [Column("skill_id")]
        public long? SkillId { get; init; }
        [Column("usage_id")]
        public long? UsageId { get; init; }
        [Column("description")]
        public string? Description { get; set; }
    }
}
