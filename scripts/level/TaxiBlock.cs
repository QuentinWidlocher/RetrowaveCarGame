using Godot;
using Helpers;
using static Helpers.Nullable;
using System;

public class TaxiBlock : RoadBlock
{
    LevelManager? _LevelManager;
    LevelManager LevelManager => ReturnIfNotNull(_LevelManager);

    Taxi? _Taxi;
    Taxi Taxi => ReturnIfNotNull(_Taxi);

    public void OnTaxiCollided(Node body)
    {
        LevelManager.OnCarTouchedTaxi();
        Taxi.QueueFree();
    }

    public override void _Ready()
    {
        base._Ready();
        _LevelManager = GetNodeOrNull<LevelManager>("/root/LevelManager");
        _Taxi = _Taxi ?? GetNodeOrNull<Taxi>("Taxi");
    }

    public override void Enable()
    {
        base.Enable();

        _Taxi = _Taxi ?? GetNodeOrNull<Taxi>("Taxi");

        var areaCollisionShape = Taxi.GetNodeOrNull<CollisionShape>("Area/CollisionShape");
        if (areaCollisionShape != null)
        {
            areaCollisionShape.Disabled = false;
        }

        var physicsCollisionShape = Taxi.GetNodeOrNull<CollisionShape>("CollisionShape");
        if (physicsCollisionShape != null)
        {
            physicsCollisionShape.Disabled = false;
        }

        Taxi.Connect(nameof(Taxi.BodyEntered), this, nameof(OnTaxiCollided));

        Taxi.SetOrigin(new Vector3(
            (GD.Randf() - .5f) / 1.5f,
            Taxi.Transform.origin.y,
            (GD.Randf() - .5f) / 1.5f
        ));
    }
}
