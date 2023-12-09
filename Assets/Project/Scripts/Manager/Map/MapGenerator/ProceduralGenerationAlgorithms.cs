using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPos, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPos);

        var prevPos = startPos;

        for (int i = 0; i < walkLength; i++)
        {
            var newPos = prevPos + Direction2D.GetRandomCardinalDirection();
            path.Add(newPos);
            prevPos = newPos;
        }

        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPos, int corridorLength)
    {
        List<Vector2Int> corridors = new List<Vector2Int>();

        // 随机选择一个方向生成走廊
        var direction = Direction2D.GetRandomCardinalDirection();
        var currentPos = startPos;
        corridors.Add(currentPos);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPos = currentPos + direction;
            corridors.Add(currentPos);
        }

        return corridors;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);

        while (roomsQueue.Count > 0)
        {
            // 分割空间，直到不能分割为止，加入到list中
            var room = roomsQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                // 随机竖直或水平分割，如果不能分割转成另一种方式
                if (Random.value < 0.5f)
                {
                    // 竖直分割
                    if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    // 水平分割
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    // 都不能分割，但是能包含一个房间，把原始bound加入list
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomList.Add(room);
                    }
                    // 再不行就直接舍弃
                }
                else
                {
                    // 水平分割
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    // 竖直分割
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    // 都不能分割，但是能包含一个房间，把原始bound加入list
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomList.Add(room);
                    }
                    // 再不行就直接舍弃
                }
            }
            else
            {
                roomList.Add(room);
            }
        }

        return roomList;
    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        // 以最小宽度进行分割
        // (minWidth, room.size.x - minWidth)
        int xSplit = minWidth;

        // 随机分割
        // int xSplit = Random.Range(4, minWidth);

        // 均匀分割
        // int xSplit = 

        // 构造两个被分割后的空间
        // room.min is the start of the bounds(maybe left?)
        BoundsInt roomLeft = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(roomLeft);

        // 如果剩下的宽度只有1就不分割了
        // if (room.size.x - xSplit > minWidth)
        // {
            BoundsInt roomRight = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
                new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
            roomsQueue.Enqueue(roomRight);
        // }
    }


    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        int ySplit = minHeight;
        // int ySplit = Random.Range(4, minHeight);
        // Debug.Log(ySplit);
        
        // 构造两个被分割后的空间
        BoundsInt roomDown = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        roomsQueue.Enqueue(roomDown);

        // if (room.size.y - ySplit > minHeight)
        // {
            BoundsInt roomUp = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
                new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
            roomsQueue.Enqueue(roomUp);
        // }
    }
}

public static class Direction2D
{
    public static readonly List<Vector2Int> CardinalDirectionsList = new List<Vector2Int>
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return CardinalDirectionsList[Random.Range(0, CardinalDirectionsList.Count)];
    }
}