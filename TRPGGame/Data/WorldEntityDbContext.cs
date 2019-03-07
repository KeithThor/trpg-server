using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;

namespace TRPGGame.Data
{
    public class WorldEntityDbContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TRpgServer.Entities.mdf;Trusted_Connection=True;");
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
            });
        }

        public DbSet<WorldEntity> PlayerEntities { get; set; }
    }
}
