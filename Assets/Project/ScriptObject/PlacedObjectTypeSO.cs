using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/PlacedObject")]
public class PlacedObjectTypeSO : ScriptableObject
{
    public string nameString;
    public int width;
    public int height;
    public Transform prefab;

    public enum Dir
    {
        Up,
        Right,
        Down,
        Left,
    }

    /// <summary>
    /// 返回下一个旋转的方向
    /// </summary>
    /// <param name="dir">当前方向</param>
    /// <returns>下一个旋转方向</returns>
    public Dir GetNextDir(Dir dir) => dir switch
    {
        Dir.Up => Dir.Right,
        Dir.Right => Dir.Down,
        Dir.Down => Dir.Left,
        Dir.Left => Dir.Up,
        _ => Dir.Up
    };

    /// <summary>
    /// 获取当前方向需要旋转绕y轴多少度
    /// </summary>
    /// <param name="dir">当前方向</param>
    /// <returns>绕y轴旋转的角度</returns>
    public float GetRotationAngle(Dir dir) => dir switch
    {
        Dir.Up => 0f,
        Dir.Right => 90f,
        Dir.Down => 180f,
        Dir.Left => 270f,
        _ => 0f
    };

    /// <summary>
    /// 返回旋转后要偏移的位移
    /// </summary>
    /// <param name="dir">当前方向</param>
    /// <returns>旋转后要偏移的位移</returns>
    public Vector2Int GetRotationOffset(Dir dir) => dir switch
    {
        Dir.Up => new Vector2Int(0, 0),
        Dir.Right => new Vector2Int(0, width),
        Dir.Down => new Vector2Int(width, height),
        Dir.Left => new Vector2Int(height, 0),
        _ => Vector2Int.zero
    };

    /// <summary>
    /// 返回该单位占领的格子
    /// </summary>
    /// <param name="offset">单位在棋盘上的位置</param>
    /// <param name="dir">单位占领的方向</param>
    /// <returns>单位占领的格子</returns>
    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir)
    {
        List<Vector2Int> listVec2 = new List<Vector2Int>();

        switch (dir)
        {
            case Dir.Up:
            case Dir.Down:
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        listVec2.Add(offset + new Vector2Int(x, z));
                    }
                }

                break;
            }

            case Dir.Left:
            case Dir.Right:
            {
                for (int x = 0; x < height; x++)
                {
                    for (int z = 0; z < width; z++)
                    {
                        listVec2.Add(offset + new Vector2Int(x, z));
                    }
                }

                break;
            }
        }


        return listVec2;
    }
}