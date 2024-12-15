using System;
using Godot;

namespace Civilization.Scripts.Utils;

public static class Equality
{
    public static bool IsAlmostEqual(double a, double b, double delta = double.Epsilon)
    {
        return Math.Abs(a - b) < delta;
    }
}