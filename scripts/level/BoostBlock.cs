using Godot;
using Helpers;
using static Helpers.Nullable;

public class BoostBlock : RoadBlock
{
    [Export] public readonly float Amplitude = 5;
    [Export] public readonly float Speed = 400f;

    private AnimationPlayer? _animationPlayer;

    private CSGCylinder? _coinMesh;

    private CollisionShape? _collider;

    private LevelManager? _levelManager;
    private CollisionShape Collider => ReturnIfNotNull(_collider);
    private LevelManager LevelManager => ReturnIfNotNull(_levelManager);
    private AnimationPlayer AnimationPlayer => ReturnIfNotNull(_animationPlayer);

    public override void _Ready()
    {
        base._Ready();

        _levelManager = GetNodeOrNull<LevelManager>("/root/LevelManager");
        _animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        _coinMesh = GetNodeOrNull<CSGCylinder>("Area/CollisionShape/CSGCylinder");

        AnimationPlayer.CurrentAnimation = "Floating";
    }

    public override void Enable()
    {
        base.Enable();

        _collider = GetNodeOrNull<CollisionShape>("Area/CollisionShape");
        var area = GetNodeOrNull<Area>("Area");

        if (area != null)
        {
            Collider.Disabled = false;

            Collider.SetOrigin(new Vector3(
                (GD.Randf() - .5f) / 1.5f,
                Collider.Transform.origin.y,
                (GD.Randf() - .5f) / 1.5f
            ));

            area.Connect("body_entered", this, nameof(OnCoinTouched));
        }
    }

    private void OnCoinTouched(Node body)
    {
        if (body is Car)
        {
            LevelManager.OnCarTouchedCoin();

            AnimationPlayer.RunAnimationThen("Collected", () =>
            {
                // Freeing cause some dumb problems and we'll free it in a few seconds so...
                _coinMesh?.Hide();
                Collider.Disabled = true;
            });
        }
    }
}