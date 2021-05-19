using Godot;
using Helpers;
using static Helpers.Nullable;
using System;

public class LevelManager : Node
{
    [Signal]
    public delegate void CarTouchedCoin();

    [Signal]
    public delegate void CarTouchedTaxi();

    [Signal]
    public delegate void CarTouchedJumpPad();

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
        GD.Print("OnCarTouchedJumpPad");
        EmitSignal(nameof(CarTouchedJumpPad));
    }
}