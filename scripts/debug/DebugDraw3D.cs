using Godot;
using static Godot.GD;
using static Helpers.Nullable;

public class DebugDraw3D : Control
{

    [Export]
    bool Enabled = false;

    [Export]
    NodePath? PlayerPath;

    [Export]
    NodePath? CameraPath;

    [Export]
    float Width;

    Car? _Player;
    Car Player => ReturnIfNotNull(_Player);

    Camera? _Camera;
    Camera Camera => ReturnIfNotNull(_Camera);

    public override void _Ready()
    {
        if (!Enabled)
        {
            return;
        }

        if (PlayerPath != null)
        {
            _Player = GetNodeOrNull<Car>(PlayerPath);
        }

        if (CameraPath != null)
        {
            _Camera = GetNodeOrNull<Camera>(CameraPath);
        }
    }

    public override void _Draw()
    {
        if (!Enabled)
        {
            return;
        }
        var color = Colors.Green;
        var start = Camera.UnprojectPosition(Player.GlobalTransform.origin);
        var end = Camera.UnprojectPosition(Player.GlobalTransform.origin + Player.Velocity);
        DrawLine(start, end, color, Width);
        DrawTriangle(end, start.DirectionTo(end), Width * 2, color);
    }

    public override void _Process(float delta)
    {
        if (!Enabled)
        {
            return;
        }

        Update();
    }

    private void DrawTriangle(Vector2 pos, Vector2 dir, float size, Color color)
    {
        var a = pos + dir * size;
        var b = pos + dir.Rotated(2 * Mathf.Pi / 3) * size;
        var c = pos + dir.Rotated(4 * Mathf.Pi / 3) * size;
        DrawPolygon(new Vector2[] { a, b, c }, new Color[] { color });
    }
}