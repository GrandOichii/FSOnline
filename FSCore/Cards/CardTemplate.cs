namespace FSCore.Cards;

/// <summary>
/// Card template, describes a basic card
/// </summary>
public class CardTemplate {
    /// <summary>
    /// Card key
    /// </summary>
    public required string Key { get; set; }
    /// <summary>
    /// Printed card name
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Card type
    /// </summary>
    public required string Type { get; set; }
    /// <summary>
    /// Printed card Health stat
    /// </summary>
    public int Health { get; set; } = -1;
    /// <summary>
    /// Printed card Attack stat
    /// </summary>
    public int Attack { get; set; } = -1;
    /// <summary>
    /// Printed card Evasion stat
    /// </summary>
    public int Evasion { get; set; } = -1;
    /// <summary>
    /// Printed card text
    /// </summary>
    public string Text { get; set; } = "";
    /// <summary>
    /// Lua script for generating a card object
    /// </summary>
    public required string Script { get; set; }
    /// <summary>
    /// Soul value of a card
    /// </summary>
    public int SoulValue { get; set; } = 0;
}