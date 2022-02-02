using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.ActionRoller;
using TheOracle2.Commands;
using TheOracle2.UserContent;

namespace TheOracle2;

public class EntityCommand : InteractionModuleBase
{
    public EntityCommand(IServiceProvider services, EFContext dbContext)
    {
        DbContext = dbContext;
        Services = services;
    }

    public EFContext DbContext { get; }
    public Random Random { get; }
    public IServiceProvider Services { get; }

    [SlashCommand("entity", "Creates an entity based on its template")]
    public async Task PostTemplate([Autocomplete(typeof(EntityAutocomplete))] string entity)
    {
        if (!int.TryParse(entity, out int id)) throw new ArgumentException($"Unknown entity type: {entity}");
        var template = DbContext.OracleTemplates.Find(id);

        var rollerFactory = ActivatorUtilities.GetServiceOrCreateInstance<TableRollerFactory>(Services);
        var entityItem = template.Build(rollerFactory);

        await RespondAsync(embeds: entityItem.GetEmbeds(), ephemeral: entityItem.IsEphemeral); //components: entityItem.GetComponents()
    }
}

public class EntityComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    //Not used yet (ever?)
}
