using System.Diagnostics;

namespace FSCore.Matches;

public class StateModFunc {
    private readonly LuaFunction _func;
    public string Text { get; }

    public StateModFunc(LuaTable table) {
        _func = LuaUtility.TableGet<LuaFunction>(table, "func");
        Text = LuaUtility.TableGet<string>(table, "text");
    }

    public void Modify(object originator) {
        _func.Call(originator);
    }
}