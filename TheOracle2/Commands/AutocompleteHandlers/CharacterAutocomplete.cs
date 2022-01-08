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

            var value = autocompleteInteraction.Data.Current.Value as string;
            var guildId = (context.Guild?.Id ?? autocompleteInteraction.User.Id);


            if (string.IsNullOrEmpty(value))
            {
                var defaultChars = Db.PlayerCharacters.Where(c => c.DiscordGuildId == guildId);
                if (defaultChars.Count() > 10) defaultChars = defaultChars.Where(c => c.UserId == context.User.Id);
                successList = defaultChars.Select(x => new AutocompleteResult(x.Name, x.Id.ToString())).Take(SelectMenuBuilder.MaxOptionCount);
                return Task.FromResult(AutocompletionResult.FromSuccess(successList));
            }

            var characters = Db.PlayerCharacters.Where(c => c.DiscordGuildId == guildId && EF.Functions.Like(c.Name, $"{value}%"));
            if (characters.Count() > 10) characters = characters.Where(c => c.UserId == context.User.Id);
            successList = characters.Select(x => new AutocompleteResult(x.Name, x.Id.ToString())).Take(SelectMenuBuilder.MaxOptionCount);
            return Task.FromResult(AutocompletionResult.FromSuccess(successList));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AutocompletionResult.FromError(ex));
        }
    }
}