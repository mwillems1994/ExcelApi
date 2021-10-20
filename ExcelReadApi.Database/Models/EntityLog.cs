using System;

namespace ExcelReadApi.Database.Models
{
    public abstract class EntityLog
    {
        public DateTime? Deleted { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Changed { get; set; } = DateTime.Now;
    }
}
