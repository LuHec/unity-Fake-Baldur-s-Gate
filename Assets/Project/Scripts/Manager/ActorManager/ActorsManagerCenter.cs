using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 管理中心负责管理所有的游戏内物品：物品、角色等。
/// </summary>
public class ActorsManagerCenter : ICenter
{
    public int CurrentPlayerControlledPtr => _currentPlayerControlledPtr;
    public int CurrentSystemControlledPtr => _currentSystemControlledPtr;

    private List<GameActor> _systemControlledActors;
    private List<GameActor> _playerControlledActors;
    private Dictionary<Transform, GameActor> _actorsDict;
    private MapSystem _mapSystem;
    private int _currentPlayerControlledPtr;
    private int _currentSystemControlledPtr;


    public ActorsManagerCenter(MapSystem mapSystem)
    {
        _mapSystem = mapSystem;
        _systemControlledActors = new List<GameActor>();
        _playerControlledActors = new List<GameActor>();
        _actorsDict = new Dictionary<Transform, GameActor>();

        Init();
    }

    // public ActorsManagerCenter(List<GameActor> listActors)
    // {
    //     _listActors = listActors;
    // }

    public void Init()
    {
        // LoadActorsResource();
        LoadAllActorsResource();
    }

    public bool PushSystemControlledPtr()
    {
        bool finished = CurrentSystemControlledPtr + 1 > _systemControlledActors.Count;
        _currentSystemControlledPtr = (_currentSystemControlledPtr + 1) % _systemControlledActors.Count;
        
        return finished;
    }

    public bool PushPlayerControlledPtr()
    {
        bool finished = CurrentPlayerControlledPtr + 1 > _playerControlledActors.Count;
        _currentPlayerControlledPtr = (_currentPlayerControlledPtr + 1) % _playerControlledActors.Count;
        return finished;
    }

    public List<GameActor> LoadAllActorsResource()
    {
        var objects = Resources.LoadAll("Actors/Character");
        foreach (var obj in objects)
        {
            var iniobj = Object.Instantiate(obj, Vector3.zero, Quaternion.identity);
            var actor = iniobj.GetComponent<GameActor>();
            actor.Init();
            _playerControlledActors.Add(actor);
            _mapSystem.SetGridActor(0, 0, _playerControlledActors[^1]);
        }

        return _playerControlledActors;
    }

    public GameActor LoadActorsResource()
    {
        var obj = Resources.Load("Actors/Character/character");
        var iniobj = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity);
        var actor = iniobj.GetComponent<GameActor>();
        actor.Init();
        _playerControlledActors.Add(actor);
        return actor;
    }

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
    /// 移除操作列表的角色
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
    /// 获取玩家控制列表
    /// </summary>
    /// <returns></returns>
    public List<GameActor> GetPlayerControlledActorsList() => _playerControlledActors;

    /// <summary>
    /// 获取系统控制列表
    /// </summary>
    /// <returns></returns>
    public List<GameActor> GetSystemControlledActorList() => _systemControlledActors;

    public GameActor GetActor(Transform transform)
    {
        return null;
    }

    public void CenterUpdate()
    {
    }
}