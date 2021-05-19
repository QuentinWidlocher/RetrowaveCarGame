using Godot;
using System;
using Helpers;

public class RoadBlock : Spatial
{

    [Signal]
    delegate void LoadZoneEntered(Node body);

    private void OnLoadZoneEntered(Node body)
    {
        if (body is Car)
        {
            EmitSignal(nameof(LoadZoneEntered), body);
        }
    }

    public virtual void Enable()
    {
        var collision = GetNode<CollisionShape>("RoadMesh/RoadBody/RoadCollisionShape");
        if (collision != null)
        {
            collision.Disabled = false;
        }

        var loadZone = GetNodeOrNull("LoadZone");

        if (loadZone != null)
        {
            loadZone.Connect("body_entered", this, "OnLoadZoneEntered");
        }

        SetProcess(true);

        GD.Randomize();
    }
}
