using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.DiscordHelpers;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2;

public class PlayerRollCommand : InteractionModuleBase
{
  public PlayerRollCommand(Random random, EFContext efContext)
  {
    Random = random;
    EfContext = efContext;
  }

  public Random Random { get; }
  public EFContext EfContext { get; }

  [SlashCommand("action-pc-roll", "Performs an Ironsworn action roll using a player character's stats.")]
  public async Task ActionRoll(
      [Summary(description: "The character to use for the roll")][Autocomplete(typeof(CharacterAutocomplete))] string character,
      [Summary(description: "The stat value to use for the roll")] RollableStats stat,
      [Summary(description: "Any adds to the roll")][MinValue(0)] int adds,
      [Summary(description: "A preset value for the Action Die (d6) to use instead of rolling.")][MinValue(1)][MaxValue(6)] int? actionDie = null,
      [Summary(description: "A preset value for the first Challenge Die (d10) to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie1 = null,
      [Summary(description: "A preset value for the second Challenge Die (d10) to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie2 = null,
      [Summary(description: "Any notes, fiction, or other text you'd like to include in the post")] string text = "")
  {
    if (!int.TryParse(character, out int id))
    {
      await RespondAsync($"Unknown character", ephemeral: true);
      return;
    }

    var pc = EfContext.PlayerCharacters.Find(id);

    var roll = new ActionRoll(Random, GetStatValue(stat, pc), adds, GetStatValue(RollableStats.Momentum, pc), text, actionDie, challengeDie1, challengeDie2);

    ComponentBuilder component = null;
    if (roll.IsBurnable)
    {
      component = new ComponentBuilder()
          .WithButton("Burn", $"burn-roll-{roll.ChallengeDie1.Value},{roll.ChallengeDie2.Value},{pc.Id}", ButtonStyle.Danger, new Emoji("🔥"));
    }

    EmbedAuthorBuilder author = new EmbedAuthorBuilder().WithName($"{roll.RollTypeLabel}: +{stat}");
    if (pc.MessageId > 0)
    {
      IMessageChannel channel = (pc.ChannelId == Context.Channel.Id) ? Context.Channel : await (Context.Client as DiscordSocketClient).Rest.GetChannelAsync(pc.ChannelId) as IMessageChannel;
      var msg = await channel.GetMessageAsync(pc.MessageId);
      author.WithUrl(msg.GetJumpUrl());
    }

    await RespondAsync(embed: roll.ToEmbed().WithAuthor(author).Build(), components: component?.Build()).ConfigureAwait(false);
  }

  private int GetStatValue(RollableStats stat, PlayerCharacter pc)
  {
    return stat switch
    {
      RollableStats.Edge => pc.Edge,
      RollableStats.Heart => pc.Heart,
      RollableStats.Iron => pc.Iron,
      RollableStats.Shadow => pc.Shadow,
      RollableStats.Wits => pc.Wits,
      RollableStats.Health => pc.Health,
      RollableStats.Spirit => pc.Spirit,
      RollableStats.Supply => pc.Supply,
      RollableStats.Momentum => pc.Momentum,
      _ => throw new NotImplementedException(),
    };
  }
}

public class PCRollComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
  public PCRollComponents(Random random, EFContext efContext)
  {
    EfContext = efContext;
    Random = random;
  }

  public Random Random { get; }
  public EFContext EfContext { get; }

  [ComponentInteraction("burn-roll-*,*,*")]
  public async Task burnFromRoll(string Die1, string Die2, string pcId)
  {
    await DeferAsync();

    var embed = Context.Interaction.Message.Embeds?.FirstOrDefault();
    if (embed == null || !int.TryParse(pcId, out var Id) || !int.TryParse(Die1, out var die1Val) || !int.TryParse(Die2, out var die2Val))
    {
      await FollowupAsync($"I couldn't burn your momentum, please try doing it from the character card.", ephemeral: true);
      return;
    }

    var pc = EfContext.PlayerCharacters.Find(Id);

    var roll = new ActionRoll(Random, 0, 0, 0, $"{embed.Description}\n{pc.Name} burned {pc.Momentum} momentum to change this roll result", 1, die1Val, die2Val);
    roll.ActionDie = new Die(Random, 10, pc.Momentum);

    pc.BurnMomentum();
    await EfContext.SaveChangesAsync();

    if (pc.ChannelId == 0 || pc.MessageId == 0)
    {
      //Modify the message, but don't use FollowupAsync so we can reply with the ephemeral message
      await Context.Interaction.ModifyOriginalResponseAsync(msg =>
      {
        msg.Embed = roll.ToEmbed().WithAuthor(embed.Author?.ToEmbedAuthorBuilder()).Build();
        msg.Components = new ComponentBuilder().Build();
      });
      await FollowupAsync($"I couldn't find the character card to update, but it should update the next time you click a button on that card", ephemeral: true);
      return;
    }

    var entity = new PlayerCharacterEntity(pc);

    IMessageChannel channel = (pc.ChannelId == Context.Channel.Id) ? Context.Channel : await Context.Client.Rest.GetChannelAsync(pc.ChannelId) as IMessageChannel;
    await channel.ModifyMessageAsync(pc.MessageId, msg => msg.Embeds = entity.GetEmbeds());

    await FollowupAsync(
        embed: roll.ToEmbed().WithAuthor(embed.Author?.ToEmbedAuthorBuilder()).Build(),
        components: new ComponentBuilder().Build()
    );
  }
}

public enum RollableStats
{
  Edge,
  Heart,
  Iron,
  Shadow,
  Wits,
  Health,
  Spirit,
  Supply,
  Momentum,
}