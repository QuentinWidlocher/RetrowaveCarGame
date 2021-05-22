using Godot;
using static Godot.GD;
using static Helpers.Nullable;
using static Helpers.SpatialExtentions;

public class JumpPadBlock : RoadBlock
{
    private LevelManager? _levelManager;
    private LevelManager LevelManager => ReturnIfNotNull(_levelManager);

    public override void _Ready()
    {
        base._Ready();
        _levelManager = GetNodeOrNull<LevelManager>("/root/LevelManager");
    }

    public override void Enable()
    {
        base.Enable();

        var jumpPad = GetNodeOrNull<CSGBox>("JumpPad");

        if (jumpPad != null)
        {
            jumpPad.UseCollision = true;

            jumpPad.SetOrigin(new Vector3(
                (Randf() - .5f) / 1.5f,
                jumpPad.Transform.origin.y,
                (Randf() - .5f) / 1.5f
            ));

            var boostArea = GetNodeOrNull<Area>("JumpPad/Area");

            boostArea?.Connect("body_entered", this, nameof(OnBoostAreaTouched));
        }
    }

    public void OnBoostAreaTouched(Node body)
    {
        if (body is Car) LevelManager.OnCarTouchedJumpPad();
    }
}