namespace FSCore.Matches;

public enum ModificationLayer
{
    COIN_GAIN_AMOUNT = 1,
    ROLL_REPLACEMENT_EFFECTS = 2,
    LOOT_AMOUNT = 3,
    HAND_CARD_VISIBILITY = 4,
    LOOT_PLAY_RESTRICTIONS = 5,
    ITEM_ACTIVATION_RESTRICTIONS = 6,
    PURCHASE_COST = 7,
    LAST = 8,
    ITEM_DESTRUCTION_REPLACEMENT_EFFECTS = 9,
    MOD_MAX_LOOT_PLAYS = 10,
    PLAYER_MAX_HEALTH = 11,
    PLAYER_ATTACK = 12,
    DEATH_PENALTY_MODIFIERS = 13,
    DEATH_PENALTY_REPLACEMENT_EFFECTS = 14,
    MONSTER_HEALTH = 15,
    MONSTER_EVASION = 16,
    MONSTER_ATTACK = 17,
    DAMAGE_RECEIVED_MODIFICATORS = 18,
    ROLL_RESULT_MODIFIERS = 19,
    PLAYER_SOUL_COUNT = 20,
    PLAYER_ATTACK_OPPORTUNITIES = 21,
}

/// <summary>
/// State modifier
/// </summary>
public interface IStateModifier {
    public void UpdateState();
    public void Modify(ModificationLayer layer);
}