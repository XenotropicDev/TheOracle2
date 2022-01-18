namespace TheOracle2.DataClassesNext;
public class Source
{
  public string Name { get; set; }
  public int? Page { get; set; }
  [JsonProperty("Date")]
  private string RawDate { get; set; }
  [JsonIgnore]
  public DateOnly Date
  // TODO: rewrite as a proper converter. it's arguable whether the Dataforged date string (e.g. "122421") needs to be a Date anyways... but maybe it'll have some use in managing user DB content?
  {
    get
    {
      int month = Int16.Parse(RawDate.Substring(0, 2));
      int day = Int16.Parse(RawDate.Substring(2, 2));
      int year = Int16.Parse(RawDate.Substring(4, 2)) + 2000;
      return new DateOnly(year, month, day);
    }
  }
  public override string ToString()
  {
    var outputStr = Name;
    if (RawDate != null) { outputStr = outputStr + $" {Date.ToString("MMddyy")}"; }
    if (Page != 0) { outputStr = outputStr + $", p. {Page}"; }
    return outputStr;
  }
}
