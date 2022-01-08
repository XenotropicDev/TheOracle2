using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OracleData;
using TheOracle2.DataClasses;
using TheOracle2.GameObjects;

namespace TheOracle2.UserContent;

public class EFContext : DbContext
{
    public EFContext(DbContextOptions<EFContext> options) : base(options)
    {
        //Database.EnsureCreated();
    }

    public DbSet<OracleGuild> OracleGuilds { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<OracleInfo> OracleInfo { get; set; }
    public DbSet<Oracle> Oracles { get; set; }
    public DbSet<Ability> AssetAbilities { get; set; }
    public DbSet<Tables> Tables { get; set; }
    public DbSet<OracleStub> OracleStubs { get; set; }
    public DbSet<ChanceTable> ChanceTables { get; set; }
    public DbSet<PlayerCharacter> PlayerCharacters { get; set; }

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
        modelBuilder.UseCollation("NOCASE");

        var stringArrayToCSVConverter = new ValueConverter<IList<string>, string>(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<IList<string>>(v)
            );

        var valueComparer = new ValueComparer<IList<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()
            );

        var requiresConverter = new ValueConverter<IDictionary<string, string[]>, string>(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<IDictionary<string, string[]>>(v)
            );

        var requiresComparer = new ValueComparer<IDictionary<string, string[]>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c
            );

        modelBuilder.Entity<Subcategory>().HasOne(o => o.OracleInfo).WithMany(oi => oi.Subcategories).HasForeignKey(o => o.OracleInfoId).IsRequired();
        modelBuilder.Entity<Oracle>().HasOne(o => o.Subcategory).WithMany(sub => sub.Oracles).HasForeignKey(o => o.SubcategoryId).IsRequired(false);

        //TheOracle Stuff
        modelBuilder.Entity<PlayerCharacter>().Property(pc => pc.Impacts).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);

        //Dataforged stuff
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
        modelBuilder.Entity<Inherit>().Property(s => s.Requires).HasConversion(requiresConverter).Metadata.SetValueComparer(requiresComparer);
        modelBuilder.Entity<MoveStatOptions>().Property(a => a.Legacies).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<MoveStatOptions>().Property(a => a.Progress).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<MoveStatOptions>().Property(a => a.Stats).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Oracle>().Property(o => o.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Oracle>().Property(o => o.ContentTags).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Oracle>().Property(o => o.PartOfSpeech).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Oracle>().Property(s => s.Requires).HasConversion(requiresConverter).Metadata.SetValueComparer(requiresComparer);
        modelBuilder.Entity<OracleInfo>().Property(o => o.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<OracleInfo>().Property(o => o.Tags).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Select>().Property(a => a.Options).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Subcategory>().Property(s => s.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Subcategory>().Property(s => s.ContentTags).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Subcategory>().Property(s => s.Requires).HasConversion(requiresConverter).Metadata.SetValueComparer(requiresComparer);
        modelBuilder.Entity<Subcategory>().Property(s => s.SampleNames).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Tables>().Property(s => s.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Tables>().Property(s => s.Requires).HasConversion(requiresConverter).Metadata.SetValueComparer(requiresComparer);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            //.UseNpgsql("Host=TheOracle;Database=GameData;")
            .UseSqlite("Data Source=GameContent.db;Cache=Shared")
            .UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }
}