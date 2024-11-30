using System.Linq;
using Godot;
using Godot.Collections;

public partial class Map : TileMapLayer
{
    [Export] Node2D tilesParentNode;
    [Export] PackedScene tilePackedScene;
    [Export] Line2D pathDrawer;

    AStar2D astar;
    Dictionary<Vector2I, int> positionToIndex;
    Array<Vector2I> usedCells;

    private void Move(Pawn sender, Vector2I from, Vector2I to, int maxDistance)
    {
        var mapFrom = LocalToMap(from);
        var mapTo = LocalToMap(to);

        var fromId = positionToIndex[mapFrom];
        var toId = positionToIndex[mapTo];

        var path = astar.GetPointPath(fromId, toId);

        if (path.Length == 0 || path.Length > maxDistance + 1)
        {
            sender.Move(null);
        }
        else
        {
            var destination = (Vector2I)path[^1];
            sender.Move(MapToLocal(destination));
        }
    }

    private void AskMove(Vector2I from, Vector2I to, int maxDistance)
    {
        var mapFrom = LocalToMap(from);
        var mapTo = LocalToMap(to);

        var fromId = positionToIndex[mapFrom];
        var toId = positionToIndex[mapTo];

        var path = astar.GetPointPath(fromId, toId);

        pathDrawer.Points = path.Select((x) => MapToLocal((Vector2I)x)).ToArray();
        if (path.Length == 0 || path.Length > maxDistance + 1)
        {
            pathDrawer.DefaultColor = Color.Color8(255, 0, 0, 255);
        }
        else
        {
            pathDrawer.DefaultColor = Color.Color8(0, 255, 0, 255);
        }
    }

    private void OnPawnReleased(Pawn pawn)
    {
        pathDrawer.Points = new Vector2[0];
    }

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

    private void AddTile(Vector2I position)
    {
        var tile = tilePackedScene.Instantiate() as Area2D;
        tile.Position = MapToLocal(position);
        tilesParentNode.AddChild(tile);
    }

    private void Debug()
    {
        // EnableNavPointsUpTo(1);

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
        Events.Instance.MoveFromTo += Move;
        Events.Instance.AskMoveFromTo += AskMove;
        Events.Instance.PawnReleased += OnPawnReleased;

        astar = new();
        positionToIndex = new();
        usedCells = GetUsedCells();

        int index = 1;

        foreach (var cell in usedCells)
        {
            astar.AddPoint(index, cell);
            positionToIndex.Add(cell, index);

            AddTile(cell);

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

        Debug();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
