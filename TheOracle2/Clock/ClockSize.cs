using System.ComponentModel.DataAnnotations;
using Discord.Interactions;
namespace TheOracle2.GameObjects;

// TODO: add display names once ChoiceDisplayAttribute is available
public enum ClockSize
{
  [Display(Name = "4 segments")]
  Four = 4,

  [Display(Name = "6 segments")]
  Six = 6,

  [Display(Name = "8 segments")]
  Eight = 8,

  [Display(Name = "10 segments")]
  Ten = 10
}

public enum SceneChallengeClockSize
{
  [Display(Name = "4 segments")]
  Four = 4,

  [Display(Name = "6 segments")]
  Six = 6,

  [Display(Name = "8 segments")]
  Eight = 8,
}