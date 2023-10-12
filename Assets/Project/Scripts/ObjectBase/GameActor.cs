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
    public float X => _x;
    public float Z => _z;
    private float _x;
    private float _z;

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
        Debug.Log(_cmdQue == null);
        _cmdQue.Add(cmdInstance);
        return true;
    }

    /// <summary>
    /// 获取命令队列的队尾
    /// </summary>
    /// <returns></returns>
    public CommandInstance GetCommand() => _cmdQue?.Back();

    public void MoveTo(float x, float z)
    {
        _x = x;
        _z = z;
        transform.position = Vector3.MoveTowards(
            transform.position, new Vector3(x, transform.position.y, z), _moveSpeed);
        Debug.Log(transform.position);
    }
}