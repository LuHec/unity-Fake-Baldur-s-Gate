using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using UnityEngine;

/// <summary>
/// 动态id分配池，只处理id存储，不处理物体的消亡
/// </summary>
public class DynamicIDPool
{
    private Dictionary<uint, GameActor> dynamicIDdict;

    private const uint MIN_CHARACTER_ID = 100001;
    private const uint MAX_CHARACTER_ID = 800000;
    private const uint MIN_ITEM_ID = 1000001;
    private const uint MAX_ITEM_ID = 10000000;
    private uint assignableCharacterID;
    private uint assignableItemID;

    /// <summary>
    /// 队列存储回收的id
    /// </summary>
    private Queue<uint> assignableCharacterIDQueue;

    private Queue<uint> assignableItemIDQueue;

    #region #CheckFunction

    /// <summary>
    /// 获取可用的id，并更新可用id
    /// </summary>
    /// <returns>可用的id</returns>
    private uint GetAssignableItemID()
    {
        // 如果队列空了才会推进指针，否则从队列里拿。队列内永远不会存当前指针的值
        if (assignableItemIDQueue.Count == 0)
        {
            return assignableItemID++;
        }

        return assignableItemIDQueue.Dequeue();
    }

    private uint GetAssignableCharacterID()
    {
        if (assignableCharacterIDQueue.Count == 0)
        {
            return assignableCharacterID++;
        }

        return assignableCharacterIDQueue.Dequeue();
    }
    
    public bool CharacterSignable() => assignableCharacterID < MAX_CHARACTER_ID;
    public bool ItemSignable() => assignableItemID < MAX_ITEM_ID;
    private bool ActorExist(uint id) => dynamicIDdict.ContainsKey(id);

    #endregion

    public DynamicIDPool()
    {
        dynamicIDdict = new Dictionary<uint, GameActor>();
        assignableCharacterIDQueue = new Queue<uint>();
        assignableItemIDQueue = new Queue<uint>();

        assignableCharacterID = MIN_CHARACTER_ID;
        assignableItemID = MIN_ITEM_ID;
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
        return dynamicIDdict[id];
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
        if (GetActorById(id).GetActorType() == ActorEnumType.ActorType.Character)
            assignableCharacterIDQueue.Enqueue(id);
        else assignableItemIDQueue.Enqueue(id);

        dynamicIDdict.Remove(id);

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

        dynamicIDdict[assignableCharacterID] = actor;
        actor.InitDynamicId(assignableCharacterID++);
        return true;
    }

    private bool SignItem(GameActor actor)
    {
        if (!ItemSignable()) return false;

        dynamicIDdict[assignableItemID] = actor;
        actor.InitDynamicId(assignableItemID++);
        return true;
    }
}