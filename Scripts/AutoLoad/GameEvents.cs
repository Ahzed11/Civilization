using Civilization.Scripts.Game;
using Godot;

namespace Civilization.Scripts.Autoload;

public partial class GameEvents : Node
{
    [Signal]
    public delegate void PawnDraggedToOtherTileEventHandler(Pawn sender);

    [Signal]
    public delegate void PawnReleasedEventHandler(Pawn sender);

    public static GameEvents Instance { get; private set; }

    public void EmitPawnDraggedToOtherTile(Pawn sender)
    {
        EmitSignal(SignalName.PawnDraggedToOtherTile, sender);
    }

    public void EmitPawnReleased(Pawn sender)
    {
        EmitSignal(SignalName.PawnReleased, sender);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated) Instance = this;
    }
}