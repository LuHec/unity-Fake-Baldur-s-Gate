using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

/// <summary>
/// 动态id分配池
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

    #region #CheckFunction

    public uint GetAssignableItemID() => _assignableItemID;

    public uint GetAssignableCharacterID() => _assignableCharacterID;

    public bool CharacterSignable() => _assignableCharacterID > MaxCharacterId;
    public bool ItemSignable() => _assignableItemID > MaxItemId;
    private bool ActorExist(uint id) => _dynamicIDdict.ContainsKey(id);

    #endregion

    public DynamicIDPool()
    {
        _dynamicIDdict = new Dictionary<uint, GameActor>();

        _assignableCharacterID = MinCharacterId;
        _assignableItemID = MinItemId;
    }

    /// <summary>
    /// 注册actor，如果已经存在则注册失败
    /// </summary>
    /// <param name="actor"></param>
    /// <returns></returns>
    public bool SignActor(GameActor actor)
    {
        if (ActorExist(actor.dynamic_id)) return false;

        if (actor.GetActorType() == ActorEnumType.ActorType.Character) return SignCharacter(actor);
        if (actor.GetActorType() == ActorEnumType.ActorType.Pickableitem) return SignItem(actor);

        return false;
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
    /// 通过id删除actor，不存在返回false
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool RemoveActorById(uint id)
    {
        if (!ActorExist(id)) return false;

        return _dynamicIDdict.Remove(id);
    }


    private bool SignCharacter(GameActor actor)
    {
        if (!CharacterSignable()) return false;

        _dynamicIDdict[_assignableCharacterID++] = actor;
        return true;
    }

    private bool SignItem(GameActor actor)
    {
        if (!ItemSignable()) return false;

        _dynamicIDdict[_assignableItemID++] = actor;
        return true;
    }
}