using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField] protected SimpleRandomWalkSo randomWalkParameters;

    void Start()
    {
    }

    public override void RunProceduralGenerator()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
        mapVisualizer.Clear(); 
        mapVisualizer.GeneratePlane(floorPositions, randomWalkParameters);
        WallGenerator.GenerateWalls(mapVisualizer, floorPositions);
    }

    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSo parameters, Vector2Int position)
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition,
                parameters.walkLength);

            // 合并set
            floorPositions.UnionWith(path);

            // 从已有集合中继续生成
            if (parameters.startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }

        return floorPositions;
    }
}