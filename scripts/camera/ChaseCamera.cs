using Godot;
using Helpers;

public class ChaseCamera : Camera
{
    [Export] protected float LerpSpeed = 10;
    [Export] private bool LockYPos;
    [Export] private bool LockYRot;
    private Vector3 Offset;
    private Vector3 OriginalPos;
    private Basis OriginalRot = Basis.Identity;

    private Spatial? Target;
    [Export] private NodePath? TargetPath;

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
                Offset = Target.GlobalTransform.origin - GlobalTransform.origin;
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