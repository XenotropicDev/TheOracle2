using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OracleData;
using TheOracle2.DataClasses;

namespace TheOracle2.UserContent;

public class EFContext : DbContext
{
    public EFContext(DbContextOptions<EFContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<OracleGuild> OracleGuilds { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<OracleInfo> OracleInfo { get; set; }
    public DbSet<Oracle> Oracles { get; set; }
    public DbSet<Tables> ChanceTables { get; set; }
    public DbSet<Ability> AssetAbilities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var stringArrayToCSVConverter = new ValueConverter<IList<string>, string>(
            v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
            v => JsonSerializer.Deserialize<IList<string>>(v, new JsonSerializerOptions())
            );

        var valueComparer = new ValueComparer<IList<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()
            );

        modelBuilder.Entity<Asset>().Property(a => a.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Asset>().Property(a => a.Input).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Ability>().Property(a => a.Input).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Select>().Property(a => a.Options).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<StatOptions>().Property(a => a.Stats).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Oracle>().Property(o => o.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Oracle>().Property(o => o.ContentTags).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Oracle>().Property(o => o.PartOfSpeech).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<OracleInfo>().Property(o => o.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<OracleInfo>().Property(o => o.Tags).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Attributes>().Property(a => a.Location).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<ChanceTable>().Property(c => c.Assets).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Inherit>().Property(i => i.Exclude).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Inherit>().Property(i => i.Name).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.DerelictType).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.Environment).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.Life).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.Location).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.PlanetaryClass).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.Region).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.Scale).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.StarshipType).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.Type).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.ThemeType).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.Zone).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Subcategory>().Property(s => s.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Subcategory>().Property(s => s.SampleNames).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Tables>().Property(s => s.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=GameContent.db;Cache=Shared");
        base.OnConfiguring(optionsBuilder);
    }
}