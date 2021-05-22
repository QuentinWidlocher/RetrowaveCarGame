using Godot;
using static Helpers.Nullable;

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

    private ScoreManager? _scoreManager;
    private ScoreManager ScoreManager => ReturnIfNotNull(_scoreManager);

    public override void _Ready()
    {
        _scoreManager = GetNodeOrNull<ScoreManager>("/root/ScoreManager");
    }

    public void OnCarTouchedCoin()
    {
        ScoreManager.AddPoints(50);
        EmitSignal(nameof(CarTouchedCoin));
    }

    public void OnCarTouchedTaxi()
    {
        ScoreManager.SubtractPoints(100);
        EmitSignal(nameof(CarTouchedTaxi));
    }

    public void OnCarTouchedJumpPad()
    {
        ScoreManager.AddPoints(10);
        EmitSignal(nameof(CarTouchedJumpPad));
    }

    public void OnCarTouchedKillZone()
    {
        GetTree().Notification(MainLoop.NotificationWmQuitRequest);
        EmitSignal(nameof(CarTouchedKillZone));
    }
}