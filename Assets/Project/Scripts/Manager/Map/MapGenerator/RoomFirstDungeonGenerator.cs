using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] private int minRoomWidth = 4;
    [SerializeField] private int minRoomHeight = 4;
    [SerializeField] private int dungeonWidth = 20;
    [SerializeField] private int dungeonHeight = 20;
    [SerializeField, Range(0, 10)] private int offset = 1;
    [SerializeField] private bool connectRooms = true;

    [SerializeField, Tooltip("随机生成房间或生成方形")]
    private bool randomWalkRooms = false;

    [Space] [SerializeField, Tooltip("四叉树参数")]
    private int quadTreeWidth = 100, quadTreeHeight = 100;

    [SerializeField] private Vector2 quadTreeOffset;

    // 存储数据结构
    private QuadTree quadTree;
    private Dictionary<uint, Vector2Int> idCenterDict = new Dictionary<uint, Vector2Int>();
    private Dictionary<uint, List<uint>> mapIdMap = new Dictionary<uint, List<uint>>();

    // draw center
    private List<BoundsInt> gizCenters = new List<BoundsInt>();

    private uint currentId = 1;


    public override void RunProceduralGenerator()
    {
        GenerateRooms();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(GetMouse3DPosition(LayerMask.GetMask("Default")), .5f);
    }

    private void Update()
    {
        // if (quadTree != null)
        // {
        //     Debug.Log(quadTree.QueryMap(GetMouse3DPosition(LayerMask.GetMask("Default"))));
        // }

        if (Input.GetMouseButtonDown(0))
        {
            uint targetId = quadTree.QueryMap(GetMouse3DPosition(LayerMask.GetMask("Default")));
            var path = PathFind(currentId, targetId);
            if (path != null)
            {
                currentId = targetId;
                Debug.Log( "Path: "+ string.Join(',', path));
            }
            else
            {
                Debug.Log("Already in target : " + currentId);
            }
        }
    }

    public Vector3 GetMouse3DPosition(int mouseLayerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseLayerMask))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public List<uint> PathFind(uint startMapId, uint targetMapId)
    {
        if (startMapId == targetMapId) return null;
        const int cost = 1;
        Queue<uint> outQueue = new Queue<uint>();

        // 存路径
        Dictionary<uint, uint> closeDict = new Dictionary<uint, uint>();


        outQueue.Enqueue(startMapId);

        while (outQueue.Count > 0)
        {
            uint currentMapId = outQueue.Dequeue();

            foreach (var neighbor in mapIdMap[currentMapId])
            {
                if (neighbor == targetMapId)
                {
                    List<uint> path = new List<uint>();
                    path.Add(neighbor);
                    path.Add(currentMapId);
                    uint backMapId = currentMapId;

                    // 反向找源头得到路径
                    while (backMapId != startMapId)
                    {
                        backMapId = closeDict[backMapId];
                        path.Add(backMapId);
                    }

                    path.Reverse();
                    return path;
                }

                if (!closeDict.ContainsKey(neighbor))
                {
                    outQueue.Enqueue(neighbor);
                    closeDict.Add(neighbor, currentMapId);
                }
            }
        }

        return null;
    }


    private void GenerateRooms()
    {
        // 初始化四叉树,并把四叉树的中心大致移动到地图的中心
        quadTree = new QuadTree(
            new Vector3(startPosition.x - quadTreeWidth / 4, 0, startPosition.y - quadTreeHeight / 4),
            quadTreeWidth, quadTreeHeight);

        // bounds用xy来计算
        var roomList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth,
            minRoomHeight);

        // 获取每个房间的中心，连接房间
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        for (int i = 0; i < roomList.Count; i++)
        {
            uint mapId = (uint)i + 1;
            float subWidth = roomList[i].size.x / 2;
            float subHeight = roomList[i].size.y / 2;

            Vector3 topLeft = new Vector3(roomList[i].center.x - subWidth, 0, roomList[i].center.y + subHeight);
            Vector3 topRight = new Vector3(roomList[i].center.x + subWidth, 0, roomList[i].center.y + subHeight);
            Vector3 botLeft = new Vector3(roomList[i].center.x - subWidth, 0, roomList[i].center.y - subHeight);
            Vector3 botRight = new Vector3(roomList[i].center.x + subWidth, 0, roomList[i].center.y - subHeight);

            Debug.DrawLine(topLeft, botLeft, Color.green, 1000f);
            Debug.DrawLine(topLeft, topRight, Color.green, 1000f);
            Debug.DrawLine(botLeft, botRight, Color.green, 1000f);
            Debug.DrawLine(botRight, topRight, Color.green, 1000f);


            // 加入四叉树
            quadTree.Insert(new QuadMapInfo(
                new Bounds(new Vector3(roomList[i].center.x, 0, roomList[i].center.y),
                    new Vector3(roomList[i].size.x, 0f, roomList[i].size.y)), mapId));

            // 加入绘制列表
            gizCenters.Add(roomList[i]);

            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(roomList[i].center));

            // 加入图记录
            idCenterDict.Add((uint)i + 1, roomCenters[^1]);
            mapIdMap.Add(mapId, new List<uint>());
        }

        HashSet<Vector2Int> floorPositions =
            randomWalkRooms ? GenerateRandomWalkRooms(roomList) : GenerateSimpleRooms(roomList);

        // HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        HashSet<Vector2Int> corridors = ConnectRooms(idCenterDict);

        if (connectRooms) floorPositions.UnionWith(corridors);

        mapVisualizer.Clear();

        mapVisualizer.GeneratePlane(floorPositions, randomWalkParameters);
        WallGenerator.GenerateWalls(mapVisualizer, floorPositions);

        DfsTest();
    }

    private void DfsTest()
    {
        HashSet<uint> path = new HashSet<uint>();
        path.Add(1);
        Dfs(1, path);
    }

    private void Dfs(uint mapId, HashSet<uint> path)
    {
        var neighbors = mapIdMap[mapId];
        foreach (var neighbor in neighbors)
        {
            if (!path.Contains(neighbor))
            {
                path.Add(neighbor);
                Debug.DrawLine(new Vector3(idCenterDict[mapId].x, 0, idCenterDict[mapId].y),
                    new Vector3(idCenterDict[neighbor].x, 0, idCenterDict[neighbor].y), Color.blue, 1000f);
                Dfs(neighbor, path);
            }
        }
    }

    private HashSet<Vector2Int> GenerateRandomWalkRooms(List<BoundsInt> roomList)
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        // 以房间中心为起点生成
        // 会clamp超出bound的部分
        foreach (var roomBound in roomList)
        {
            Vector2Int roomCenter =
                new Vector2Int(Mathf.RoundToInt(roomBound.center.x - offset),
                    Mathf.RoundToInt(roomBound.center.y - offset));
            var room = RunRandomWalk(randomWalkParameters, roomCenter);

            // clamp
            foreach (var position in room)
            {
                if (position.x >= (roomBound.xMin + offset) && position.x <= (roomBound.xMax - offset) &&
                    position.y >= (roomBound.yMin + offset) && position.y <= (roomBound.yMax - offset))
                {
                    floorPositions.Add(position);
                }
            }
        }

        return floorPositions;
    }

    private HashSet<Vector2Int> GenerateSimpleRooms(List<BoundsInt> roomList)
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        foreach (var room in roomList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floorPositions.Add(position);
                }
            }
        }

        return floorPositions;
    }

    /// <summary>
    /// 连接房间，并加入到图里
    /// </summary>
    /// <param name="roomCenters"></param>
    /// <returns></returns>
    private HashSet<Vector2Int> ConnectRooms(Dictionary<uint, Vector2Int> idCentersDict)
    {
        List<KeyValuePair<uint, Vector2Int>> idCentersList = idCentersDict.ToList();

        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = idCentersList[Random.Range(0, idCentersList.Count)];
        idCentersList.Remove(currentRoomCenter);

        while (idCentersList.Count > 0)
        {
            var closest = FindClosestPointTo(currentRoomCenter, idCentersList);
            idCentersList.Remove(closest);

            // 连通图
            mapIdMap[currentRoomCenter.Key].Add(closest.Key);
            mapIdMap[closest.Key].Add(currentRoomCenter.Key);

            HashSet<Vector2Int> newCorridor = GenerateCorridors(currentRoomCenter.Value, closest.Value);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }

        return corridors;
    }


    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = GenerateCorridors(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }

        return corridors;
    }

    private HashSet<Vector2Int> GenerateCorridors(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var position = currentRoomCenter;

        // 根据目标的方向生成走廊
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }

            corridors.Add(position);
        }

        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }

            corridors.Add(position);
        }

        return corridors;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var roomCenter in roomCenters)
        {
            var currentDistance = Vector2Int.Distance(roomCenter, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = roomCenter;
            }
        }

        return closest;
    }

    /// <summary>
    /// 填充图版本
    /// </summary>
    /// <param name="currentRoomCenter"></param>
    /// <param name="roomCenters"></param>
    /// <returns></returns>
    private KeyValuePair<uint, Vector2Int> FindClosestPointTo(KeyValuePair<uint, Vector2Int> currentRoomCenter,
        List<KeyValuePair<uint, Vector2Int>> roomCenters)
    {
        KeyValuePair<uint, Vector2Int> closest = new KeyValuePair<uint, Vector2Int>(0, Vector2Int.zero);
        float distance = float.MaxValue;
        foreach (var roomCenter in roomCenters)
        {
            var currentDistance = Vector2Int.Distance(roomCenter.Value, currentRoomCenter.Value);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = roomCenter;
            }
        }

        return closest;
    }
}