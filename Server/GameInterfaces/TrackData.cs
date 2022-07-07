using System.ComponentModel.DataAnnotations;

namespace Server.GameInterfaces
{
    public record TrackData
    {
        public int Id { get; set; }

        [MaxLength(EmbedBuilder.MaxDescriptionLength)]
        public string? Description { get; set; }
        public ChallengeRank Rank { get; set; }
        public int Ticks { get; set; } = 0;
        
        [MaxLength(EmbedBuilder.MaxTitleLength)]
        public string? Title { get; set; }
    }
}
