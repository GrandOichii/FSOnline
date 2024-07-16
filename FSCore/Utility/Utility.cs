using System.Text.Json;

namespace FSCore.Utility;

/// <summary>
/// General utility class
/// </summary>
public static class Common {
    /// <summary>
    /// Returns the shuffled list
    /// </summary>
    /// <param name="list">List</param>
    /// <param name="rnd">Random number generator</param>
    /// <typeparam name="T">Type of contained value</typeparam>
    /// <returns>The shuffled list</returns>
    static public LinkedList<T> Shuffled<T>(LinkedList<T> list, Random rnd) {
        return new LinkedList<T>(list.OrderBy(a => rnd.Next()));
    }
}