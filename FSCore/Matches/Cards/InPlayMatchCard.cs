namespace FSCore.Matches.Cards;

public class InPlayMatchCard {
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

    public InPlayMatchCard(MatchCard card) {
        Card = card;

        IPID = card.Match.GenerateInPlayID();
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
}