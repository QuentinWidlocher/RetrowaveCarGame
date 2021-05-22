using Godot;

public class Point : RichTextLabel
{
    [Export] public readonly float TotalTimeVisible = 1;
    
    private float TimeVisible;
    private bool ShouldShowPoints => TimeVisible > 0;
    public Vector2 Direction = Vector2.Up;
    private bool Initialized;
    public Vector2 SpawnPosition = new Vector2();

    public override void _Ready()
    {
        TimeVisible = TotalTimeVisible;
        Visible = true;
        Initialized = true;
    }

    public override void _Process(float delta)
    {
        if (ShouldShowPoints)
        {
            RectPosition = SpawnPosition * Direction * TimeVisible * delta;
            TimeVisible -= delta;
        }
        else if (Initialized)
        {
            Visible = false;
            // QueueFree();
        }
    }

    public void Init(Vector2 position, string value, Vector2 direction)
    {        

    }
}