using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using UnityEngine;

/// <summary>
/// 动态id分配池，只处理id存储，不处理物体的消亡
/// </summary>
public class DynamicIDPool
{
    private Dictionary<uint, GameActor> _dynamicIDdict;

    private const uint MinCharacterId = 100001;
    private const uint MaxCharacterId = 800000;
    private const uint MinItemId = 1000001;
    private const uint MaxItemId = 10000000;
    private uint _assignableCharacterID;
    private uint _assignableItemID;

    /// <summary>
    /// 队列存储回收的id
    /// </summary>
    private Queue<uint> _assignableCharacterIDQueue;
    private Queue<uint> _assignableItemIDQueue;

    #region #CheckFunction

    /// <summary>
    /// 获取可用的id，并更新可用id
    /// </summary>
    /// <returns>可用的id</returns>
    private uint GetAssignableItemID()
    {
        // 如果队列空了才会推进指针，否则从队列里拿，需要注意队列永远不会存当前指针的值
        if (_assignableItemIDQueue.Count == 0)
        {
            return _assignableItemID++;
        }
        
        return _assignableItemIDQueue.Dequeue();
    }
    
    private uint GetAssignableCharacterID()
    {
        if (_assignableCharacterIDQueue.Count == 0)
        {
            return _assignableCharacterID++;
        }

        return _assignableCharacterIDQueue.Dequeue();
    }
    

    public bool CharacterSignable() => _assignableCharacterID < MaxCharacterId;
    public bool ItemSignable() => _assignableItemID < MaxItemId;
    private bool ActorExist(uint id) => _dynamicIDdict.ContainsKey(id);

    #endregion

    public DynamicIDPool()
    {
        _dynamicIDdict = new Dictionary<uint, GameActor>();
        _assignableCharacterIDQueue = new Queue<uint>();
        _assignableItemIDQueue = new Queue<uint>();

        _assignableCharacterID = MinCharacterId;
        _assignableItemID = MinItemId;
    }

    /// <summary>
    /// 注册actor，舍弃原来的id
    /// </summary>
    /// <param name="actor"></param>
    /// <returns></returns>
    public bool SignActor(GameActor actor)
    {
        if (actor.GetActorType() == ActorEnumType.ActorType.Character) return SignCharacter(actor);
        if (actor.GetActorType() == ActorEnumType.ActorType.Pickableitem) return SignItem(actor);

        return true;
    }

    /// <summary>
    /// 通过id获取Actor，不存在返回空
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public GameActor GetActorById(uint id)
    {
        if (!ActorExist(id)) return null;
        return _dynamicIDdict[id];
    }

    /// <summary>
    /// 通过id删除actor，不存在返回false，id会被回收
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool RemoveActorById(uint id)
    {
        if (!ActorExist(id)) return false;

        // 回收id
        if(GetActorById(id).GetActorType() == ActorEnumType.ActorType.Character) _assignableCharacterIDQueue.Enqueue(id);
        else _assignableItemIDQueue.Enqueue(id);
        
        _dynamicIDdict.Remove(id);
        
        return true;
    }


    /// <summary>
    /// 注册并初始化actor的动态id
    /// </summary>
    /// <param name="actor"></param>
    /// <returns></returns>
    private bool SignCharacter(GameActor actor)
    {
        if (!CharacterSignable()) return false;

        _dynamicIDdict[_assignableCharacterID] = actor;
        actor.InitDynamicId(_assignableCharacterID ++);
        return true;
    }

    private bool SignItem(GameActor actor)
    {
        if (!ItemSignable()) return false;

        _dynamicIDdict[_assignableItemID++] = actor;
        actor.InitDynamicId(_assignableItemID ++);
        return true;
    }
}