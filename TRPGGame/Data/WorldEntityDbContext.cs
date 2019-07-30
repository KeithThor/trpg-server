using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;

namespace TRPGGame.Data
{
    public class WorldEntityDbContext: DbContext
    {
        private readonly DataConfig _dataConfig;

        public WorldEntityDbContext()
        {
            _dataConfig = new DataConfig();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_dataConfig.DbConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WorldEntity>(entity =>
            {
                entity.OwnsOne(e => e.Position, position =>
                {
                    position.Property(e => e.PositionX).HasColumnName("PositionX");
                    position.Property(e => e.PositionY).HasColumnName("PositionY");
                });
                //entity.Property(e => e.IconUris)
                //      .HasConversion(
                //        v => string.Join(',', v),
                //        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
            });
        }

        public DbSet<WorldEntity> PlayerEntities { get; set; }
    }
}
