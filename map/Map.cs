using Godot;
using Godot.Collections;
using System;

public partial class Map : TileMapLayer
{
	AStar2D astar;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		astar = new();
		var usedCells = GetUsedCells();
        int index = 1;
        Dictionary<Vector2I, int> positionToIndex = new();

        foreach (var cell in usedCells)
		{
            astar.AddPoint(index, cell);
			positionToIndex.Add(cell, index);
			index++;
		}

        foreach (var cell in usedCells)
        {
            var cellIndex = positionToIndex[cell];
            var neighbors = GetSurroundingCells(cell);
            foreach (var neighbor in neighbors)
            {
                if (!usedCells.Contains(neighbor))
                {
                    continue;
                }
                var neighborIndex = positionToIndex[neighbor];
                astar.ConnectPoints(cellIndex, neighborIndex, true);
            }
        }

		foreach (var cell in usedCells)
		{
            var label = new Label
            {
                Text = $"({positionToIndex[cell]})",  // Use coordinates or tileId, as needed
                Position = MapToLocal(cell),  // Offset the position slightly
            };
            AddChild(label);
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
