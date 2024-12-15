using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Civilization.Scripts.Game;

public partial class PathDrawer : Node
{
    [Export] private Color _invalidColor;
    private Line2D _lineDrawer;
    [Export] private Color _validColor;

    public override void _Ready()
    {
        _lineDrawer = GetNode<Line2D>("LineDrawer");
    }

    public void SetVisibility(bool isVisible)
    {
        _lineDrawer.Points = Array.Empty<Vector2>();
        _lineDrawer.Visible = isVisible;
    }

    public void DrawPathWithMaxCount(IEnumerable<Vector2I> points, int maxCount)
    {
        _lineDrawer.DefaultColor = points.Count() > maxCount ? _invalidColor : _validColor;
        DrawPath(points);
    }

    public void DrawPath(IEnumerable<Vector2I> points)
    {
        var pointsArray = points.Select(x => (Vector2)x).ToArray();
        _lineDrawer.Points = pointsArray;
    }
}