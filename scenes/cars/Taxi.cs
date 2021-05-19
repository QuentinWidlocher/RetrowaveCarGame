using Godot;

public class Taxi : KinematicBody
{
    [Signal]
    public delegate void BodyEntered(Node body);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var area = GetNodeOrNull("Area");

        area?.Connect("body_entered", this, nameof(Emit));
    }

    public void Emit(Node body)
    {
        if (body is Car) EmitSignal(nameof(BodyEntered), body);
    }
}