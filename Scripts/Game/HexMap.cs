using System;
using System.Collections.Generic;
using System.Linq;
using Civilization.Scripts.Autoload;
using Godot;

namespace Civilization.Scripts.Game;

public partial class HexMap : TileMapLayer
{
    private AStar2D _aStar;
    private Dictionary<Vector2I, int> _positionToIndex;
    [Export] private PackedScene _tilePackedScene;
    private Node _tilesParentNode;
    private List<Vector2I> _usedCells;

    private void EnableNavPointsUpTo(int x)
    {
        foreach (var cell in _usedCells)
        {
            var cellData = GetCellTileData(cell);
            var cost = (int)cellData.GetCustomData("cost");
            var isDisabled = cost > x;
            var pointId = _positionToIndex[cell];
            _aStar.SetPointDisabled(pointId, isDisabled);
        }
    }

    private void AddTile(Vector2I position)
    {
        var tile = _tilePackedScene.Instantiate() as Area2D;
        tile!.Position = MapToLocal(position);
        _tilesParentNode.AddChild(tile);
    }

    private void Debug()
    {
        // EnableNavPointsUpTo(1);

        foreach (var cell in _usedCells)
        {
            var isEnabled = _aStar.IsPointDisabled(_positionToIndex[cell]) ? "Disabled" : "Enabled";
            var label = new Label
            {
                Text = $"{isEnabled} ({_positionToIndex[cell]})", // Use coordinates or tileId, as needed
                Position = MapToLocal(cell) // Offset the position slightly
            };
            AddChild(label);
        }
    }

    private Vector2I[] GetPath(Vector2I from, Vector2I to)
    {
        var fromTileset = LocalToMap(from);
        var toTileset = LocalToMap(to);

        if (!_positionToIndex.ContainsKey(fromTileset) || !_positionToIndex.ContainsKey(toTileset))
            return Array.Empty<Vector2I>();

        var path = _aStar.GetPointPath(_positionToIndex[fromTileset], _positionToIndex[toTileset]);

        return MapToLocal(path);
    }

    private Vector2I[] MapToLocal(Vector2[] path)
    {
        return path
            .Select(v => (Vector2I)v)
            .Select(MapToLocal)
            .Select(v => (Vector2I)v)
            .ToArray();
    }

    private void OnPawnDraggedToOtherTile(Pawn pawn)
    {
        var path = GetPath(pawn.OriginalPosition, pawn.TargetPosition);
        pawn.DrawPath(path);
    }

    private void OnPawnReleased(Pawn pawn)
    {
        var path = GetPath(pawn.OriginalPosition, pawn.TargetPosition);
        pawn.TryMove(path);
    }

    public override void _Ready()
    {
        GameEvents.Instance.PawnDraggedToOtherTile += OnPawnDraggedToOtherTile;
        GameEvents.Instance.PawnReleased += OnPawnReleased;
        
        _tilesParentNode = GetNodeOrNull("Tiles");
        _aStar = new AStar2D();
        _positionToIndex = new Dictionary<Vector2I, int>();
        _usedCells = new List<Vector2I>(GetUsedCells());

        AddTiles(_usedCells);
        BuildAStarMesh(_usedCells);

        Debug();
    }

    private void AddTiles(IEnumerable<Vector2I> positions)
    {
        foreach (var position in positions) AddTile(position);
    }

    private void BuildAStarMesh(IEnumerable<Vector2I> usedCells)
    {
        var index = 1;

        foreach (var cell in _usedCells)
        {
            _aStar.AddPoint(index, cell);
            _positionToIndex.Add(cell, index);

            index++;
        }

        foreach (var cell in _usedCells)
        {
            var cellIndex = _positionToIndex[cell];
            var neighbors = GetSurroundingCells(cell);
            foreach (var neighbor in neighbors)
            {
                if (!_usedCells.Contains(neighbor)) continue;
                var neighborIndex = _positionToIndex[neighbor];
                _aStar.ConnectPoints(cellIndex, neighborIndex);
            }
        }
    }
}