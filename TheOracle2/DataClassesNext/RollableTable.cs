using System.Linq;
namespace TheOracle2.DataClassesNext;


public class RollableTable : List<RollableTableRow>
{
  public RollableTableRow Lookup(int roll)
  {
    return Find(row => row.RollIsInRange(roll));
  }
  public RollableTableRow LookupResult(string result)
  {
    return Find(row => row.Result == result);
  }
  public override string ToString()
  {
    return string.Join("\n", this.Select(row => row.ToString()));
  }
}