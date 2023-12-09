using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 管理中心负责管理所有的游戏内物品：物品、角色等。
/// </summary>
public class ActorsManagerCenter : Singleton<ActorsManagerCenter>
{
    private ScriptObjectDataManager _scriptObjectDataManager;
    private DynamicIDPool _dynamicIDPool;
    private MapSystem _mapSystem;
    private HashSet<uint> _controlledActorsSet;
    
    public void Init()
    {
        _dynamicIDPool = new DynamicIDPool();
        _mapSystem = MapSystem.Instance;
        _controlledActorsSet = new HashSet<uint>();
        _scriptObjectDataManager = new ScriptObjectDataManager();
        // LoadAllControlledActorsResource();

        // 暂时禁用，死亡不移除id
        // MessageCenter.Instance.SubmitActorDie(OnActorDie);
    }

    #region #Listener

    public void OnActorDie(System.Object sender, EventArgsType.ActorDieMessage message)
    {
        if(message.removeActor)
            RemoveConActorByDynamicId(message.dead_dynamic_id);
    }

    #endregion

    #region #LoadResource

    /// <summary>
    /// 加载actor资源，并注册到id池
    /// </summary>
    public void LoadAllControlledActorsResource()
    {
        var objects = ResourcesLoader.LoadAllControlledActorsResource();
        foreach (var obj in objects)
        {
            // 初始化数据
            var objCharacter = Object.Instantiate(obj, Vector3.zero, Quaternion.identity);
            var charActor = objCharacter.GetComponent<GameActor>();
            charActor.InitBase(_scriptObjectDataManager.CharacterAttrSOData.DataDictionary[charActor.id],
                ActorEnumType.ActorStateTag.AI);
            // 添加到id池
            SignActor(charActor);

            // 获取初始武器并注册到idpool
            var weaponObj =
                Object.Instantiate(ResourcesLoader.LoadWeaponById(charActor.characterAttribute.WeaponId),
                    Vector3.zero, Quaternion.identity);
            var weaponActor = weaponObj.GetComponent<Weapon>();
            weaponActor.InitBase();
            weaponActor.InitWeaponAttribute(_scriptObjectDataManager.WeaponAttrSOData.weaponAttDict[weaponActor.id]);
            SignActor(weaponActor);

            (charActor as Character).EquipWeapon(weaponActor);

            // 随机生成到地图上可用位置
            Vector2Int randomPos = GetRandomGridPos();
            _mapSystem.SetGridActor(charActor.startPos.x, charActor.startPos.z, charActor);
            charActor.transform.position = charActor.startPos;
        }
    }

    public uint LoadWeapon(uint weaponId)
    {
        // 获取初始武器并注册到idpool
        var weaponObj =
            Object.Instantiate(ResourcesLoader.LoadWeaponById(weaponId),
                Vector3.zero, Quaternion.identity);
        var weaponActor = weaponObj.GetComponent<Weapon>();
        weaponActor.InitBase();
        weaponActor.InitWeaponAttribute(_scriptObjectDataManager.WeaponAttrSOData.weaponAttDict[weaponActor.id]);
        SignActor(weaponActor);

        return weaponActor.DynamicId;
    }

    public uint LoadActorTest(Vector3 position)
    {
        Object obj = ResourcesLoader.LoadTestActorResource();
        // 初始化数据
        var objCharacter = Object.Instantiate(obj, Vector3.zero, Quaternion.identity);
        var charActor = objCharacter.GetComponent<GameActor>();
        charActor.InitBase(_scriptObjectDataManager.CharacterAttrSOData.DataDictionary[charActor.id],
            ActorEnumType.ActorStateTag.AI);
        // 添加到id池
        SignActor(charActor);

        var weapon = GetActorByDynamicId(LoadWeapon(charActor.characterAttribute.WeaponId));
        (charActor as Character).EquipWeapon(weapon);
        
        _mapSystem.SetGridActor(position.x, position.z, charActor);
        charActor.transform.position = position;

        return charActor.DynamicId;
    }

    /// <summary>
    /// 加载四个玩家控制角色
    /// </summary>
    /// <returns></returns>
    public List<uint> LoadPlayerActor()
    {
        var list = new List<uint>();
        list.Add(LoadActorTest(MapSystem.Instance.GetGrid().GetWorldPosition(0, 0)));
        list.Add(LoadActorTest(MapSystem.Instance.GetGrid().GetWorldPosition(0, 1)));
        list.Add(LoadActorTest(MapSystem.Instance.GetGrid().GetWorldPosition(0, 2)));
        list.Add(LoadActorTest(MapSystem.Instance.GetGrid().GetWorldPosition(0, 3)));

        GetActorByDynamicId(list[0]).SetCharacterStateTo(ActorEnumType.ActorStateTag.Player);
        GetActorByDynamicId(list[1]).SetCharacterStateTo(ActorEnumType.ActorStateTag.AI);
        GetActorByDynamicId(list[2]).SetCharacterStateTo(ActorEnumType.ActorStateTag.AI);
        GetActorByDynamicId(list[3]).SetCharacterStateTo(ActorEnumType.ActorStateTag.AI);

        (GetActorByDynamicId(list[0]) as Character).SetAIMode(ActorEnumType.AIMode.Follow);
        (GetActorByDynamicId(list[1]) as Character).SetAIMode(ActorEnumType.AIMode.Follow);
        (GetActorByDynamicId(list[2]) as Character).SetAIMode(ActorEnumType.AIMode.Follow);
        (GetActorByDynamicId(list[3]) as Character).SetAIMode(ActorEnumType.AIMode.Follow);

        return list;
    }

    public Vector2Int GetRandomGridPos()
    {
        return Vector2Int.zero;
    }

    #endregion

    #region #ActorManager

    /// <summary>
    /// 用动态id移除actor，先从控制列表移除，然后从pool中移除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool RemoveConActorByDynamicId(uint id)
    {
        if (_controlledActorsSet.Remove(id) == false) return false;
        _mapSystem.GetGridObject(GetActorByDynamicId(id).transform.position).ClearActor();
        return _dynamicIDPool.RemoveActorById(id);
    }

    #endregion

    #region #Getter

    public GameActor GetActorByDynamicId(uint dynamicId)
    {
        return _dynamicIDPool.GetActorById(dynamicId);
    }


    public List<uint> GetAllConActorsDynamicId()
    {
        List<uint> actorsDynamicId = new List<uint>();
        foreach (var id in _controlledActorsSet)
        {
            actorsDynamicId.Add(id);
        }

        return actorsDynamicId;
    }

    #endregion

    #region #Setter

    /// <summary>
    /// Warning!! -- 只注册未注册过的，你也不想一个实例占用两个id吧？ -- Warning!! 
    /// </summary>
    /// <param name="actor"></param>
    /// <returns></returns>
    private bool SignActor(GameActor actor)
    {
        if (_dynamicIDPool.SignActor(actor) == false)
            return false;
        if (actor.GetActorType() == ActorEnumType.ActorType.Character) _controlledActorsSet.Add(actor.DynamicId);
        return true;
    }

    #endregion
}