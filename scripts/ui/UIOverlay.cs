using Godot;
using static Helpers.Nullable;

public class UIOverlay : CanvasLayer
{
    [Export] public readonly NodePath? PlayerPath;

    private Car? _player;

    private ProgressBar? _speedometer;
    private Car Player => ReturnIfNotNull(_player);
    private ProgressBar Speedometer => ReturnIfNotNull(_speedometer);

    public override void _Ready()
    {
        _player = GetNodeOrNull<Car>(PlayerPath);
        _speedometer = GetNodeOrNull<ProgressBar>("Speedometer");

        Speedometer.MinValue = Player.BaseMaxSpeed;
        Speedometer.MaxValue = Player.MaxMaxSpeed;
    }

    public override void _Process(float delta)
    {
        Speedometer.Value = Mathf.Lerp((float) Speedometer.Value, Player.MaxSpeed, .5f);
    }
}