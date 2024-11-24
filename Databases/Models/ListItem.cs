// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeManagementApi.Databases.Models
{
    [Table("list")]
    public record class ListItem
    {
        [Column("id")]
        public long? Id { get; init; }
        [Column("guid")]
        public Guid? Guid { get; init; }
    }
}
