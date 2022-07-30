using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Server.GameInterfaces;
using TheOracle2.GameObjects;

namespace Server.DiscordServer;

public class ApplicationContext : DbContext
{
    public DbSet<PlayerCharacter> PlayerCharacters { get; set; }
    public DbSet<TrackData> ProgressTrackers { get; set; }
    public DbSet<AssetData> CharacterAssets { get; set; }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var stringArrayToCSVConverter = new ValueConverter<IList<string>, string>(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<IList<string>>(v) ?? new List<string>()
            );

        var valueComparer = new ValueComparer<IList<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()
            );

        modelBuilder.Entity<PlayerCharacter>().Property(pc => pc.Impacts).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<AssetData>().Property(a => a.SelectedAbilities).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<AssetData>().Property(a => a.Inputs).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);

    }
}
