using Godot;
using static Godot.GD;

public class RoadBlock : Spatial
{
    private void OnLoadZoneEntered(Node body)
    {
        if (body is Car) EmitSignal(nameof(LoadZoneEntered), body);
    }

    public virtual void Enable()
    {
        var collision = GetNode<CollisionShape>("RoadMesh/RoadBody/RoadCollisionShape");
        if (collision != null) collision.Disabled = false;

        var loadZone = GetNodeOrNull("LoadZone");

        loadZone?.Connect("body_entered", this, nameof(OnLoadZoneEntered));

        SetProcess(true);

        Randomize();
    }

    [Signal]
    private delegate void LoadZoneEntered(Node body);
}