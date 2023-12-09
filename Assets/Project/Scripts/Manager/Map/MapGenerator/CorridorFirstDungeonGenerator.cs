using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] private int corridorLength = 10;
    [SerializeField] private int corridorCount = 3;
    [SerializeField, Range(0.1f, 1)] private float roomPercent = 0.8f;
    [SerializeField] private SimpleRandomWalkSo roomGenerationParameters;

    public override void RunProceduralGenerator()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPosition = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        // 创建走廊
        GenerateCorridor(floorPosition, potentialRoomPositions);
        // 创建房间
        HashSet<Vector2Int> roomPositions = GenerateRooms(potentialRoomPositions);
        // 寻找死点,并在死点创建房间
        List<Vector2Int> deadEnds = FindAllDeadEnd(floorPosition);
        GenerateDeadEndRooms(deadEnds, roomPositions);
        // 合并走廊和房间
        floorPosition.UnionWith(roomPositions);

        mapVisualizer.Clear();
        mapVisualizer.GeneratePlane(floorPosition, roomGenerationParameters);
        WallGenerator.GenerateWalls(mapVisualizer, floorPosition);
    }

    public HashSet<Vector2Int> GenerateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        // 随机生成标识符后排序
        List<Vector2Int> roomsToGenerate =
            potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var roomPosition in roomsToGenerate)
        {
            // 生成房间
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private void GenerateCorridor(HashSet<Vector2Int> floorPosition, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[^1];
            // 把走廊最后一个点作为房间的起点
            potentialRoomPositions.Add(currentPosition);

            floorPosition.UnionWith(corridor);
        }
    }

    private List<Vector2Int> FindAllDeadEnd(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        foreach (var position in floorPositions)
        {
            int neighbours = 0;
            foreach (var dir in Direction2D.CardinalDirectionsList)
            {
                var neighbourPos = position + dir;
                if (floorPositions.Contains(neighbourPos)) neighbours++;
            }

            if (neighbours == 1)
            {
                deadEnds.Add(position);
            }
        }

        return deadEnds;
    }

    private void GenerateDeadEndRooms(List<Vector2Int> deadEnds, HashSet<Vector2Int> floorPositions)
    {
        foreach (var deadEnd in deadEnds)
        {
            if (!floorPositions.Contains(deadEnd))
            {
                var deadEndRoom = RunRandomWalk(randomWalkParameters, deadEnd);
                floorPositions.UnionWith(deadEndRoom);   
            }
        }
    }
}