using Godot;
using static Helpers.Nullable;

public class TighteningChaseCamera : ChaseCamera
{
    [Export] public readonly NodePath? CarPath;
    [Export] public readonly float MaxLerpSpeed;

    private Car? _car;
    private Car Car => ReturnIfNotNull(_car);

    public override void _Ready()
    {
        base._Ready();

        _car = GetNodeOrNull<Car>(CarPath);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        LerpSpeed = Car.MaxSpeed / Car.MaxMaxSpeed * MaxLerpSpeed;
    }
}