using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 管理中心负责管理所有的游戏内物品：物品、角色等。
/// </summary>
public class ActorsManagerCenter
{
    private ScriptObjectDataManager _scriptObjectDataManager;

    private DynamicIDPool _dynamicIDPool;

    // private List<GameActor> _systemControlledActors;
    // private List<GameActor> _playerControlledActors;
    // private Dictionary<Transform, GameActor> _actorsDict;
    private MapSystem _mapSystem;

    // 当前持有的所有可控制Actor
    private Dictionary<uint, GameActor> _controlledActors;

    public ActorsManagerCenter()
    {
        // _systemControlledActors = new List<GameActor>();
        // _playerControlledActors = new List<GameActor>();
        _dynamicIDPool = new DynamicIDPool();
        _mapSystem = MapSystem.Instance;
        _controlledActors = new Dictionary<uint, GameActor>();
        // _actorsDict = new Dictionary<Transform, GameActor>();

        Init();
    }

    public void Init()
    {
        _scriptObjectDataManager = new ScriptObjectDataManager();
        LoadAllControlledActorsResource();
    }

    #region #NoUse

    #endregion

    #region LoadResource

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
            charActor.InitBase(_scriptObjectDataManager.CharacterAttrSOData.DataDictionary[charActor.id]);
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
        }
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
        if (_controlledActors.Remove(id) == false) return false;
        return _dynamicIDPool.RemoveActorById(id);
    }

    #endregion

    #region #Getter

    public GameActor GetActorByDynamicId(uint dynamicId)
    {
        return _dynamicIDPool.GetActorById(dynamicId);
    }

    /// <summary>
    /// Warning!! -- Sign之前需要删除 -- Warning!! 
    /// </summary>
    /// <param name="actor"></param>
    /// <returns></returns>
    public bool SignActor(GameActor actor)
    {
        if (_dynamicIDPool.SignActor(actor) == false)
            return false;
        if (actor.GetActorType() == ActorEnumType.ActorType.Character) _controlledActors[actor.Dynamic_Id] = actor;
        return true;
    }

    public List<uint> GetAllConActorsDynamicId()
    {
        List<uint> actorsDynamicId = new List<uint>();
        foreach (var pair in _controlledActors)
        {
            actorsDynamicId.Add(pair.Key);
        }

        return actorsDynamicId;
    }

    #endregion
}