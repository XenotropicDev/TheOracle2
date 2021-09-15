using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OracleData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle2.UserContent
{
    public class EFContext : DbContext
    {
        public EFContext(DbContextOptions<EFContext> options) : base(options)
        {

        }

        public DbSet<OracleGuild> OracleGuilds { get; set; }
        public DbSet<GameItem> GameItems { get; set; }
        public DbSet<Asset> GameAssets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<OracleGuild>().HasMany(o => o.GameItems).WithMany(gi => gi.SubscribedGuilds).UsingEntity(j => j.ToTable("GameItemOracleGuild"));

            //modelBuilder.Entity<Asset>().Property(a => a.Fields).HasConversion(v => string.Join(',', v), v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=GameContent.db;Cache=Shared");
            base.OnConfiguring(optionsBuilder);
        }

    }
}
