namespace FSCore.Matches.Cards;

public class InPlayMatchCard : IStateModifier
{
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
    public List<string> Labels { get; }

    public Dictionary<TriggeredAbility, int> TriggerCountMap { get; }

    public InPlayMatchCard(MatchCard card) {
        Card = card;

        IPID = card.Match.GenerateInPlayID();
        Counters = [];
        TriggerCountMap = [];
        Labels = [];

        // Initial state
        State = new(this);
        Stats = Card.Template.Health == -1 ? null : new();
    }

    public virtual Player? GetOwner() => null;

    public string LogName => $"{Card.Template.Name} [{IPID} ({Card.ID})]";

    public virtual string GetFormattedName() {
        return $"{{ip:{LogName}}}";
    }

    /// <summary>
    /// Tap the card
    /// </summary>
    public Task Tap() {
        Tapped = true;

        // TODO add update
        return Task.CompletedTask;
    }

    /// <summary>
    /// Untap the card
    /// </summary>
    /// <returns></returns>
    public Task Untap() {
        Tapped = false;

        // TODO add update
        return Task.CompletedTask;
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
        // TODO
        return true;
    }

    public ActivatedAbilityWrapper GetActivatedAbility(int idx) {
        if (idx < 0) throw new MatchException($"Tried to get ability with idx {idx}");
        var abilities = GetActivatedAbilities();
        if (abilities.Count <= idx) throw new MatchException($"Tried to get ability with idx {idx} (number of abilities: {abilities.Count})");

        return abilities[idx];
    }

    public List<StateModFunc> GetStateModifiers(ModificationLayer layer) {
        // TODO use abilities from state
        if (!Card.StateModifiers.TryGetValue(layer, out List<StateModFunc>? value))
            return [];
        return value;
    }

    public void Modify(ModificationLayer layer)
    {
        try {
            var stateModifiers = GetStateModifiers(layer);
            foreach (var mod in stateModifiers) {
                mod.Modify(this);
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
    public Task AddCounters(int amount) {
        if (!Counters.TryGetValue(Counter.GENERIC_NAME, out Counter? value)) {
            value = new(0);
            Counters.Add(Counter.GENERIC_NAME, value);
        }

        value.Add(amount);
        Card.Match.LogDebug("Placed {Amount} generic counters on {CardLogName}", amount, LogName);

        return Task.CompletedTask;
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

    public Task RemoveCounters(int amount) {
        // TODO this removes generic counters first, then picks at random - change to player choice
        Card.Match.LogDebug("Requested to remive {Amount} counters from {CardLogName}", amount, LogName);

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

        return Task.CompletedTask;
    }

    #endregion

    public bool HasLabel(string label) {
        return State.Labels.Contains(label) || Labels.Contains(label);
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

    public int GetAttack() => Stats!.State.Attack;

    #endregion

    #region Attacking

    public bool IsMiss(int roll) {
        // TODO
        return roll < Stats!.GetEvasion();
    }

    #endregion

    public async Task ProcessDamage(int amount, DamageStackEffect effect) {
        if (Stats is null)
            throw new MatchException($"Tried to deal damage to card {LogName}, which has no stats");

        await Stats.ProcessDamage(this, amount, effect);
    }

    public Task HealToMax() {
        if (Stats is null) return Task.CompletedTask;

        Stats.Damage = 0;
        Stats.DeathSource = null;
        Stats.IsDead = false;

        return Task.CompletedTask;
    }

    public async Task CheckDead() {
        if (Stats is null || Stats.DeathSource is null) return;

        await PushDeath(Stats.DeathSource);

        Stats.DeathSource = null;
    }

    public async Task ProcessDeath(StackEffect deathSource) {
        if (Stats is null)
            throw new MatchException($"Tried to push death of card {LogName} onto the stack, which has no stats");

        if (Stats.IsDead) return;

        // TODO death preventors

        // TODO trigger "when a monster dies"
        // TODO trigger "when a monster dies, after gaining rewards"
        // TODO add monster as a soul card if able
        // TODO otherwise discard the monster
        // TODO check queued monsters, if none, refill slot

        // TODO catch exceptions
        // TODO add back
        // foreach (var preventor in DeathPreventors) {
        //     var returned = preventor.Call(deathSource);
        //     if (LuaUtility.GetReturnAsBool(returned)) {
        //         Match.LogDebug("Death of player {LogName} was prevented", LogName);
        //         DeathPreventors.Remove(preventor);
        //         return;
        //     }
        // }

        // TODO
        // Abilities that trigger when a monster dies, but not after gaining rewards, trigger here.
        // The active player gains any rewards from the monster.
        // Abilities that trigger when a monster dies, after gaining rewards, trigger here.
        // If the monster has a soul icon, it becomes a soul and the active player gains it. Otherwise, it is moved to discard.
        // Refill monster slots, if applicable.

        Stats.IsDead = true;

        // TODO death replacement effects
        Card.Match.LogDebug("Card {LogName} dies", LogName);

        await Card.Match.Emit("card_death", new() {
            { "Card", this },
            { "Source", deathSource },
        });

        await Card.Match.ReloadState();

        await PushRewards(deathSource);

        // TODO add update

        await Card.Match.RemoveFromPlay(this);

        if (Card.Template.SoulValue > 0) {
            await Card.Match.CurrentPlayer.AddSoulCard(new(Card));
        } else {
            await Card.Match.PlaceIntoDiscard(Card);
        }
        
        Card.Match.DeadCards.Add(Card);
    }

    public List<RewardAbility> GetRewards() {
        return Card.Rewards;
    }

    public async Task PushRewards(StackEffect deathSource) {
        // TODO use death source
        // TODO? this uses only the first reward, change
        Card.Match.LogDebug("Pusing rewards of card {CardLogName} to the stack", LogName);
        
        var rewards = GetRewards();
        if (rewards.Count == 0) return;

        var reward = rewards[0];
        var effect = new RewardStackEffect(this, reward, Card.Match.CurPlayerIdx);

        await Card.Match.PlaceOnStack(effect);

        var payed = reward.PayCosts(this, Card.Match.CurrentPlayer, effect);
        if (!payed) {
            throw new MatchException($"Player {Card.LogName} decided not to pay reward costs for reward ability of card {LogName}");
        }
    }


    public async Task PushDeath(StackEffect deathSource) {
        Card.Match.LogDebug("Death of card {LogName} is pushed onto the stack", LogName);

        var effect = new CardDeathStackEffect(this, deathSource);
        await Card.Match.PlaceOnStack(effect);
    }

    public Task AddDamagePreventors(int amount) {
        if (Stats is null) throw new MatchException($"Tried to add damage preventors to non-living item {LogName}");
        
        Stats.AddDamagePreventors(amount);
        // TODO? update
        return Task.CompletedTask;
    }

}