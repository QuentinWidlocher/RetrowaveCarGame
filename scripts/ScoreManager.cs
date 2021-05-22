using Godot;
using static Helpers.Nullable;

public class ScoreManager: Node
{
    public int Score { get; private set; }

    private RichTextLabel? _label;
    private RichTextLabel Label => ReturnIfNotNull(_label);

    [Signal] public delegate void PointsAdded(int value, int total);
    [Signal] public delegate void PointsSubtracted(int value, int total);

    public override void _Ready()
    {
        Score = 0;
        _label = GetTree().Root.FindNode("TotalScoreLabel", owned: false) as RichTextLabel;
        UpdateLabel();
    }

    public void AddPoints(int points)
    {
        Score += points;
        EmitSignal(nameof(PointsAdded), points, Score);
        UpdateLabel();
    }
    
    public void SubtractPoints(int points)
    {
        Score -= points;
        EmitSignal(nameof(PointsSubtracted), points, Score);
        UpdateLabel();
    }

    public void UpdateLabel() => Label.BbcodeText = $"[right]Score : {Score}pts[/right]";
}