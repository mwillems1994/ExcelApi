
using ExcelReadApi.Database.Context;
using ExcelReadApi.Database.Models;
using System.Linq;

namespace ExcelReadApi.Services.Repositories
{
    public class ExcelFormRepository : BaseRepository<ExcelFormModel>
    {
        public ExcelFormRepository(ExcelDbContext db) : base(db)
        {
        }

        public IQueryable<ExcelFormModel> AllExcelForms => All
            .Where(item => !item.Deleted.HasValue); // Could solve with with a query filter and reflection...
    }
}
