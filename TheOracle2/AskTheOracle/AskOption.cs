using System.ComponentModel.DataAnnotations;
namespace TheOracle2;
public enum AskOption
{
  [Display(Name = "Sure thing")]
  SureThing = 90,
  Likely = 75,
  [Display(Name = "Fifty-fifty")]
  FiftyFifty = 50,
  Unlikely = 25,
  [Display(Name = "Small chance")]
  SmallChance = 10
}

public class AskDefs
{
  public Dictionary<int, string> ByNumber { get; set; }
  public Dictionary<string, int> ByString { get; set; }

  public AskDefs()
  {
    ByNumber = new()
    {
      { 10, "small chance" },
      { 25, "unlikely" },
      { 50, "fifty-fifty" },
      { 75, "likely" },
      { 90, "sure thing" }
    };
    ByString = new();
  }
}

