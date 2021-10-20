
using ExcelReadApi.Database.Context;
using ExcelReadApi.Database.Models;
using System.Linq;

namespace ExcelReadApi.Services.Repositories
{
    public class ExcelFormRepository : BaseRepository<ExcelForm>
    {
        public ExcelFormRepository(ExcelDbContext db) : base(db)
        {
        }

        public IQueryable<ExcelForm> AllExcelForms => All
            .Where(item => !item.Deleted.HasValue); // Could solve with with a query filter and reflection...
    }
}
