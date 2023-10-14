using System;
using UnityEngine;
using System.Collections.Generic;

public class GameActor : MonoBehaviour
{
    [SerializeField] private PlacedObjectTypeSO _placedObjectTypeSo;
    [SerializeField] private float _moveSpeed = 2.0f;
    public uint id;
    public CharacterAttribute characterAttribute;
    public PlacedObjectTypeSO PlacedObject => _placedObjectTypeSo;

    public Vector3 startPos;
    private CommandQueue _cmdQue;

    private MapSystem _mapSystem;

    /// <summary>
    /// 初始化Actor
    /// </summary>
    /// <param name="newCharacterAttribute"></param>
    public void Init(CharacterAttributeSerializable newCharacterAttribute)
    {
        characterAttribute = new CharacterAttribute(newCharacterAttribute.id, newCharacterAttribute.name,
            newCharacterAttribute.maxHp, newCharacterAttribute.maxActPoints, newCharacterAttribute.weaponId);

        _cmdQue = new CommandQueue();

        startPos.x *= MapSystem.Instance.GetGrid().Cellsize;
        startPos.z *= MapSystem.Instance.GetGrid().Cellsize;
        transform.position = startPos;
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
    public CommandInstance GetCommand() => _cmdQue.Back();

    /// <summary>
    /// 输入XZ，返回世界坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Vector3 CalculateMoveTo(float x, float z)
    {
        return Vector3.MoveTowards(
            transform.position, new Vector3(x, transform.position.y, z), _moveSpeed * Time.deltaTime);
    }

    public void DirectMoveTo(Vector3 position)
    {
        transform.position = position;
    }

    public Vector3 MoveTo(float x, float z)
    {
        transform.position = Vector3.MoveTowards(
            transform.position, new Vector3(x, transform.position.y, z), _moveSpeed);

        return transform.position;
    }
}