using ExcelReadApi.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExcelReadApi.Database.Context
{
    public class ExcelDbContext: DbContext
    {
        public DbSet<ExcelFormModel> Blogs { get; set; } = null!;
        public string DbPath { get; private set; } = string.Empty;

        public ExcelDbContext(DbContextOptions<ExcelDbContext> options) : base(options)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}excel.db";
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is EntityLog
                    && (e.State == EntityState.Added
                        || e.State == EntityState.Modified))
                .ToArray();

            foreach (var entry in entries)
            {
                var entity = (EntityLog)entry.Entity;
                entity.Changed = DateTime.Now;

                if (entry.State == EntityState.Added)
                {
                    entity.Created = DateTime.Now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ExcelFormModel>()
                .HasIndex(item => new { item.FirstName, item.LastName, item.Address })
                .IsUnique();
        }
    }
}
