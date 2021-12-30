using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
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
    public DbSet<Ability> AssetAbilities { get; set; }
    public DbSet<Tables> Tables { get; set; }

    public async Task RecreateDB()
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();

        var baseDir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Data"));
        var file = baseDir.GetFiles("assets.json").FirstOrDefault();

        string text = file.OpenText().ReadToEnd();
        var root = JsonConvert.DeserializeObject<List<Asset>>(text);

        foreach (var asset in root)
        {
            Assets.Add(asset);
        }
        await SaveChangesAsync();

        file = baseDir.GetFiles("moves.json").FirstOrDefault();

        text = file.OpenText().ReadToEnd();
        var moveRoot = JsonConvert.DeserializeObject<MovesInfo>(text);

        foreach (var move in moveRoot.Moves)
        {
            Moves.Add(move);
        }
        await SaveChangesAsync();

        file = baseDir.GetFiles("oracles.json").FirstOrDefault();

        text = file.OpenText().ReadToEnd();
        var oracleRoot = JsonConvert.DeserializeObject<List<OracleInfo>>(text);

        foreach (var oi in oracleRoot)
        {
            OracleInfo.Add(oi);
            await SaveChangesAsync();
        }
    }

    public bool HasTables() => Database.GetService<IRelationalDatabaseCreator>().HasTables();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var stringArrayToCSVConverter = new ValueConverter<IList<string>, string>(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<IList<string>>(v)
            );

        var valueComparer = new ValueComparer<IList<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()
            );

        modelBuilder.Entity<Subcategory>().HasOne(o => o.OracleInfo).WithMany(oi => oi.Subcategories).HasForeignKey(o => o.OracleInfoId).IsRequired();
        modelBuilder.Entity<Oracle>().HasOne(o => o.Subcategory).WithMany(sub => sub.Oracles).HasForeignKey(o => o.SubcategoryId).IsRequired(false);
        //modelBuilder.Entity<Oracle>().HasOne(o => o.OracleInfo).WithMany(oi => oi.Oracles).HasForeignKey(o => o.OracleInfoId);
        //modelBuilder.Entity<ChanceTable>().HasOne(t => t.Oracle).WithMany(o => o.Table).HasForeignKey(o => o.OracleId);
        //modelBuilder.Entity<Tables>().HasOne(t => t.Oracle).WithMany(o => o.Tables).HasForeignKey(o => o.OracleId);

        modelBuilder.Entity<Ability>().Property(a => a.Input).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Asset>().Property(a => a.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Asset>().Property(a => a.Input).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<AssetStatOptions>().Property(s => s.Legacies).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<AssetStatOptions>().Property(s => s.Resources).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<AssetStatOptions>().Property(s => s.Stats).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Attributes>().Property(a => a.DerelictType).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Attributes>().Property(a => a.Location).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Attributes>().Property(a => a.Type).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<ChanceTable>().Property(c => c.Assets).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<ChanceTable>().Property(c => c.PartOfSpeech).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<ConditionMeter>().Property(c => c.Conditions).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Inherit>().Property(i => i.Exclude).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Inherit>().Property(i => i.Name).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<MoveStatOptions>().Property(a => a.Stats).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<MoveStatOptions>().Property(a => a.Legacies).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<MoveStatOptions>().Property(a => a.Progress).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Oracle>().Property(o => o.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Oracle>().Property(o => o.ContentTags).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Oracle>().Property(o => o.PartOfSpeech).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<OracleInfo>().Property(o => o.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<OracleInfo>().Property(o => o.Tags).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.DerelictType).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.Environment).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.Life).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.Location).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.PlanetaryClass).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.Region).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.Scale).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.StarshipType).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.ThemeType).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.Type).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(r => r.Zone).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Select>().Property(a => a.Options).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Subcategory>().Property(s => s.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Subcategory>().Property(s => s.ContentTags).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Subcategory>().Property(s => s.SampleNames).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Tables>().Property(s => s.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlite("Data Source=GameContent.db;Cache=Shared")
            .UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }
}