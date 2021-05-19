using Godot;
using Helpers;
using static Helpers.Nullable;
using static Godot.GD;

public class TaxiBlock : RoadBlock
{
    private LevelManager? _levelManager;

    private Taxi? _taxi;
    private LevelManager LevelManager => ReturnIfNotNull(_levelManager);
    private Taxi Taxi => ReturnIfNotNull(_taxi);

    public void OnTaxiCollided(Node body)
    {
        LevelManager.OnCarTouchedTaxi();
        Taxi.QueueFree();
    }

    public override void _Ready()
    {
        base._Ready();
        _levelManager = GetNodeOrNull<LevelManager>("/root/LevelManager");
        _taxi ??= GetNodeOrNull<Taxi>("Taxi");
    }

    public override void Enable()
    {
        base.Enable();

        _taxi ??= GetNodeOrNull<Taxi>("Taxi");

        var areaCollisionShape = Taxi.GetNodeOrNull<CollisionShape>("Area/CollisionShape");
        if (areaCollisionShape != null) areaCollisionShape.Disabled = false;

        var physicsCollisionShape = Taxi.GetNodeOrNull<CollisionShape>("CollisionShape");
        if (physicsCollisionShape != null) physicsCollisionShape.Disabled = false;

        Taxi.Connect(nameof(Taxi.BodyEntered), this, nameof(OnTaxiCollided));

        Taxi.SetOrigin(new Vector3(
            (Randf() - .5f) / 1.5f,
            Taxi.Transform.origin.y,
            (Randf() - .5f) / 1.5f
        ));
    }
}