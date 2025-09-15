namespace FSCore.Matches.Players.Attacking;

/// <summary>
/// Attack opportunity
/// </summary>
public interface IAttackOpportunity
{
    // TODO docs
    public bool CanAttackSlot(int slotIdx);
    public IEnumerable<int> GetAvailableAttackSlots(Match match);
    public IAttackOpportunity Copy();
}

/// <summary>
/// Implementation of <c>IAttackOpportunity</c>
/// </summary>
public class AttackOpportunity : IAttackOpportunity
{
    public static AttackOpportunity Default => new()
    {
        AllowedAttackSlots = []
    };

    public static AttackOpportunity TopOfDeckOnly => new()
    {
        AllowedAttackSlots = [-1]
    };

    public required List<int> AllowedAttackSlots { get; set; }

    public bool CanAttackSlot(int slotIdx)
    {
        return AllowedAttackSlots.Count == 0 || AllowedAttackSlots.Contains(slotIdx);
    }

    public IEnumerable<int> GetAvailableAttackSlots(Match match)
    {
        return AllowedAttackSlots.Count == 0
            ? [-1, .. match.MonsterSlots.Select(s => s.Idx)]
            : AllowedAttackSlots;
    }

    public IAttackOpportunity Copy()
    {
        return new AttackOpportunity()
        {
            AllowedAttackSlots = AllowedAttackSlots
        };
    }
}