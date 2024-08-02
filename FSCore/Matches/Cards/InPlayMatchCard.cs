namespace FSCore.Matches.Cards;

public class InPlayMatchCard : IStateModifier {
    /// <summary>
    /// Original card
    /// </summary>
    public MatchCard Card { get; }
    /// <summary>
    /// Item ID
    /// </summary>
    public string IPID { get; protected set; }
    /// <summary>
    /// Shows whether the card is tapped (deactivated)
    /// </summary>
    public bool Tapped { get; set; } = false;
    /// <summary>
    /// List of counters
    /// </summary>
    public Dictionary<string, Counter> Counters { get; protected set; }
    /// <summary>
    /// State
    /// </summary>
    public InPlayMatchCardState State { get; private set; }
    public Stats? Stats { get; set; }

    public Dictionary<TriggeredAbility, int> TriggerCountMap { get; }

    public InPlayMatchCard(MatchCard card) {
        Card = card;

        IPID = card.Match.GenerateInPlayID();
        Counters = [];
        TriggerCountMap = [];

        // Initial state
        State = new(this);
        Stats = Card.Template.Health == -1 ? null : new();
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
    public List<ActivatedAbilityWrapper> GetActivatedAbilities() {
        return State.ActivatedAbilities;
    }

    /// <summary>
    /// Checks whether the card can be activated by the specified player
    /// </summary>
    /// <param name="player">Activator</param>
    /// <returns>True, if card can be activated</returns>
    public virtual bool CanBeActivatedBy(Player player) {
        return true;
    }

    public ActivatedAbilityWrapper GetActivatedAbility(int idx) {
        return GetActivatedAbilities()[idx];
    }

    public List<LuaFunction> GetStateModifiers(ModificationLayer layer) {
        // TODO use abilities from state
        if (!Card.StateModifiers.TryGetValue(layer, out List<LuaFunction>? value))
            return [];
        return value;
    }

    public void Modify(ModificationLayer layer)
    {
        try {
            var stateModifiers = GetStateModifiers(layer);
            foreach (var mod in stateModifiers) {
                mod.Call(this);
            }
        } catch (Exception e) {
            throw new MatchException($"Failed to modify state by card {LogName}", e);
        }
    }

    public void UpdateState()
    {
        State = new(this);
        Stats?.UpdateState(this);
    }

    #region Counters

    /// <summary>
    /// Add an amount of generic counters
    /// </summary>
    /// <param name="amount">Amount of counters to add</param>
    public async Task AddCounters(int amount) {
        if (!Counters.TryGetValue(Counter.GENERIC_NAME, out Counter? value)) {
            value = new(0);
            Counters.Add(Counter.GENERIC_NAME, value);
        }

        value.Add(amount);
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

    public bool HasLabel(string label) {
        return State.Labels.Contains(label);
    }

    public List<TriggeredAbilityWrapper> GetTriggeredAbilities() {
        return State.TriggeredAbilities;
    }

    public async Task ProcessTrigger(QueuedTrigger trigger) {
        var abilities = GetTriggeredAbilities();
        // TODO prompt the user to order the triggers
        foreach (var ability in abilities) {
            await ability.TryTrigger(this, trigger);
        } 
    }

    public int GetTriggerCount(TriggeredAbility ability) {
        var result = 0;
        if (TriggerCountMap.TryGetValue(ability, out int value))
            result = value;
        return result;
    }

    public void AddToTriggerCount(TriggeredAbility ability) {
        TriggerCountMap.TryAdd(ability, 0);
        TriggerCountMap[ability] = TriggerCountMap[ability] + 1;
    }

    public void ResetTriggers() {
        TriggerCountMap.Clear();
    }

    public bool IsItem() => Card.Template.Type == "Item" || Card.Template.Type == "StartingItem";

    #region Stats

    public bool IsDead() {
        // TODO
        return false;
    }

    public int GetAttack() => Stats!.State.Attack;

    #endregion

    #region Attacking

    public bool IsMiss(int roll) {
        // TODO
        return roll < Stats!.State.Evasion;
    }

    #endregion
}