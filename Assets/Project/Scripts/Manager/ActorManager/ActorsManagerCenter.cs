using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 管理中心负责管理所有的游戏内物品：物品、角色等。
/// </summary>
public class ActorsManagerCenter
{
    private ScriptObjectDataManager _scriptObjectDataManager;
    private DynamicIDPool _dynamicIDPool;
    private List<GameActor> _systemControlledActors;
    private List<GameActor> _playerControlledActors;
    private Dictionary<Transform, GameActor> _actorsDict;
    private MapSystem _mapSystem;

    // 当前持有的所有可控制Actor
    private Dictionary<uint, GameActor> _controlledActors;

    public ActorsManagerCenter()
    {
        _systemControlledActors = new List<GameActor>();
        _playerControlledActors = new List<GameActor>();
        _dynamicIDPool = new DynamicIDPool();
        _mapSystem = MapSystem.Instance;
        _actorsDict = new Dictionary<Transform, GameActor>();

        Init();
    }

    /// <summary>
    /// 加载所有Actors资源，并且添加到Id池中
    /// </summary>
    /// <param name="i"></param>
    public ActorsManagerCenter(int i)
    {
    }

    public void Init()
    {
        _scriptObjectDataManager = new ScriptObjectDataManager();
        LoadAllControlledActorsResource();
    }

    public void Init(int i)
    {
        _scriptObjectDataManager = new ScriptObjectDataManager();
    }

    #region #NoUse

    #endregion

    #region LoadResource

    public List<GameActor> LoadAllControlledActorsResource()
    {
        var objects = ResourcesLoader.LoadAllControlledActorsResource();
        foreach (var obj in objects)
        {
            var iniobj = Object.Instantiate(obj, Vector3.zero, Quaternion.identity);
            var actor = iniobj.GetComponent<GameActor>();
            actor.InitBase(_scriptObjectDataManager.CharacterAttrSOData.DataDictionary[actor.id]);
            _playerControlledActors.Add(actor);
            SignActor(actor);
            _mapSystem.SetGridActor(actor.startPos.x, actor.startPos.z, _playerControlledActors[^1]);
        }

        return _playerControlledActors;
    }

    /// <summary>
    /// 加载actor资源，并注册到id池
    /// </summary>
    /// <param name="i"></param>
    public void LoadAllControlledActorsResource(int i)
    {
        var objects = ResourcesLoader.LoadAllControlledActorsResource();
        foreach (var obj in objects)
        {
            var iniobj = Object.Instantiate(obj, Vector3.zero, Quaternion.identity);
            var actor = iniobj.GetComponent<GameActor>();
            actor.InitBase(_scriptObjectDataManager.CharacterAttrSOData.DataDictionary[actor.id]);

            // 添加到id池
            SignActor(actor);
            
            // 随机生成到地图上可用位置
            Vector2Int randomPos = GetRandomGridPos();
            _mapSystem.SetGridActor(actor.startPos.x, actor.startPos.z, _playerControlledActors[^1]);
        }
    }

    public Vector2Int GetRandomGridPos()
    {
        return Vector2Int.zero;
    }

    #endregion

    #region #ActorManager

    /// <summary>
    /// 添加角色到操作列表中
    /// </summary>
    /// <param name="actor">角色</param>
    public bool AddControlledActor(GameActor actor)
    {
        if (_actorsDict.ContainsKey(actor.transform)) return false;

        _actorsDict.Add(actor.transform, actor);
        _playerControlledActors.Add(actor);
        return true;
    }

    /// <summary>
    /// 移除操作列表的角色，转移版本，非删除
    /// </summary>
    /// <param name="actor">角色</param>
    /// <returns>是否成功</returns>
    public bool RemoveControlledActor(GameActor actor)
    {
        if (!_actorsDict.ContainsKey(actor.transform)) return false;

        // 寻找actor，和最后一个交换，然后再删除。这里排序并不重要
        for (int i = 0; i < _playerControlledActors.Count; i++)
        {
            if (_playerControlledActors[i] == actor)
            {
                (_playerControlledActors[i], _playerControlledActors[^1]) =
                    (_playerControlledActors[^1], _playerControlledActors[i]);
            }
        }

        _playerControlledActors.RemoveAt(_playerControlledActors.Count - 1);
        _actorsDict.Remove(actor.transform);

        return true;
    }

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

    /// <summary>
    /// 获取玩家控制列表
    /// </summary>
    /// <returns></returns>
    public List<GameActor> GetPlayerControlledActorsList() => _playerControlledActors;

    /// <summary>
    /// 获取系统控制列表
    /// </summary>
    /// <returns></returns>
    public List<GameActor> GetSystemControlledActorList() => _systemControlledActors;

    public GameActor GetActorByDynamicId(uint dynamicId)
    {
        return _dynamicIDPool.GetActorById(dynamicId);
    }

    public bool SignActor(GameActor actor)
    {
        if (_dynamicIDPool.SignActor(actor) == false)
            return false;
        _controlledActors[actor.Dynamic_Id] = actor;
        return true;
    }

    #endregion
}