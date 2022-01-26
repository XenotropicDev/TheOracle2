namespace TheOracle2.GameObjects;

public interface IEncounterStub : IRanked
{ }

public interface IEncounter : IEncounterStub
{ }

public interface IEncounterVariant : IEncounterStub
{
    public IEncounter Parent { get; set; }
}
