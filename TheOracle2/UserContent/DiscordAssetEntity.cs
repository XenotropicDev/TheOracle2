using OracleData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle2.UserContent
{
  internal class DiscordAssetEntity : IDiscordEntity
  {
    private ComponentBuilder compBuilder;
    private EmbedBuilder builder;

    public DiscordAssetEntity(Asset asset)
    {
      Asset = asset;

      builder = new EmbedBuilder()
          .WithAuthor($"Asset: {asset.AssetType}")
          .WithTitle(asset.Name);

      if (asset.Abilities != null)
      {
        compBuilder ??= new ComponentBuilder();

        var select = new SelectMenuBuilder()
            .WithCustomId($"asset-ability-select:{asset.Id}")
            .WithPlaceholder($"Ability Selection")
            .WithMinValues(0)
            .WithMaxValues(asset.Abilities.Count);

        string description = string.Empty;
        foreach (var ability in asset.Abilities)
        {
          select.AddOption(new SelectMenuOptionBuilder()
              .WithLabel($"Ability {asset.Abilities.IndexOf(ability) + 1}")
              .WithValue($"{ability.Id}")
              .WithDefault(ability.Enabled)
              );

          if (ability.Enabled)
          {
            description += $"⬢ {ability.Text}\n\n";
          }
        }
        builder.WithDescription(description);
        compBuilder.WithSelectMenu(select);
      }

      if (asset.Counter != null)
      {
        compBuilder ??= new ComponentBuilder();

        compBuilder.WithSelectMenu(new SelectMenuBuilder()
            .WithCustomId($"asset-counter-select:{asset.Id}")
            .WithPlaceholder($"{asset.Counter.Name} actions")
            .WithMinValues(1)
            .WithMaxValues(1)
            .AddOption(new SelectMenuOptionBuilder().WithLabel($"+1 {asset.Counter.Name}").WithValue("asset-counter-up"))
            .AddOption(new SelectMenuOptionBuilder().WithLabel($"-1 {asset.Counter.Name}").WithValue("asset-counter-down"))
            .AddOption(new SelectMenuOptionBuilder().WithLabel($"Roll {asset.Counter.Name}").WithValue("asset-counter-roll"))
            );
      }

      if (asset.ConditionMeter != null)
      {
        builder.AddField(asset.ConditionMeter.Name, asset.ConditionMeter.StartsAt ?? asset.ConditionMeter.Max, false);

        //todo: show condition in select?
        var select = new SelectMenuBuilder()
            .WithCustomId($"asset-condition-select:{asset.Id}")
            .WithPlaceholder($"{asset.ConditionMeter.Name} actions")
            .WithMinValues(1)
            .WithMaxValues(1)
            .AddOption(new SelectMenuOptionBuilder().WithLabel($"+1 {asset.ConditionMeter.Name}").WithValue("asset-condition-up"))
            .AddOption(new SelectMenuOptionBuilder().WithLabel($"-1 {asset.ConditionMeter.Name}").WithValue("asset-condition-down"))
            .AddOption(new SelectMenuOptionBuilder().WithLabel($"Roll {asset.ConditionMeter.Name}").WithValue("asset-condition-roll"));

        if (asset.ConditionMeter.Conditions?.Count > 0)
        {
          foreach (var condition in asset.ConditionMeter.Conditions)
          {
            select.AddOption(new SelectMenuOptionBuilder().WithLabel($"Condition: {condition}").WithValue(condition));
          }
        }

        compBuilder ??= new ComponentBuilder();
        compBuilder.WithSelectMenu(select);
      }
    }

    public Asset Asset { get; }

    public Embed[] GetEmbeds() => new Embed[] { builder.Build() };
    public MessageComponent GetComponents() => compBuilder.Build();

    public bool IsEphemeral { get; set; } = false;

    public string GetDiscordMessage()
    {
      return null;
    }

  }
}
