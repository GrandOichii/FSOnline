namespace FSCore.Tests.Setup;

public class ProgrammedRollerBuilder
{
    private readonly ProgrammedRoller _result = new();

    public ProgrammedRollerBuilder Default(int value)
    {
        _result.DefaultRoll = value;
        return this;
    }

    public ProgrammedRollerBuilder Then(int value)
    {
        _result.Rolls.Enqueue(value);
        return this;
    }

    public ProgrammedRollerBuilder ThenNTimes(int value, int n)
    {
        for (int i = 0; i < n; ++i)
            _result.Rolls.Enqueue(value);
        return this;
    }

    public ProgrammedRoller Build() => _result;
}

public class ProgrammedRoller : IRoller
{
    public int DefaultRoll = -1;
    public Queue<int> Rolls { get; } = new();

    public int Roll()
    {
        if (Rolls.TryDequeue(out var result))
        {
            return result;
        }
        if (DefaultRoll > 0)
        {
            return DefaultRoll;
        }
        throw new Exception($"Roller queue is empty and default roll is not set!");
    }
}