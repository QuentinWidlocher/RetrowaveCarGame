using Godot;
using static Helpers.Nullable;
using static Helpers.SpatialExtentions;

public class KillZone : Area
{
    [Export] public readonly NodePath? CarPath;

    private Car? _car;

    private LevelManager? _levelManager;
    private Car Car => ReturnIfNotNull(_car);
    private LevelManager LevelManager => ReturnIfNotNull(_levelManager);

    public override void _Ready()
    {
        _car = GetNodeOrNull<Car>(CarPath);
        _levelManager = GetNodeOrNull<LevelManager>("/root/LevelManager");

        Connect("body_entered", this, nameof(OnBodyEntered));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        this.SetOriginZ(Car.Transform.origin.z);
        this.SetOriginX(Car.Transform.origin.x);
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Car) LevelManager.OnCarTouchedKillZone();
    }
}