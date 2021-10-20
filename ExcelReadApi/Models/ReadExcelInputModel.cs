using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ExcelReadApi.Models
{
    public class ReadExcelInputModel
    {
        [Required(ErrorMessage = "File cannot be empty")]
        public IFormFile File { get; set; }
    }
}
