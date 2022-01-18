using TheOracle2;
namespace TheOracle2.GameObjects;

/// <summary>
/// Interface for game objects where challenge dice matches are relevant.
/// </summary>
public interface IMatchable
{
  public bool IsMatch { get; }
}