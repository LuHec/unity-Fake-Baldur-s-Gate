using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void GenerateWalls(MapVisualizer mapVisualizer, HashSet<Vector2Int> floorPositions)
    {
        var wallPositions = FindWallsInDirections(floorPositions, Direction2D.CardinalDirectionsList);
        mapVisualizer.GenerateWall(wallPositions);
    }

    public static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var vec2 in floorPositions)
        {
            foreach (var dir in directionList)
            {
                var neighbor = dir + vec2;
                if (!floorPositions.Contains(neighbor))
                    wallPositions.Add(neighbor);
            }
        }

        return wallPositions;
    }
}