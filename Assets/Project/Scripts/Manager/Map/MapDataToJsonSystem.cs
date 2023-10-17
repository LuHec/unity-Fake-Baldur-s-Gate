using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Windows;
using System.IO;
using File = System.IO.File;

public class MapDataToJsonSystem
{
    private HashSet<Vector3> buildPos;
    private readonly string savePath = Application.dataPath + "/MapData/mapsave.json";

    public MapDataToJsonSystem()
    {
        buildPos = new HashSet<Vector3>();
    }

    public void RecordBuild(Vector3 pos)
    {
        buildPos.Add(pos);
    }

    public void RemoveBuild(Vector3 pos)
    {
        buildPos.Remove(pos);
    }
    
    public void SaveMap()
    {
        Debug.Log("11");
        MapSaveInfo data = new MapSaveInfo();
        var grid = MapSystem.Instance.GetGrid();

        data.width = grid.Width;
        data.height = grid.Height;
        data.cellsize = grid.Cellsize;
        data.placeObject = buildPos.ToArray();

        string json = JsonUtility.ToJson(data, true);
        
        File.WriteAllText(savePath, json);
    }

    public void LoadMap()
    {
        
    }
}
