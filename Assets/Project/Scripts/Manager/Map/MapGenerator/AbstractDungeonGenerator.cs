using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapVisualizer))]
public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
    protected MapVisualizer mapVisualizer;

    private void Awake()
    {
        mapVisualizer = GetComponent<MapVisualizer>();
    }

    public void GenerateDungeon()
    {
        RunProceduralGenerator();
    }

    public abstract void RunProceduralGenerator();
}