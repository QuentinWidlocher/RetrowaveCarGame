using Godot;
using static Helpers.Nullable;

public class DebugDraw3D : Control
{
    [Export] public readonly NodePath? CameraPath;
    [Export] public readonly bool Enabled;

    [Export] public readonly NodePath? PlayerPath;

    [Export] public readonly float Width;

    private Camera? _camera;

    private Car? _player;
    private Car Player => ReturnIfNotNull(_player);
    private Camera Camera => ReturnIfNotNull(_camera);

    public override void _Ready()
    {
        if (!Enabled) return;

        if (PlayerPath != null) _player = GetNodeOrNull<Car>(PlayerPath);

        if (CameraPath != null) _camera = GetNodeOrNull<Camera>(CameraPath);
    }

    public override void _Draw()
    {
        if (!Enabled) return;

        var direction = new Vector3(
            Player.Velocity.x,
            Player.Velocity.y,
            Mathf.Abs(Player.Velocity.z)
        );
        DrawArrow(Player.GlobalTransform.origin, direction - Vector3.Forward, Colors.Green);
        DrawArrow(Player.GlobalTransform.origin, Player.Velocity, Colors.Red);
        DrawArrow(Player.GlobalTransform.origin, Player.DirectionVector, Colors.Blue);
    }

    private void DrawArrow(Vector3 start, Vector3 end, Color color)
    {
        var s = Camera.UnprojectPosition(start);
        var e = Camera.UnprojectPosition(start + end);
        DrawLine(s, e, color, Width);
        DrawTriangle(e, s.DirectionTo(e), Width * 2, color);
    }

    public override void _Process(float delta)
    {
        if (!Enabled) return;

        Update();
    }

    private void DrawTriangle(Vector2 pos, Vector2 dir, float size, Color color)
    {
        var a = pos + dir * size;
        var b = pos + dir.Rotated(2 * Mathf.Pi / 3) * size;
        var c = pos + dir.Rotated(4 * Mathf.Pi / 3) * size;
        DrawPolygon(new[] {a, b, c}, new[] {color});
    }
}