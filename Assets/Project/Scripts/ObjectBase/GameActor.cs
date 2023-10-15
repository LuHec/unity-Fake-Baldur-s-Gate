using System;
using UnityEngine;
using System.Collections.Generic;

public class GameActor : MonoBehaviour
{
    #region #Info

    protected ActorEnumType.ActorType _actorEnumType;
    private ActorEnumType.ActorStateTag _actorStateTag;

    public ActorEnumType.ActorType GetActorType() => _actorEnumType;
    public ActorEnumType.ActorStateTag GetActorStateTag() => _actorStateTag;
  
    /// <summary>
    /// 资源加载id
    /// </summary>
    public uint id;
    
    /// <summary>
    /// 运行时id
    /// </summary>
    private uint dynamic_id;

    public uint Dynamic_Id => dynamic_id;
    public float speed;

    #endregion
    

    [SerializeField] private PlacedObjectTypeSO _placedObjectTypeSo;
    [SerializeField] private float _moveSpeed = 2.0f;
    
    public CharacterAttribute characterAttribute;
    public PlacedObjectTypeSO PlacedObject => _placedObjectTypeSo;

    public Vector3 startPos;
    private CommandQueue _cmdQue;

    private MapSystem _mapSystem;

    /// <summary>
    /// 初始化Actor
    /// </summary>
    /// <param name="newCharacterAttribute"></param>
    public void InitBase(CharacterAttributeSerializable newCharacterAttribute)
    {
        characterAttribute = new CharacterAttribute(newCharacterAttribute.id, newCharacterAttribute.name,
            newCharacterAttribute.maxHp, newCharacterAttribute.maxActPoints, newCharacterAttribute.weaponId);

        _cmdQue = new CommandQueue();

        startPos.x *= MapSystem.Instance.GetGrid().Cellsize;
        startPos.z *= MapSystem.Instance.GetGrid().Cellsize;
        transform.position = startPos;

        InitExtend();
    }

    /// <summary>
    /// 初始化Actor子类的类型
    /// </summary>
    protected virtual void InitExtend()
    {
    }

    public void InitDynamicId(uint id)
    {
        dynamic_id = id;
    }
    
    /// <summary>
    /// 设置当前为AI控制还是角色控制
    /// </summary>
    /// <param name="newStateTag"></param>
    public void SetCharacterStateTo(ActorEnumType.ActorStateTag newStateTag)
    {
        _actorStateTag = newStateTag;
    }

    /// <summary>
    /// 生成AI
    /// </summary>
    public virtual void SelfAICalculate()
    {
        Debug.Log(dynamic_id + " " + "Ai Running....");
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

    public virtual void Attack(GameActor actor)
    {
        actor.Hurt();
    }

    public virtual void Hurt()
    {
        
    }
}