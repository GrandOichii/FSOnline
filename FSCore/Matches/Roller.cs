namespace FSCore.Matches;

public interface IRoller {
    public int Roll();
}

public class Roller : IRoller {
    private readonly Random _rng;

    public Roller(Random rng) {
        _rng = rng;
    }

    public int Roll() {
        return _rng.Next();
    }
}