using Godot;
using static Godot.GD;

public class LevelManager : Node
{
    [Signal]
    public delegate void CarTouchedCoin();

    [Signal]
    public delegate void CarTouchedJumpPad();

    [Signal]
    public delegate void CarTouchedTaxi();

    public void OnCarTouchedCoin()
    {
        EmitSignal(nameof(CarTouchedCoin));
    }

    public void OnCarTouchedTaxi()
    {
        EmitSignal(nameof(CarTouchedTaxi));
    }

    public void OnCarTouchedJumpPad()
    {
        Print("OnCarTouchedJumpPad");
        EmitSignal(nameof(CarTouchedJumpPad));
    }
}