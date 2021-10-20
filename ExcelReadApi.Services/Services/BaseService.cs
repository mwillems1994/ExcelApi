using ExcelReadApi.Database.Context;
using System.Threading.Tasks;

namespace ExcelReadApi.Services.Services
{
    public abstract class BaseService
    {
        private readonly ExcelDbContext _db;

        public BaseService(ExcelDbContext db)
        {
            _db = db;
        }

        public async Task CommitAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
