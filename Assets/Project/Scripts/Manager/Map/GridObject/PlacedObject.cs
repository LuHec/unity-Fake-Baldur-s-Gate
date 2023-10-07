using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 附着在建筑prefab上
/// </summary>
public class PlacedObject : MonoBehaviour
{
    private PlacedObjectTypeSO _placedObjectTypeSo;
    private PlacedObjectTypeSO.Dir _dir;
    private Vector2Int _origin;

    /// <summary>
    /// 创建建筑实体，并记录坐标和方向
    /// </summary>
    /// <param name="worldPos"></param>
    /// <param name="origin"></param>
    /// <param name="dir"></param>
    /// <param name="placedObjectTypeSo"></param>
    /// <returns></returns>
    public static PlacedObject Create(Vector3 worldPos, Vector2Int origin, PlacedObjectTypeSO.Dir dir,
        PlacedObjectTypeSO placedObjectTypeSo)
    {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSo.prefab, worldPos,
            Quaternion.Euler(
                new Vector3(0, placedObjectTypeSo.GetRotationAngle(dir), 0))
        );

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

        placedObject._placedObjectTypeSo = placedObjectTypeSo;
        placedObject._origin = origin;
        placedObject._dir = dir;

        return placedObject;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 获取占用的格子
    /// </summary>
    /// <returns></returns>
    public List<Vector2Int> GetGridPositionList()
    {
        return _placedObjectTypeSo.GetGridPositionList(_origin, _dir);
    }
}