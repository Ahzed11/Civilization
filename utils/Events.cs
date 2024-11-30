using Godot;

public partial class Events : Node
{
    public static Events Instance { get; private set; }

    [Signal] public delegate void MoveFromToEventHandler(Pawn sender, Vector2I from, Vector2I to, int maxDistance);
    [Signal] public delegate void AskMoveFromToEventHandler(Vector2I from, Vector2I to, int maxDistance);
    [Signal] public delegate void PawnReleasedEventHandler(Pawn pawn);

    public void EmitMoveFromTo(Pawn sender, Vector2I from, Vector2I to, int maxDistance)
    {
        EmitSignal(SignalName.MoveFromTo, sender, from, to, maxDistance);
    }

    public void EmitAskMoveFromTo(Vector2I from, Vector2I to, int maxDistance)
    {
        EmitSignal(SignalName.AskMoveFromTo, from, to, maxDistance);
    }

    public void EmitPawnReleased(Pawn pawn)
    {
        EmitSignal(SignalName.PawnReleased, pawn);
    }

    public override void _Ready()
    {
        Instance = this;
    }
}