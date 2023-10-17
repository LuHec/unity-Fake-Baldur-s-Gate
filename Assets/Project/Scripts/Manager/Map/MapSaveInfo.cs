using System;
using UnityEngine;

[Serializable]
public class MapSaveInfo
{
    public int width;
    public int height;
    public float cellsize;

    public Vector3[] placeObject;
}