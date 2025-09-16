using System.Diagnostics;

namespace FSCore.Matches.Players.Attacking;

public class AttackOpportunityManager
{
    public Player _player;

    /// <summary>
    /// Attacks that can be used this turn
    /// </summary>
    public Dictionary<IAttackOpportunity, int> Available { get; }
    /// <summary>
    /// Attaks that have been used this turn
    /// </summary>
    public Dictionary<IAttackOpportunity, int> Used { get; }
    public StateT State { get; private set; }

    public Queue<IAttackOpportunity> RequiredAttackQueue { get; set; }


    public AttackOpportunityManager(Player player)
    {
        _player = player;

        Available = [];
        Used = [];

        State = new(this);
        RequiredAttackQueue = [];
    }

    public bool HasToAttack()
    {
        return _player.Match.Stack.Effects.Count == 0 && RequiredAttackQueue.Count > 0;
    }

    public void AddRequired(IAttackOpportunity ao)
    {
        RequiredAttackQueue.Enqueue(ao);
    }

    public void UpdateState()
    {
        State = new(this);
    }

    public void Clear()
    {
        Used.Clear();
        Available.Clear();
        RequiredAttackQueue.Clear();
    }

    public void ProcessAttackedSlot(int chosenSlotIdx)
    {
        if (RequiredAttackQueue.TryPeek(out var requiredTop))
        {
            if (!requiredTop.CanAttackSlot(chosenSlotIdx))
            {
                throw new Exception($"Mismatch of chosen monster slot idx for attacking and top of required attack queue"); // TODO type
            }
            RequiredAttackQueue.Dequeue();
            return;
        } 

        IAttackOpportunity? bestAo = null;
        int bestAoSize = -1;
        foreach (var pair in State.Available)
        {
            var ao = pair.Key;
            if (!ao.CanAttackSlot(chosenSlotIdx))
                continue;
            var diff = 0;
            if (Used.TryGetValue(ao, out int value))
                diff = value;
            var available = pair.Value - diff;
            if (available <= 0)
                continue;
            if (bestAo == null || available < bestAoSize)
            {
                bestAo = ao;
                bestAoSize = available;
            }
        }

        if (bestAo is null)
        {
            throw new Exception($"Player {_player.LogName} tried to attack monster slot {chosenSlotIdx}, which they can't");// TODO type
        }

        if (!Used.TryGetValue(bestAo, out int v))
        {
            v = 0;
            Used.Add(bestAo, v);
        }
        Used[bestAo] = ++v;
    }

    public void Add(int amount, IAttackOpportunity? ao = null)
    {
        ao ??= AttackOpportunity.Default;

        if (!Available.ContainsKey(ao)) {
            Available[ao] = 0;
        }
        Available[ao] += amount;
    }

    public void AddForTurn()
    {
        Add(_player.Match.Config.AttackCountDefault);
    }

    public IEnumerable<int> GetAvailableSlots()
    {
        if (RequiredAttackQueue.TryPeek(out var requiredTop))
        {
            return requiredTop.GetAvailableAttackSlots(_player.Match);
        }

        HashSet<int> result = [];

        foreach (var pair in State.Available)
        {
            var ao = pair.Key;
            var available = pair.Value;
            if (!Used.TryGetValue(ao, out int value) || available > value)
            {
                foreach (var slotIdx in ao.GetAvailableAttackSlots(_player.Match))
                {
                    result.Add(slotIdx);
                }
            }
        }

        return result;
    }

    public int Count() {
        int result = 0;

        foreach (var pair in State.Available)
        {
            var ao = pair.Key;
            var available = pair.Value;
            var diff = 0;
            if (Used.TryGetValue(ao, out int value))
                diff = value;
            result += available - diff;
        }

        return result + RequiredAttackQueue.Count;
    }



    public class StateT
    {
        public Dictionary<IAttackOpportunity, int> Available { get; set; }

        public StateT(AttackOpportunityManager parent)
        {
            Available = new(parent.Available);
        }

        public void Add(IAttackOpportunity ao, int amount)
        {
            if (!Available.ContainsKey(ao))
            {
                Available[ao] = 0;
            }
            Available[ao] += amount;
        }

        public void Add(int amount)
        {
            Add(AttackOpportunity.Default, amount);
        }

        public void AddTopOfDeckOnly(int amount)
        {
            Add(AttackOpportunity.TopOfDeckOnly, amount);
        }
    }
}