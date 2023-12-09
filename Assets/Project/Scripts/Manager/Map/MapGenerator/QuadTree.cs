using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuadTree
{
    public int Width => width;
    public int Height => height;
    public float Hwidth => width / 2;
    public float Hheight => height / 2;

    private int width;
    private int height;

    private QuadNode root;

    public QuadTree(Vector3 startPosition, int width, int height)
    {
        this.width = width;
        this.height = height;

        // 放置在大地图中心，根节点涵盖整个地图
        root = new QuadNode(new Bounds(startPosition + new Vector3(Hwidth, 0, Hheight),
            new Vector3(this.width, 0, this.height)));
    }

    /// <summary>
    /// map id从1开始，为0表示不存在
    /// </summary>
    /// <param name="position">要查询的位置</param>
    /// <returns></returns>
    public uint QueryMap(Vector3 position)
    {
        position.y = 0;
        return root.QueryMap(position);
    }

    public void Insert(QuadMapInfo mapInfo)
    {
        root.Insert(mapInfo);
    }
}

public class QuadNode
{
    private const int MAX_SIZE = 4;
    private bool isLeaf = true;
    private Bounds bounds;

    // 四个分支
    private List<QuadNode> sonQuads;
    private List<QuadMapInfo> sharedMapInfosList;
    private List<QuadMapInfo> mapInfosList;

    public QuadNode(Bounds bounds)
    {
        this.bounds = bounds;

        sonQuads = new List<QuadNode>();
        sharedMapInfosList = new List<QuadMapInfo>();
        mapInfosList = new List<QuadMapInfo>();
    }

    /// <summary>
    /// dfs查询
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public uint QueryMap(Vector3 position)
    {
        // 先和共享列表进行判定
        foreach (var mapInfo in sharedMapInfosList)
        {
            if (mapInfo.IsInQuad(position)) return mapInfo.MapId;
        }

        // 非叶子节点，找子节点
        if (!isLeaf)
        {
            foreach (var quad in sonQuads)
            {
                uint res = quad.QueryMap(position);
                if (res != 0) return res;
            }
        }

        // 叶子节点，找存储的信息
        if (isLeaf)
        {
            foreach (var mapInfo in mapInfosList)
            {
                if (mapInfo.IsInQuad(position)) return mapInfo.MapId;
            }
        }

        return 0;
    }

    private void SubDivide()
    {
        isLeaf = false;

        float subWidth = bounds.size.x / 2, subHeight = bounds.size.z / 2;
        Vector2[] step = new[]
        {
            new Vector2(-subWidth, subHeight),
            new Vector2(subWidth, subHeight),
            new Vector2(-subWidth, -subHeight),
            new Vector2(subWidth, -subHeight),
        };

        // Debug 画线
        Debug.Log("DrawLine");

        // 划分当前象限
        // ---------
        // | 1 | 2 |
        // ----.----
        // | 3 | 4 |
        // ---------

        Vector3 topLeft = new Vector3(bounds.center.x - subWidth, 0, bounds.center.z + subHeight);
        Vector3 topRight = new Vector3(bounds.center.x + subWidth, 0, bounds.center.z + subHeight);
        Vector3 botLeft = new Vector3(bounds.center.x - subWidth, 0, bounds.center.z - subHeight);
        Vector3 botRight = new Vector3(bounds.center.x + subWidth, 0, bounds.center.z - subHeight);
        Vector3 topMid = new Vector3(bounds.center.x, 0, bounds.center.z + subHeight);
        Vector3 leftMid = new Vector3(bounds.center.x - subWidth, 0, bounds.center.z);
        Vector3 rightMid = new Vector3(bounds.center.x + subWidth, 0, bounds.center.z);
        Vector3 botMid = new Vector3(bounds.center.x, 0, bounds.center.z - subHeight);

        Debug.DrawLine(topLeft, botLeft, Color.red, 1000f);
        Debug.DrawLine(topLeft, topRight, Color.red, 1000f);
        Debug.DrawLine(botLeft, botRight, Color.red, 1000f);
        Debug.DrawLine(botRight, topRight, Color.red, 1000f);
        Debug.DrawLine(topMid, botMid, Color.red, 1000f);
        Debug.DrawLine(leftMid, rightMid, Color.red, 1000f);


        for (int i = 0; i < 4; i++)
        {
            Vector3 center = new Vector3(bounds.center.x + step[i][0] / 2, bounds.center.y,
                bounds.center.z + step[i][1] / 2);
            sonQuads.Add(new QuadNode(new Bounds(center, new Vector3(subWidth, 0, subHeight))));
        }

        // 转移列表的物体
        int index = -1;
        int cnt = 0;
        for (int i = 0; i < mapInfosList.Count; i++)
        {
            for (int j = 0; j < sonQuads.Count; j++)
            {
                if (sonQuads[j].InQuad(mapInfosList[i].GetBound()))
                {
                    cnt++;
                    index = j;
                }
            }

            if (cnt == 1)
            {
                sonQuads[index].Insert(mapInfosList[i]);
            }
            else
            {
                sharedMapInfosList.Add(mapInfosList[i]);
            }
        }
    }

