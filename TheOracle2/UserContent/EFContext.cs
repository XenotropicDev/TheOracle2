using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TheOracle2.DataClasses;
using TheOracle2.GameObjects;

namespace TheOracle2.UserContent;

public class EFContext : DbContext
{
    public EFContext()
    {
        //Database.EnsureCreated();
    }

    public DbSet<GuildPlayer> GuildPlayers { get; set; }
    public DbSet<PlayerCharacter> PlayerCharacters { get; set; }
    public DbSet<OracleObjectTemplate> OracleTemplates { get; set; }
    public DbSet<OracleInfo> OracleInfo { get; set; }

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

        //TheOracle Stuff
        modelBuilder.Entity<PlayerCharacter>().Property(pc => pc.Impacts).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<GuildPlayer>().HasKey(guildPlayer => new { guildPlayer.UserId, guildPlayer.DiscordGuildId });

        //Data stuff
        modelBuilder.Entity<OracleInfo>().Navigation(oi => oi.Oracles).AutoInclude();
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
