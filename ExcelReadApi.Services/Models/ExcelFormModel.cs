
using ExcelReadApi.Database.Models;

namespace ExcelReadApi.Services.Models
{
    public record ExcelFormModel(string FirstName, string LastName, string Address)
    {
        public bool Equals(ExcelForm excelForm) =>
            // TODO: Check case etc..
            excelForm.FirstName == FirstName && excelForm.LastName == LastName && excelForm.Address == Address;
    }
}
