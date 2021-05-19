using Godot;
using Helpers;
using Trail;
using static Godot.GD;
using static Helpers.Nullable;

class Car : KinematicBody
{
    #region Exported
    [Export]
    public readonly float Gravity = 1f;
    [Export]
    public readonly float BaseAcceleration = 0.5f;
    [Export]
    public readonly float BoostingAcceleration = 1f;
    [Export]
    public readonly float Deceleration = 0.4f;
    [Export]
    public readonly float SteerSpeed = 1f;
    [Export]
    public readonly float BaseMaxSpeed = 50f;
    [Export]
    public readonly float BoostingMaxSpeed = 70f;
    [Export]
    public readonly float BrakeAngle = 2;
    [Export]
    public readonly float TotalBoostTime = 1f;
    #endregion

    #region Node Props
    RayCast? _Down;
    RayCast Down => ReturnIfNotNull(_Down);

    Spatial? _CarMesh;
    Spatial CarMesh => ReturnIfNotNull(_CarMesh);

    MeshInstance? _RWheel;
    MeshInstance RWheel => ReturnIfNotNull(_RWheel);

    MeshInstance? _LWheel;
    MeshInstance LWheel => ReturnIfNotNull(_LWheel);

    MeshInstance? _Hull;
    MeshInstance Hull => ReturnIfNotNull(_Hull);

    LevelManager? _LevelManager;
    LevelManager LevelManager => ReturnIfNotNull(_LevelManager);

    ImmediateGeometry? _Trail;
    ImmediateGeometry Trail => ReturnIfNotNull(_Trail);
    #endregion

    public Vector3 Velocity = new Vector3();
    public Vector2 Direction = Vector2.Right;
    public Vector3 DirectionVector => Vector3.Forward.Rotated(
        Vector3.Up,
        Direction.Angle() / (BrakeAngle / (Mathf.Abs(Velocity.z) / MaxSpeed)) / (IsOnFloor() ? 1 : 5)
        ).Normalized();

    public float BoostTime = 0f;
    public bool isBoosting => BoostTime > 0 && BoostTime <= TotalBoostTime;
    public float Acceleration => isBoosting ? BoostingAcceleration : BaseAcceleration;
    public float MaxSpeed => isBoosting ? BoostingMaxSpeed : BaseMaxSpeed;

    public override void _Ready()
    {
        _Down = GetNodeOrNull<RayCast>("DownRayCast");
        _CarMesh = GetNodeOrNull<Spatial>("Mesh");
        _Hull = GetNodeOrNull<MeshInstance>("Mesh/Hull");
        _RWheel = GetNodeOrNull<MeshInstance>("Mesh/RWheel");
        _LWheel = GetNodeOrNull<MeshInstance>("Mesh/LWheel");
        _Trail = GetNodeOrNull<ImmediateGeometry>("Mesh/Trail");

        _LevelManager = GetNodeOrNull<LevelManager>("/root/LevelManager");

        LevelManager.Connect(nameof(LevelManager.CarTouchedCoin), this, nameof(OnCoinTouched));
        LevelManager.Connect(nameof(LevelManager.CarTouchedTaxi), this, nameof(OnTaxiTouched));
        LevelManager.Connect(nameof(LevelManager.CarTouchedJumpPad), this, nameof(OnJumpPadTouched));
    }

    public override void _PhysicsProcess(float delta)
    {
        ComputeBoosting(delta);

        ComputeDirection();

        ComputeVelocity();

        ComputeGravity();

        Velocity = MoveAndSlide(Velocity, Transform.basis.y);

        if (Velocity.Length() > 15)
        {
            CarMesh.LookAt(Transform.origin + Velocity, Transform.basis.y);
        }

        AlignCarWithGround();
    }

    private void AlignCarWithGround()
    {
        if (Down.IsColliding())
        {
            var n = Down.GetCollisionNormal().Normalized();
            GlobalTransform = GlobalTransform.InterpolateWith(GlobalTransform.LookingAtWithY(n), .1f);
        }
    }

    private void ComputeDirection()
    {

        // Rotate the direction a bit toward the pushed direction
        if (Input.IsActionPressed("steer_left"))
        {
            Direction = Direction.LinearInterpolate(new Vector2(1, 1), SteerSpeed);
        }
        else if (Input.IsActionPressed("steer_right"))
        {
            Direction = Direction.LinearInterpolate(new Vector2(1, -1), SteerSpeed);
        }
        else
        {
            Direction = Direction.LinearInterpolate(new Vector2(1, 0), SteerSpeed);
        }

        RWheel.Rotation = new Vector3(
            RWheel.Rotation.x,
            Direction.Angle() / 2,
            RWheel.Rotation.z
        );

        LWheel.Rotation = RWheel.Rotation;

        Hull.Rotation = new Vector3(
            Hull.Rotation.x,
            Hull.Rotation.y,
            Direction.Angle() / 10
        );
    }

    private void ComputeVelocity()
    {

        if (Input.IsActionPressed("accelerate"))
        {
            if (Mathf.Abs(Velocity.z) <= MaxSpeed)
            {
                Velocity += DirectionVector * Acceleration;
            }
            else
            {
                // Cap the velocity at MaxSpeed if > Maxspeed but keep the direction
                Velocity = (Velocity + DirectionVector * Acceleration).Normalized() * MaxSpeed;
            }
        }

        if (!Input.IsActionPressed("accelerate") && Velocity.z <= -Deceleration)
        {
            Velocity += Transform.basis.z * Deceleration;

            if (Velocity.x < -Deceleration)
            {
                Velocity += Transform.basis.x * Deceleration;
            }
            else if (Velocity.x > Deceleration)
            {
                Velocity -= Transform.basis.x * Deceleration;
            }
            else
            {
                Velocity.x = 0;
            }

            Direction = Direction.LinearInterpolate(new Vector2(1, 0), SteerSpeed);
        }
    }

    private void ComputeBoosting(float delta)
    {
        if (isBoosting)
        {
            BoostTime -= delta;
        }

        Trail.Set("distance", isBoosting ? .5f : 0);
        Trail.Set("emit", isBoosting);
    }

    private void ComputeGravity()
    {
        if (!IsOnFloor())
        {
            Velocity += Vector3.Down * Gravity;
        }
    }

    private void OnCoinTouched()
    {
        BoostTime = TotalBoostTime;
    }

    private void OnTaxiTouched()
    {
        Velocity *= -0.3f;
    }

    private void OnJumpPadTouched()
    {
        GD.Print("OnJumpPadTouched");
        BoostTime = .2f;
        Velocity *= 5f;
    }
}