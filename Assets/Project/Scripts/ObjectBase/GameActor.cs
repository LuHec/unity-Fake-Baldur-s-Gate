using System;
using UnityEngine;
using System.Collections.Generic;

public class GameActor : MonoBehaviour
{
    [SerializeField] private PlacedObjectTypeSO _placedObjectTypeSo;
    [SerializeField] private float _moveSpeed = 2.0f;
    private CommandQueue _cmdQue;
    public PlacedObjectTypeSO PlacedObject => _placedObjectTypeSo;
    public string _name;
    private MapSystem _mapSystem;

    public void Init()
    {
        _cmdQue = new CommandQueue();
    }

    /// <summary>
    /// 添加命令
    /// </summary>
    /// <param name="cmdInstance"></param>
    /// <returns></returns>
    public bool AddCommand(CommandInstance cmdInstance)
    {
        if (cmdInstance == null) return false; 
        _cmdQue.Add(cmdInstance);
        return true;
    }

    /// <summary>
    /// 获取命令队列的队尾
    /// </summary>
    /// <returns></returns>
    public CommandInstance GetCommand() => _cmdQue?.Back();

    public Vector3 MoveTo(float x, float z)
    {
        transform.position = Vector3.MoveTowards(
            transform.position, new Vector3(x, transform.position.y, z), _moveSpeed);
        
        return transform.position;
    }
}