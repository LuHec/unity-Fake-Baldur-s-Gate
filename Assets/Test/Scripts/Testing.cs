using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private PlayerInput _playerInput;
    private Pathfiding _pathfiding;
    public CharacterAttributesScriptobjectData _characterAttributesScriptobject;

    private void Start()
    {
        // _playerInput = PlayerInput.Instance;
        // _pathfiding = new Pathfiding(15, 15, 3);
        // Debug.Log(Application.dataPath + "/ExcelData/CharacterAttribute.xls");
        _characterAttributesScriptobject = ResourcesLoader.LoadCharacterAttributesData();
        _characterAttributesScriptobject.InitDict();
        Debug.Log(_characterAttributesScriptobject.DataDictionary[10001]);
    }

    // void Update()
    // {
    //     //     if (_playerInput.IsLClick)
    //     //     {
    //     //         var worldPos = _playerInput.GetMouse3DPosition(LayerMask.GetMask("Default"));
    //     //         _pathfiding.GetGrid().GetXZ(worldPos, out int x, out int y);
    //     //         List<PathNode> path = _pathfiding.FindPath(0, 0, x, y);
    //     //         Debug.Log(path == null);
    //     //         if (path != null)
    //     //         {
    //     //             for (int i = 0; i < path.Count - 1; i++)
    //     //             {
    //     //                 Debug.DrawLine(_pathfiding.GetGrid().GetWorldPosition(path[i].x, path[i].y),
    //     //                     _pathfiding.GetGrid().GetWorldPosition(path[i + 1].x, path[i + 1].y), Color.green, 20f);
    //     //             }
    //     //             
    //     //             _pathfiding.Clear();
    //     //         }
    //     //     }
    //     //
    //     //     if (_playerInput.IsRClick)
    //     //     {
    //     //         var worldPos = _playerInput.GetMouse3DPosition(LayerMask.GetMask("Default"));
    //     //         _pathfiding.GetGrid().GetXZ(worldPos, out int x, out int y);
    //     //         _pathfiding.GetGrid().GetGridObject(x, y).Reachable = false;
    //     //     }
    //     // }
    // }
}