using System.ComponentModel.DataAnnotations;

namespace ExcelReadApi.Database.Models
{
    public class ExcelFormModel: EntityBase
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Address { get; set; } = string.Empty;
    }
}
