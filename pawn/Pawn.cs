using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class Pawn : Node2D
{
	[Export] Area2D collision;

	bool isDragged;
	Vector2 originalPosition;
	bool isMouseHovering = false;
	public int MaxMove { get; } = 2;
	private Vector2 target;

	public void Move(Vector2? destination)
	{
		if (destination == null)
		{
			Position = originalPosition;
			return;
		}

		Position = (Vector2)destination;
	}


	private Vector2 GetClosestArea(Array<Area2D> areas)
	{
		Vector2 closestPoint = Position;
		var shortesDistance = 10000000.0;

		foreach (var area in areas)
		{
			var distance = Math.Pow(Position.X - area.Position.X, 2) + Math.Pow(Position.Y - area.Position.Y, 2);
			if (distance < shortesDistance)
			{
				shortesDistance = distance;
				closestPoint = area.Position;
			}
		}

		return closestPoint;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		collision.MouseEntered += () => { isMouseHovering = true; };
		collision.MouseExited += () => { isMouseHovering = false; };
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (isDragged)
		{
			var mousePosition = GetViewport().GetMousePosition();
			Position = mousePosition;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(Actions.SELECT))
		{
			if (isMouseHovering)
			{
				originalPosition = Position;
				isDragged = true;
			}
		}
		else if (@event.IsActionReleased(Actions.SELECT))
		{
			isDragged = false;

			var areas = collision.GetOverlappingAreas();
			if (areas.Count > 0)
			{
				var closestArea = GetClosestArea(areas);

				var from = (Vector2I)originalPosition.Floor();
				var to = (Vector2I)closestArea.Floor();
				Events.Instance.EmitMoveFromTo(this, from, to);
			}
			else
			{
				Position = originalPosition;
			}
		}
	}
}
