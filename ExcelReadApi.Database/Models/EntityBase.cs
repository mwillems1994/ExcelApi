using System;
using System.ComponentModel.DataAnnotations;

namespace ExcelReadApi.Database.Models
{
    public abstract class EntityBase : EntityLog
    {
        [Required, Key]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
