// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeManagementApi.Databases.Models
{
    [Table("usage_alloc_seg")]
    public record class UsageAllocSegment
    {
        [Column("skill_id")]
        public long SkillId { get; init; }
        [Column("version")]
        public int? Version { get; init; }
    }
}
