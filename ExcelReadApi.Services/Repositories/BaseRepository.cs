using ExcelReadApi.Database.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReadApi.Services.Repositories
    {
        public abstract class BaseRepository<T>
            where T : class
        {
            internal readonly ExcelDbContext _db;

            public BaseRepository(ExcelDbContext db)
            {
                _db = db;
            }

            protected IQueryable<T> All => _db.Set<T>();

            protected IQueryable<T> AllNoTracking
            {
                get
                {
                    return All.AsNoTracking();
                }
            }

            public void Add(T entity)
            {
                _db.Add(entity);
            }

            public async Task AddRangeAsync(IEnumerable<T> entities)
            {
                await _db.AddRangeAsync(entities);
            }

        public void Edit(T entity)
            {
                _db.Entry(entity).State = EntityState.Modified;
            }

            public void Delete(T entity)
            {
                _db.Remove(entity);
            }

            public async Task RefreshAsync(T entity)
            {
                await _db.Entry(entity).ReloadAsync();
            }

            public async Task CommitAsync()
            {
                await _db.SaveChangesAsync();
            }
        }
    }
