using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MapVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject map;

    protected HashSet<GameObject> generatedPlanes = new HashSet<GameObject>();
    protected HashSet<GameObject> generatedWalls = new HashSet<GameObject>();

    private GameObject generatedPlane;
    private GameObject generatedWall;

    private void Start()
    {
        generatedPlane = Resources.Load<GameObject>("ProceduralMapGeneration/Plane");
        generatedWall = Resources.Load<GameObject>("ProceduralMapGeneration/Wall");
    }

    public void GeneratePlane(Vector3 position)
    {
        var obj = Instantiate(generatedPlane, position, Quaternion.identity);
        obj.transform.SetParent(map.transform);
        generatedPlanes.Add(obj);
    }

    public void GeneratePlane(HashSet<Vector2Int> floorPositions, SimpleRandomWalkSo randomWalkParameters)
    {
        foreach (var vec2 in floorPositions)
        {
            Vector3 vec3 = new Vector3(vec2.x * randomWalkParameters.generatPlaneWidth, 0,
                vec2.y * randomWalkParameters.generatPlaneWidth);
            GeneratePlane(vec3);
        }
    }

    public void GenerateWall(Vector3 position)
    {
        var obj = Instantiate(generatedWall, position, Quaternion.identity);
        obj.transform.SetParent(map.transform);
        generatedWalls.Add(obj);
    }

    public void GenerateWall(HashSet<Vector2Int> wallPositions)
    {
        foreach (var vec2 in wallPositions)
        {
            Vector3 vec3 = new Vector3(vec2.x, 0f, vec2.y);
            GenerateWall(vec3);
        }
    }

    public void Clear()
    {
        ClearPlanes();
        ClearWalls();
    }

    public void ClearPlanes()
    {
        foreach (var obj in generatedPlanes)
        {
            Destroy(obj);
        }

        generatedPlanes.Clear();
    }

    public void ClearWalls()
    {
        foreach (var obj in generatedWalls)
        {
            Destroy(obj);
        }

        generatedWalls.Clear();
    }
}