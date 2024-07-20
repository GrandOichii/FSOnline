using System.Diagnostics.Metrics;

namespace FSCore.Matches.Cards;

public class InPlayMatchCard : IStateModifier {
    /// <summary>
    /// Original card
    /// </summary>
    public MatchCard Card { get; }
    /// <summary>
    /// Item ID
    /// </summary>
    public string IPID { get; }
    /// <summary>
    /// Shows whether the card is tapped (deactivated)
    /// </summary>
    public bool Tapped { get; private set; }
    /// <summary>
    /// List of counters
    /// </summary>
    public Dictionary<string, Counter> Counters { get; }

    public InPlayMatchCard(MatchCard card) {
        Card = card;

        IPID = card.Match.GenerateInPlayID();
        Counters = new();
    }

    public string LogName => $"{Card.Template.Name} [{IPID} ({Card.ID})]";

    public virtual string GetFormattedName() {
        return $"{{ip:{LogName}}}";
    }

    /// <summary>
    /// Tap the card
    /// </summary>
    public async Task Tap() {
        Tapped = true;

        // TODO add update
    }

    /// <summary>
    /// Untap the card
    /// </summary>
    /// <returns></returns>
    public async Task Untap() {
        Tapped = false;

        // TODO add update
    }

    /// <summary>
    /// Gets all of the activated abilities of the card
    /// </summary>
    /// <returns>Activated abilities</returns>
    public List<ActivatedAbility> GetActivatedAbilities() {
        // TODO use state
        return Card.ActivatedAbilities;
    }

    /// <summary>
    /// Checks whether the card can be activated by the specified player
    /// </summary>
    /// <param name="player">Activator</param>
    /// <returns>True, if card can be activated</returns>
    public virtual bool CanBeActivatedBy(Player player) {
        return true;
    }

    public ActivatedAbility GetActivatedAbility(int idx) {
        return GetActivatedAbilities()[idx];
    }

    public List<LuaFunction> GetStateModifiers(ModificationLayer layer) {
        // TODO use abilities from state
        if (!Card.StateModifiers.ContainsKey(layer))
            return new();
        return Card.StateModifiers[layer];
    }

    public void Modify(ModificationLayer layer)
    {
        // TODO catch exception
        var stateModifiers = GetStateModifiers(layer);
        foreach (var mod in stateModifiers) {
            mod.Call(this);
        }
    }

    public void UpdateState()
    {
        // TODO
    }

    #region Counters

    /// <summary>
    /// Add an amount of generic counters
    /// </summary>
    /// <param name="amount">Amount of counters to add</param>
    public async Task AddCounters(int amount) {
        if (!Counters.ContainsKey(Counter.GENERIC_NAME)) {
            Counters.Add(Counter.GENERIC_NAME, new(0));
        }
        Counters[Counter.GENERIC_NAME].Add(amount);
    }

    /// <summary>
    /// Get the total amount of counters on the card
    /// </summary>
    /// <returns>Amount of counters</returns>
    public int GetCountersCount() {
        var result = 0;
        foreach (var c in Counters.Values)
            result += c.Amount;
        return result;
    }

    public async Task RemoveCounters(int amount) {
        // TODO this removes generic counters first, then picks at random - change to player choice

        while (amount > 0) {
            if (Counters.Count == 0)
                throw new MatchException($"Not enough counters to remove a total of {amount} from card {LogName}");

            var key = Counters.Keys.ElementAt(0);
            if (Counters.ContainsKey(Counter.GENERIC_NAME))
                key = Counter.GENERIC_NAME;
            var counter = Counters[key];
            var a = amount;
            if (a > counter.Amount)
                a = counter.Amount;
            counter.Remove(a);
            amount -= a;
            if (counter.Amount == 0)
                Counters.Remove(key);
        }
    }

    #endregion


}