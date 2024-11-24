// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeManagementApi.Databases.Models
{
    [Table("learn_alloc")]
    public record class LearningAlloc
    {
        [Column("skill_id")]
        public long? SkillId { get; init; }
        [Column("learn_id")]
        public long? LearningId { get; init; }
        [Column("description")]
        public string? Description { get; set; }
    }
}
