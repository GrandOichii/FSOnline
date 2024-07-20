namespace FSCore.Matches.Counters;

/// <summary>
/// Generic card counter
/// </summary>
public class Counter {
    /// <summary>
    /// Name of the generic counter
    /// </summary>
    public static readonly string GENERIC_NAME = "Generic";

    /// <summary>
    /// Counter name
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Amount of counters
    /// </summary>
    public int Amount { get; private set; }

    public Counter(int amount) {
        Amount = amount;

        Name = GENERIC_NAME;
    }

    public void Add(int amount) {
        Amount += amount;
    }

    public void Remove(int amount) {
        if (amount > Amount)
            throw new MatchException($"Cannot remove {amount} {Name} counters from total of {Amount}");
        Amount -= amount;
    }
}