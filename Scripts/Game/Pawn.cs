using System;
using System.Collections.Generic;
using Civilization.Scripts.Autoload;
using Civilization.Scripts.Utils;
using Godot;

namespace Civilization.Scripts.Game;

public partial class Pawn : Node2D
{
    private Area2D _collision;

    private bool _isDragged;
    private bool _isMouseHovering;

    private PathDrawer _pathDrawer;

    private int _maxDistance = 2 + 1;
    public Vector2I OriginalPosition { get; private set; }
    public Vector2I TargetPosition { get; private set; }

    public override void _Ready()
    {
        _collision = GetNode<Area2D>("Collision");
        _pathDrawer = GetNode<PathDrawer>("PathDrawer");
        
        _collision.MouseEntered += () => { _isMouseHovering = true; };
        _collision.MouseExited += () => { _isMouseHovering = false; };

        OriginalPosition = (Vector2I)Position;
        TargetPosition = (Vector2I)Position;
    }

    public override void _Process(double delta)
    {
        if (!_isDragged) return;
        
        FollowMouse();
        UpdateTargetPosition();
    }

    private void UpdateTargetPosition()
    {
        var areas = _collision.GetOverlappingAreas();
        if (areas.Count > 0)
        {
            var closestArea = GetClosestArea(areas);

            if (closestArea == TargetPosition) return;
            
            TargetPosition = closestArea;
            GameEvents.Instance.EmitPawnDraggedToOtherTile(this);
        }
        else
        {
            ResetTargetPosition();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed(Actions.SELECT))
        {
            if (_isMouseHovering) StartDrag();
        }
        else if (@event.IsActionReleased(Actions.SELECT))
        {
            StopDrag();
        }
    }

    public void DrawPath(IEnumerable<Vector2I> points)
    {
        _pathDrawer.DrawPathWithMaxCount(points, _maxDistance);
    }

    public void TryMove(Vector2I[] path)
    {
        if (path.IsEmpty() || path.Length > _maxDistance)
        {
            Position = OriginalPosition;
            return;
        }

        Position = path[^1];
    }

    private void StartDrag()
    {
        OriginalPosition = (Vector2I)Position.Floor();
        UpdateTargetPosition();
        _isDragged = true;
        _pathDrawer.SetVisibility(true);
    }

    private void StopDrag()
    {
        _isDragged = false;
        GameEvents.Instance.EmitPawnReleased(this);
        _pathDrawer.SetVisibility(false);
    }


    private void FollowMouse()
    {
        var mousePosition = GetViewport().GetMousePosition();
        Position = mousePosition;
    }

    private void ResetTargetPosition()
    {
        TargetPosition = OriginalPosition;
    }

    private Vector2I GetClosestArea(IEnumerable<Area2D> areas)
    {
        var closestPoint = (Vector2I)Position.Floor();
        var shortedDistance = 10000000.0;

        foreach (var area in areas)
        {
            var distance = Math.Pow(Position.X - area.Position.X, 2) + Math.Pow(Position.Y - area.Position.Y, 2);
            
            if (distance >= shortedDistance) continue;
            
            shortedDistance = distance;
            closestPoint = (Vector2I)area.Position.Floor();
        }

        return closestPoint;
    }
}