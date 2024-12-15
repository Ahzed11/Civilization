using System;
using Civilization.Scripts.Utils;
using Godot;


namespace Civilization.Scripts.Game;

public partial class Camera : Camera2D
{
    private bool _isDragging;
    private float _zoomTarget = 1.0f;
    private const float ZoomIncrement = 0.1f;
    private const float ZoomMax = 1f;
    private const float ZoomMin = 0.5f;
    private const float ZoomRate = 8.0f;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_isDragging)
        {
            if (@event is InputEventMouseMotion eventMouseMotion)
            {
                Position -= eventMouseMotion.Relative / Zoom;
            }
        }

        if (@event.IsActionPressed(Actions.PAN))
        {
            _isDragging = true;
        }
        else if (@event.IsActionReleased(Actions.PAN))
        {
            _isDragging = false;
        }
        
        if (@event.IsActionPressed(Actions.ZOOM_IN))
        {
            ZoomIn();
        }
        else if (@event.IsActionPressed(Actions.ZOOM_OUT))
        {
            ZoomOut();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Zoom = Zoom.Lerp(_zoomTarget * Vector2.One, ZoomRate * (float) delta);
        
        SetPhysicsProcess(!Equality.IsAlmostEqual(Zoom.X, _zoomTarget, 0.005f));
    }

    private void ZoomIn()
    {
        _zoomTarget = Math.Min(_zoomTarget + ZoomIncrement, ZoomMax);
        SetPhysicsProcess(true);
    }

    private void ZoomOut()
    {
        _zoomTarget = Math.Max(_zoomTarget - ZoomIncrement, ZoomMin);
        SetPhysicsProcess(true);
    }
}