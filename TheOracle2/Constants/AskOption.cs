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
  [Display(Name = "Small Chance")]
  SmallChance = 10
}

