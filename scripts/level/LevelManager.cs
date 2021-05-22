using Godot;

public class LevelManager : Node
{
    [Signal]
    public delegate void CarTouchedCoin();

    [Signal]
    public delegate void CarTouchedJumpPad();

    [Signal]
    public delegate void CarTouchedKillZone();

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
        EmitSignal(nameof(CarTouchedJumpPad));
    }

    public void OnCarTouchedKillZone()
    {
        GetTree().Notification(MainLoop.NotificationWmQuitRequest);
        EmitSignal(nameof(CarTouchedKillZone));
    }
}