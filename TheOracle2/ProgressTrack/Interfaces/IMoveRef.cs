using Discord.WebSocket;
using TheOracle2.UserContent;
using Discord.Interactions;
using System.Text.RegularExpressions;
namespace TheOracle2.GameObjects;

/// <summary>
/// Interface for widgets that include a select menu with move references.
/// </summary>
public interface IMoveRef : IWidget
{
  public EFContext DbContext { get; }
  public string[] MoveReferences { get; }

  // public DataClasses.Move[] MoveRefs { get; }
  public SelectMenuBuilder MoveRefMenu();

  /// <summary>
  /// Generates menu options representing move references for an IMoveRef.
  /// </summary>
  /// <param name="moveRefParent">The IMoveRef to build a list for.</param>
  /// <param name="prefix">A prefix to add to the Value of the menu options</param>
  public static List<SelectMenuOptionBuilder> MenuOptions(IMoveRef moveRefParent, string prefix = "")
  {
    List<SelectMenuOptionBuilder> options = new();
    foreach (string moveName in moveRefParent.MoveReferences)
    {
      DataClasses.Move moveData = moveRefParent.DbContext.Moves.Find(moveName);
      DiscordMoveEntity moveEntity = new(moveData);
      options.Add(moveEntity.ReferenceOption());
    }
    return options;
  }
  /// <summary>
  /// Builds a menu of move references from an IMoveRef.
  /// </summary>
  public static SelectMenuBuilder MenuBase(IMoveRef moveRefParent)
  {
    SelectMenuBuilder menu =
     new SelectMenuBuilder()
       .WithPlaceholder("Reference moves...")
       .WithCustomId("move-ref-menu")
       .WithOptions(MenuOptions(moveRefParent));
    return menu;
  }
}