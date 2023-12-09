using System;
using UnityEditor;
using UnityEngine;

// true表示适用于其子类
[CustomEditor(typeof(AbstractDungeonGenerator), true)]
public class RandomDungeonGeneratorEditor : Editor
{
    private AbstractDungeonGenerator generator;

    private void Awake()
    {
        generator = target as AbstractDungeonGenerator;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Creat Dungeon"))
        {
            generator.GenerateDungeon();
        }
    }
}