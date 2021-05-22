using System;
using Godot;
using Helpers;
using static Helpers.Nullable;

internal class Car : KinematicBody
{
    public float BoostTime;
    public Vector2 Direction = Vector2.Right;

    public Vector3 Velocity;

    public Vector3 DirectionVector => Vector3.Forward.Rotated(
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

        if (Velocity.Length() > 15) CarMesh.LookAt(Transform.origin + Velocity, Transform.basis.y);

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
            Direction = Direction.LinearInterpolate(new Vector2(1, 1), SteerSpeed);
        else if (Input.IsActionPressed("steer_right"))
            Direction = Direction.LinearInterpolate(new Vector2(1, -1), SteerSpeed);
        else
            Direction = Direction.LinearInterpolate(new Vector2(1, 0), SteerSpeed);

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
                Velocity += DirectionVector * Acceleration;
            else
                // Cap the velocity at MaxSpeed if > MaxSpeed but keep the direction
                Velocity = (Velocity + DirectionVector * Acceleration).Normalized() * MaxSpeed;
        }

        if (!Input.IsActionPressed("accelerate") && Velocity.z <= -Deceleration)
        {
            Velocity += Transform.basis.z * Deceleration;

            if (Velocity.x < -Deceleration)
                Velocity += Transform.basis.x * Deceleration;
            else if (Velocity.x > Deceleration)
                Velocity -= Transform.basis.x * Deceleration;
            else
                Velocity.x = 0;

            Direction = Direction.LinearInterpolate(new Vector2(1, 0), SteerSpeed);
        }
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
        Velocity *= -0.3f;
    }

    private void OnJumpPadTouched()
    {
        BoostTime = .2f;
        Velocity *= 5f;
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