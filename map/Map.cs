using Godot;
using Godot.Collections;
using System;

public partial class Map : TileMapLayer
{
    AStar2D astar;
    Dictionary<Vector2I, int> positionToIndex;
    Array<Vector2I> usedCells;

    private void EnableNavPointsUpTo(int x)
    {
        foreach (var cell in usedCells)
        {
            var cellData = GetCellTileData(cell);
            int cost = (int)cellData.GetCustomData("cost");
            var isDisabled = false;
            if (cost > x)
            {
                isDisabled = true;
            }
            var pointId = positionToIndex[cell];
            astar.SetPointDisabled(pointId, isDisabled);
        }
    }

    private void Debug()
    {
        EnableNavPointsUpTo(1);

        foreach (var cell in usedCells)
        {
            string isEnabled = astar.IsPointDisabled(positionToIndex[cell]) ? "Disabled" : "Enabled";
            var label = new Label
            {
                Text = $"{isEnabled} ({positionToIndex[cell]})",  // Use coordinates or tileId, as needed
                Position = MapToLocal(cell),  // Offset the position slightly
            };
            AddChild(label);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        astar = new();
        positionToIndex = new();
        usedCells = GetUsedCells();

        int index = 1;

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
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
