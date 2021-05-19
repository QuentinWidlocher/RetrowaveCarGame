using Godot;
using static Helpers.Nullable;
using static Helpers.SpatialExtentions;

public class JumpPadBlock : RoadBlock
{

    LevelManager? _LevelManager;
    LevelManager LevelManager => ReturnIfNotNull(_LevelManager);

    public override void _Ready()
    {
        base._Ready();
        _LevelManager = GetNodeOrNull<LevelManager>("/root/LevelManager");
    }

    public override void Enable()
    {
        base.Enable();

        var jumpPad = GetNodeOrNull<CSGBox>("JumpPad");

        if (jumpPad != null)
        {
            jumpPad.UseCollision = true;

            jumpPad.SetOrigin(new Vector3(
               (GD.Randf() - .5f) / 1.5f,
               jumpPad.Transform.origin.y,
               (GD.Randf() - .5f) / 1.5f
           ));

            var boostArea = GetNodeOrNull<Area>("JumpPad/Area");

            if (boostArea != null)
            {
                boostArea.Connect("body_entered", this, nameof(OnBoostAreaTouched));
            }
        }
    }

    public void OnBoostAreaTouched(Node body)
    {
        GD.Print("OnBoostAreaTouched");
        LevelManager.OnCarTouchedJumpPad();
    }
}
