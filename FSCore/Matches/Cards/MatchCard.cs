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

    #region Effects

    public List<CardEffect> CardEffects { get; }

    #endregion

    /// <summary>
    /// Name of the card that will be used when logging using system logger
    /// </summary>
    public string LogName => $"{Template.Name} [{ID}]";

    public MatchCard(Match match, CardTemplate template) {
        Match = match;
        Template = template;

        // TODO create script
        CardEffects = new();
        ExecuteScript();

        ID = match.GenerateCardID();

        // Initial state
        State = new(this);
    }

    private void ExecuteScript() {
        Match.LState.DoString(Template.Script);

        var creationF = LuaUtility.GetGlobalF(Match.LState, CARD_CREATION_FNAME);
        var returned = creationF.Call();
        var data = LuaUtility.GetReturnAs<LuaTable>(returned);

        var effectsTable = LuaUtility.TableGet<LuaTable>(data, "Effects");

        foreach (var o in effectsTable.Values) {
            var table = o as LuaTable
                ?? throw new MatchException($"Expected card effect to be a table, but found {o.GetType()}")
            ;
            CardEffects.Add(new(this, table));
        }
    }

    public bool IsCardType(string type) {
        return Template.Type == type;
    }

    public bool IsLoot() => IsCardType("Loot");

    public List<string> Names() => State.Names;

    public bool IsNamed(string name) => State.Names.Contains(name);

    public void ExecuteCardEffects(StackEffect stackEffect) {
        // TODO targets (and costs?)

        foreach (var effect in CardEffects)
            effect.ExecuteEffect(stackEffect);
    }
}