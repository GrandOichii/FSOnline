namespace FSCore.Matches;

public class StackData {
    /// <summary>
    /// Index of the player with priority
    /// </summary>
    public int PriorityIdx { get; set; }

    public StackData(Stack stack) {
        PriorityIdx = stack.PriorityIdx;
    }
}