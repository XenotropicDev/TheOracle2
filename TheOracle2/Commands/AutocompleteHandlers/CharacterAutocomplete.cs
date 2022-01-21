using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using TheOracle2.UserContent;

namespace TheOracle2;

public class CharacterAutocomplete : AutocompleteHandler
{
    public EFContext Db { get; set; }

    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {

            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();

            var userText = autocompleteInteraction.Data.Current.Value as string;
            var userId = autocompleteInteraction.User.Id;
            var guildId = (context.Guild?.Id ?? autocompleteInteraction.User.Id);
            var guildPlayer = Db.GuildPlayers.Find(userId, guildId);
            var lastUsedPcId = guildPlayer?.LastUsedPcId ?? 0;
            IEnumerable<GameObjects.PlayerCharacter> characterList = new List<GameObjects.PlayerCharacter>();
            characterList = string.IsNullOrEmpty(userText) switch
            {
                true => Db.PlayerCharacters.Where(c => c.DiscordGuildId == guildId),
                false => Db.PlayerCharacters.Where(c => c.DiscordGuildId == guildId && EF.Functions.Like(c.Name, $"{userText}%"))
            };
            if (characterList.Count() > 10)
            {
                characterList = characterList.Where(c => c.UserId == context.User.Id);
            }
            successList = characterList.Select(x => new AutocompleteResult(x.Name, x.Id.ToString())).Take(SelectMenuBuilder.MaxOptionCount);

            return Task.FromResult(AutocompletionResult.FromSuccess(successList));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AutocompletionResult.FromError(ex));
        }
    }
}
