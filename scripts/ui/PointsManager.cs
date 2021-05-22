using Godot;
using static Helpers.Nullable;

public class PointsManager : Node
{
    [Export] public readonly NodePath? CameraPath;
    [Export] public readonly NodePath? PlayerPath;
    
    private ScoreManager? _scoreManager;
    private ScoreManager ScoreManager => ReturnIfNotNull(_scoreManager);
    
    private Camera? _camera;
    private Camera Camera => ReturnIfNotNull(_camera);
    
    private Car? _player;
    private Car Player => ReturnIfNotNull(_player);
    
    private Point? _point;
    private Point Point => ReturnIfNotNull(_point);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (CameraPath != null) _camera = GetNodeOrNull<Camera>(CameraPath);
        if (PlayerPath != null) _player = GetNodeOrNull<Car>(PlayerPath);
        
        _scoreManager = GetNodeOrNull<ScoreManager>("/root/ScoreManager");
        ScoreManager.Connect(nameof(ScoreManager.PointsAdded), this, nameof(OnPointsAdded));
        ScoreManager.Connect(nameof(ScoreManager.PointsSubtracted), this, nameof(OnPointsSubtracted));

        _point = GetNodeOrNull<Point>("Point");
        Point.Visible = false;
    }

    private void OnPointsAdded(int value, int _)
    {
        CreatePoint(Camera.UnprojectPosition(Player.GlobalTransform.origin), $"+{value}", Vector2.Up);
    }

    private void OnPointsSubtracted(int value, int _)
    {
        CreatePoint(Camera.UnprojectPosition(Player.GlobalTransform.origin), $"-{value}", Vector2.Down);
    }

    private void CreatePoint(Vector2 position, string value, Vector2 direction)
    {
        var newPoint = (Point) Point.Duplicate();
        newPoint.SpawnPosition = position;
        newPoint.Direction = direction;
        newPoint.BbcodeText = value;
        AddChild(newPoint);
    }
}
