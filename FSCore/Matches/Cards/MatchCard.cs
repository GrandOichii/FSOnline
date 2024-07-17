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

    #endregion

    /// <summary>
    /// Name of the card that will be used when logging using system logger
    /// </summary>
    public string LogName => $"{Template.Name} [{ID}]";

    public MatchCard(Match match, CardTemplate template) {
        Match = match;
        Template = template;

        // script execution
        Match.LState.DoString(Template.Script);
        var creationF = LuaUtility.GetGlobalF(Match.LState, CARD_CREATION_FNAME);
        var returned = creationF.Call();
        var data = LuaUtility.GetReturnAs<LuaTable>(returned);

        // effects
        Effects = new(LuaUtility.TableGet<LuaTable>(data, "Effects"));

        // activated abilities
        var activatedAbilities = LuaUtility.TableGet<LuaTable>(data, "ActivatedAbilities");
        ActivatedAbilities = activatedAbilities.Values.Cast<object>()
            .Select(
                o => new ActivatedAbility(
                    o as LuaTable 
                        ?? throw new MatchException($"Expected activated ability to be a table, but found {o.GetType()}")
                )
            )
            .ToList();

        ID = match.GenerateCardID();

        // Initial state
        State = new(this);
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