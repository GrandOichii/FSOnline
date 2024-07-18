using System.Collections.Immutable;

namespace FSCore.Matches.Cards;

public class MatchCard {
    /// <summary>
    /// Name of card creation function
    /// </summary>
    private readonly static string CARD_CREATION_FNAME = "_Create";

    /// <summary>
    /// Parent match
    /// </summary>
    public Match Match { get; }
    /// <summary>
    /// Template, used to create the card
    /// </summary>
    public CardTemplate Template { get; }
    /// <summary>
    /// Card ID
    /// </summary>
    public string ID { get; }
    /// <summary>
    /// State
    /// </summary>
    public MatchCardState State { get; private set; }

    #region Effects and Abilities

    /// <summary>
    /// Loot effects
    /// </summary>
    public EffectList Effects { get; }
    /// <summary>
    /// Activated abilities
    /// </summary>
    public List<ActivatedAbility> ActivatedAbilities { get; }
    public Dictionary<ModificationLayer, List<LuaFunction>> StateModifiers { get; }

    #endregion

    /// <summary>
    /// Card labels
    /// </summary>
    public List<string> Labels { get; }

    /// <summary>
    /// Name of the card that will be used when logging using system logger
    /// </summary>
    public string LogName => $"{Template.Name} [{ID}]";

    public MatchCard(Match match, CardTemplate template) {
        Match = match;
        Template = template;

        // script execution
        LuaTable data;
        try {
            Match.LState.DoString(Template.Script);
            var creationF = LuaUtility.GetGlobalF(Match.LState, CARD_CREATION_FNAME);
            var returned = creationF.Call();
            data = LuaUtility.GetReturnAs<LuaTable>(returned);
        } catch (Exception e) {
            throw new MatchException($"Failed to run card creation function in card {template.Name}", e);
        }

        // effects
        try {
            Effects = new(LuaUtility.TableGet<LuaTable>(data, "Effects"));
        } catch (Exception e) {
            throw new MatchException($"Failed to get effects for card {template.Name}", e);
        }

        // activated abilities
        try {
            var activatedAbilities = LuaUtility.TableGet<LuaTable>(data, "ActivatedAbilities");
            ActivatedAbilities = activatedAbilities.Values.Cast<object>()
                .Select(
                    o => new ActivatedAbility(
                        o as LuaTable 
                            ?? throw new MatchException($"Expected activated ability to be a table, but found {o.GetType()}")
                    )
                )
                .ToList();
        } catch (Exception e) {
            throw new MatchException($"Failed to get activated abilities for card {template.Name}", e);
        }

        // labels
        try {
            Labels = LuaUtility.TableGet<LuaTable>(data, "Labels")
                .Values.Cast<string>().ToList();
        } catch (Exception e) {
            throw new MatchException($"Failed to get labels for card {template.Name}", e);
        }

        // state modifiers
        try {
            StateModifiers = ExtractStateModifiers(data);
        } catch (Exception e) {
            throw new MatchException($"Failed to get state modifiers for card {template.Name}", e);
        }

        ID = match.GenerateCardID();

        // Initial state
        State = new(this);
    }

    public Dictionary<ModificationLayer, List<LuaFunction>> ExtractStateModifiers(LuaTable data) {
        var result = new Dictionary<ModificationLayer, List<LuaFunction>>();
        var table = LuaUtility.TableGet<LuaTable>(data, "StateModifiers");

        foreach (var keyRaw in table.Keys) {
            var key = (ModificationLayer)Convert.ToInt32(keyRaw);
            var modifiers = (
                table[keyRaw] as LuaTable
                    ?? throw new MatchException($"Expected a table while parsing state modifiers for layer {key}, but found ({table[keyRaw].GetType()})")
            ).Values.Cast<LuaFunction>().ToList();
            result.Add(key, modifiers);
        }

        return result;
    }

    public bool IsCardType(string type) {
        return Template.Type == type;
    }

    public bool IsLoot() => IsCardType("Loot");

    public List<string> Names() => State.Names;

    public bool IsNamed(string name) => State.Names.Contains(name);

    public void ExecuteCardEffects(StackEffect stackEffect) {
        try {
            Effects.Execute(stackEffect);
        } catch (Exception e) {
            throw new MatchException($"Failed to execute CardEffect of card {LogName}", e);
        }
    }

}