    /// <summary>
    /// 查询目标范围是否在当前四叉树子节点中
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool InQuad(Bounds other)
    {
        // if (((box.rMid.x - other.box.rMid.x) * (box.rMid.x - other.box.lMid.x) <= 0 ||
        //      (box.lMid.x - other.box.rMid.x) * (box.lMid.x - other.box.lMid.x) <= 0)
        //     &&
        //     ((box.tMid.y - other.box.tMid.y) * (box.tMid.y - other.box.bMid.y) <= 0 ||
        //      (box.bMid.y - other.box.tMid.y) * (box.bMid.y - other.box.bMid.y) <= 0))
        //     return true;
        // else
        //     return false;

        float thisLmid = bounds.center.x - bounds.size.x * 0.5f;
        float thisRmid = bounds.center.x + bounds.size.x * 0.5f;
        float thisTmid = bounds.center.z + bounds.size.z * 0.5f;
        float thisBmid = bounds.center.z - bounds.size.z * 0.5f;
        float otherLmid = other.center.x - other.size.x * 0.5f;
        float otherRmid = other.center.x + other.size.x * 0.5f;
        float otherTmid = other.center.z + other.size.z * 0.5f;
        float otherBmid = other.center.z - other.size.z * 0.5f;


        bool horizon = (otherRmid - thisRmid) * (otherRmid - thisLmid) <= 0 ||
                       (otherLmid - thisRmid) * (otherLmid - thisLmid) <= 0;
        bool vertical = (otherBmid - thisTmid) * (otherBmid - thisBmid) <= 0 ||
                        (otherTmid - thisTmid) * (otherTmid - thisBmid) <= 0;
        Debug.Log(horizon && vertical);
        return horizon && vertical;
    }

    /// <summary>
    /// 插入map，如果超出上限就会分裂
    /// </summary>
    /// <param name="mapInfo"></param>
    public void Insert(QuadMapInfo mapInfo)
    {
        // 是叶子节点就会直接插入，超出上限进行细分
        if (isLeaf)
        {
            mapInfosList.Add(mapInfo);
            if (mapInfosList.Count > MAX_SIZE)
            {
                SubDivide();
            }
        }
        else
        {
            // 非叶子节点与四个子象限进行判定，如果覆盖有多个则存到共享列表
            int count = 0;
            int index = -1;
            for (int i = 0; i < 4; i++)
            {
                if (sonQuads[i].InQuad(mapInfo.GetBound()))
                {
                    count++;
                    index = i;
                }
            }

            if (count == 1)
            {
                sonQuads[index].Insert(mapInfo);
            }
            else
            {
                sharedMapInfosList.Add(mapInfo);
            }
        }
    }
}

/// <summary>
/// 存储节点信息
/// </summary>
public class QuadMapInfo
{
    private Bounds bounds;

    // id从1开始，为0表示不存在
    public uint MapId = 0;

    public QuadMapInfo(Bounds bounds, uint mapId)
    {
        this.bounds = bounds;
        this.MapId = mapId;
    }

    public bool IsInQuad(Vector3 position)
    {
        return bounds.Contains(position);
    }

    public Bounds GetBound()
    {
        return bounds;
    }
}