using System;
using Godot;
using Helpers;
using static Helpers.Nullable;

internal class Car : KinematicBody
{
    public float BoostTime;
    public Vector2 Direction = Vector2.Right;

    public Vector3 Velocity;

    public Vector3 DirectionVector => Vector3.Back.Rotated(
        Vector3.Up,
        Direction.Angle() / (BrakeAngle / (Mathf.Abs(Velocity.z) / MaxSpeed)) / (IsOnFloor() ? 1 : 5)
    ).Normalized();

    public bool IsBoosting => BoostTime > 0 && BoostTime <= TotalBoostTime;
    public float Acceleration => IsBoosting ? BoostingAcceleration : BaseAcceleration;

    public float MaxSpeed
    {
        get
        {
            var maxSpeed = IsBoosting ? BoostingMaxSpeed : BaseMaxSpeed;
            var maxSpeedAccelerated = maxSpeed * (OS.GetTicksMsec() / GameAcceleration) + maxSpeed;
            return Math.Clamp(maxSpeedAccelerated, BaseMaxSpeed, MaxMaxSpeed);
        }
    }

    public override void _Ready()
    {
        _down = GetNodeOrNull<RayCast>("DownRayCast");
        _carMesh = GetNodeOrNull<Spatial>("Mesh");
        _hull = GetNodeOrNull<MeshInstance>("Mesh/Hull");
        _rWheel = GetNodeOrNull<MeshInstance>("Mesh/RWheel");
        _lWheel = GetNodeOrNull<MeshInstance>("Mesh/LWheel");
        _trail = GetNodeOrNull<ImmediateGeometry>("Mesh/Trail");

        _levelManager = GetNodeOrNull<LevelManager>("/root/LevelManager");

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

        var direction = new Vector3(
            Velocity.x,
            Velocity.y,
            Mathf.Abs(Velocity.z)
        );
        CarMesh.LookAt(GlobalTransform.origin + (direction - Vector3.Forward), Transform.basis.y);

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

        var d = 0;
        if (Input.IsActionPressed("steer_left"))
            d = 1;
        else if (Input.IsActionPressed("steer_right"))
            d = -1;

        Direction = Direction.LinearInterpolate(new Vector2(1, d), SteerSpeed);

        // Rotate the wheels toward direction
        RWheel.Rotation = new Vector3(
            RWheel.Rotation.x,
            Direction.Angle() / 2,
            RWheel.Rotation.z
        );

        // Rotate the second wheel
        LWheel.Rotation = RWheel.Rotation;

        // Shift the car toward the direction for emphasis
        Hull.Rotation = new Vector3(
            Hull.Rotation.x,
            Hull.Rotation.y,
            Direction.Angle() / 10
        );
    }

    private void ComputeVelocity()
    {
        if (Velocity.z <= MaxSpeed && Velocity.z >= 0)
            Velocity += DirectionVector * Acceleration;
        else if (Velocity.z < 0)
            // Go back to zero when re-accelerating instead of going sideways
            Velocity = Velocity.LinearInterpolate(Vector3.Zero, Acceleration);
        else
            // Cap the velocity at MaxSpeed if > MaxSpeed but keep the direction
            Velocity = (Velocity + DirectionVector * Acceleration).Normalized() * MaxSpeed;
    }

    private void ComputeBoosting(float delta)
    {
        if (IsBoosting) BoostTime -= delta;

        Trail.Set("distance", IsBoosting ? .5f : 0);
        Trail.Set("emit", IsBoosting);
    }

    private void ComputeGravity()
    {
        if (!IsOnFloor()) Velocity += Vector3.Down * Gravity;
    }

    private void OnCoinTouched()
    {
        BoostTime = TotalBoostTime;
    }

    private void OnTaxiTouched()
    {
        Velocity.z = -1f;
        Velocity.x = 0f;
    }

    private void OnJumpPadTouched()
    {
        BoostTime = Mathf.Max(.2f, BoostTime);
    }

    #region Exported

    [Export] public readonly float Gravity = 1f;
    [Export] public readonly float BaseAcceleration = 0.5f;
    [Export] public readonly float BoostingAcceleration = 1f;
    [Export] public readonly float Deceleration = 0.4f;
    [Export] public readonly float SteerSpeed = 1f;
    [Export] public readonly float BaseMaxSpeed = 50f;
    [Export] public readonly float BoostingMaxSpeed = 70f;
    [Export] public readonly float BrakeAngle = 2;
    [Export] public readonly float TotalBoostTime = 1f;
    [Export] public readonly float GameAcceleration = 100000f;
    [Export] public readonly float MaxMaxSpeed = 130f;

    #endregion

    #region Node Props

    private RayCast? _down;
    private RayCast Down => ReturnIfNotNull(_down);

    private Spatial? _carMesh;
    private Spatial CarMesh => ReturnIfNotNull(_carMesh);

    private MeshInstance? _rWheel;
    private MeshInstance RWheel => ReturnIfNotNull(_rWheel);

    private MeshInstance? _lWheel;
    private MeshInstance LWheel => ReturnIfNotNull(_lWheel);

    private MeshInstance? _hull;
    private MeshInstance Hull => ReturnIfNotNull(_hull);

    private LevelManager? _levelManager;
    private LevelManager LevelManager => ReturnIfNotNull(_levelManager);

    private ImmediateGeometry? _trail;
    private ImmediateGeometry Trail => ReturnIfNotNull(_trail);

    #endregion
}