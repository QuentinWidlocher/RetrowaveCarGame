using Godot;
using Helpers;
using static Helpers.Nullable;
using System;
using System.Threading.Tasks;

public class BoostBlock : RoadBlock
{
    [Export]
    float Speed = 400f;

    [Export]
    float Amplitude = 5;

    CSGCylinder? CoinMesh;

    CollisionShape? _Collider;
    CollisionShape Collider => ReturnIfNotNull(_Collider);

    LevelManager? _LevelManager;
    LevelManager LevelManager => ReturnIfNotNull(_LevelManager);

    AnimationPlayer? _AnimationPlayer;
    AnimationPlayer AnimationPlayer => ReturnIfNotNull(_AnimationPlayer);

    public override void _Ready()
    {
        base._Ready();

        _LevelManager = GetNodeOrNull<LevelManager>("/root/LevelManager");
        _AnimationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        CoinMesh = GetNodeOrNull<CSGCylinder>("Area/CollisionShape/CSGCylinder");

        AnimationPlayer.CurrentAnimation = "Floating";
    }

    public override void Enable()
    {
        base.Enable();

        _Collider = GetNodeOrNull<CollisionShape>("Area/CollisionShape");
        var area = GetNodeOrNull<Area>("Area");

        if (Collider != null && area != null)
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

    public override void _Process(float delta)
    {
        base._Process(delta);
    }

    private void OnCoinTouched(Node body)
    {
        LevelManager.OnCarTouchedCoin();

        AnimationPlayer.RunAnimationThen("Collected", () =>
        {
            // Freeing cause some dumb problems and we'll free it in a few seconds so...
            CoinMesh?.Hide();
            Collider.Disabled = true;
        });
    }
}
