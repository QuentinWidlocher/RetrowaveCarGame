using Godot;
using Helpers;
using static Helpers.SpatialExtentions;

public class ChaseCamera : Camera
{
    [Export]
    float LerpSpeed = 10;
    [Export]
    NodePath? TargetPath;
    [Export]
    bool LockYPos = false;
    [Export]
    bool LockYRot = false;

    Spatial? Target;
    Vector3 Offset = new Vector3();
    Vector3 OriginalPos = new Vector3();
    Basis OriginalRot = Basis.Identity;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        OriginalPos = GlobalTransform.origin;
        OriginalRot = GlobalTransform.basis;

        if (TargetPath != null)
        {
            Target = GetNodeOrNull<Spatial>(TargetPath);

            if (Target != null)
            {
                Offset = GlobalTransform.origin - Target.GlobalTransform.origin;
                SetAsToplevel(true);
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        if (Target != null)
        {
            var targetPos = Target.GlobalTransform.Translated(Offset);
            GlobalTransform = GlobalTransform.InterpolateWith(targetPos, LerpSpeed * delta);
            LookAt(Target.GlobalTransform.origin, Vector3.Up);

            if (LockYPos)
                this.SetGlobalOriginY(OriginalPos.y);

            if (LockYRot)
                this.SetGlobalBasisY(OriginalRot.y);

            GlobalTransform = GlobalTransform.Orthonormalized();
        }
    }
}
