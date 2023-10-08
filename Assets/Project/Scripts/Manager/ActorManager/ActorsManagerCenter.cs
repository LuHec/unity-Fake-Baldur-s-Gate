using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorsManagerCenter : ICenter
{
    private List<GameActor> _listActors;
    private List<GameActor> _controlledActors;
    private HashSet<GameActor> _actorsSet;

    public ActorsManagerCenter()
    {
        
    }

    public ActorsManagerCenter(List<GameActor> listActors)
    {
        _listActors = listActors;
    }

    /// <summary>
    /// 添加角色到操作列表中
    /// </summary>
    /// <param name="actor">角色</param>
    public void AddControlledActor(GameActor actor)
    {
        if (!_actorsSet.Contains(actor))
        {
            _actorsSet.Add(actor);
            _controlledActors.Add(actor);
        }
    }

    /// <summary>
    /// 移除操作列表的角色
    /// </summary>
    /// <param name="actor">角色</param>
    /// <returns>是否成功</returns>
    public bool RemoveControlledActor(GameActor actor)
    {
        if (!_actorsSet.Contains(actor))
            return false;

        // 寻找actor，和最后一个交换，然后再删除
        for (int i = 0; i < _controlledActors.Count; i++)
        {
            if (_controlledActors[i] == actor)
            {
                (_controlledActors[i], _controlledActors[^1]) = (_controlledActors[^1], _controlledActors[i]);
            }
        }

        _controlledActors.RemoveAt(_controlledActors.Count - 1);

        return true;
    }

    public void CenterUpdate()
    {
    }
}