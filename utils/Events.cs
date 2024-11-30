using Godot;

public partial class Events : Node
{
    public static Events Instance { get; private set; }

    [Signal] public delegate void MoveFromToEventHandler(Pawn sender, Vector2I from, Vector2I to);

    public void EmitMoveFromTo(Pawn sender, Vector2I from, Vector2I to)
    {
        EmitSignal(SignalName.MoveFromTo, sender, from, to);
    }

    public override void _Ready()
    {
        Instance = this;
    }
}