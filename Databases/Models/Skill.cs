// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeManagementApi.Databases.Models
{
    [Table("skill")]
    public record class Skill
    {
        [Column("id")]
        public long? Id { get; init; }
        [Column("item_id")]
        public long? ItemId { get; init; }
        [Column("row_id")]
        public int? RowId { get; init; }
        [Column("version")]
        public int? Version { get; init; }
        [Column("skill_id")]
        public string? SkillId { get; set; }
    }
}
