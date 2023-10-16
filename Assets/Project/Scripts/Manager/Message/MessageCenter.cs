using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MessageCenter : Singleton<MessageCenter>
{
    private MapSystem _mapSystem;
    public GlobalState globalState;
    private ActorsManagerCenter _actorsManagerCenter;

    private void Start()
    {
        _mapSystem = MapSystem.Instance;
        globalState = new GlobalState();
    }

    public void Init(ActorsManagerCenter actorsManagerCenter)
    {
        _actorsManagerCenter = actorsManagerCenter;
    }

    public void SubmitOnActorDied(ref EventHandler<EventArgsType.ActorDieMessage> handler)
    {
        handler += OnActorDied;
    }

    public void SubmitOnActorAttacking(ref EventHandler<EventArgsType.ActorAttackingMessage> handler)
    {
        handler += OnActorAttacked;
    }

    public void OnActorDied(object sender, EventArgsType.ActorDieMessage message)
    {
        Debug.Log("I'm Dieeeeeee!!!!!");
    }

    public void OnActorAttacked(object sender, EventArgsType.ActorAttackingMessage message)
    {
        Debug.Log("attacker: " + message.attacker_dynamic_id + " attacked :" + message.attacked_dynamic_id);
        // AddNewTurnByAttack(message.attacker_dynamic_id, message.attacked_dynamic_id);
    }

    /// <summary>
    /// 由玩家攻击导致的新回合添加，会将附近所有范围内符合条件的角色加入新回合，
    /// 如果有些角色已经处在某个回合中，则合并多个回合
    /// </summary>
    public void AddNewTurnByAttack(uint attackerId, uint attackedId)
    {
        Debug.Log("New Turn Add !!!");
        var turnInstances = TurnManager.Instance.turnInstances;
        int idx = -1;

        Character attackerChar = _actorsManagerCenter.GetActorByDynamicId(attackerId) as Character;
        
        // 先获取id列表，然后统一加载，Set去重
        HashSet<uint> newTurnSet = new HashSet<uint>();
        List<uint> allIds = _actorsManagerCenter.GetAllConActorsDynamicId();
        
        // 按范围寻找敌人，如果敌人已在回合内，合并敌人的回合
        foreach (var id in allIds)
        {
            // 范围内
            if (Vector3.SqrMagnitude(attackerChar.transform.position -
                                     _actorsManagerCenter.GetActorByDynamicId(id).transform.position) < 250)
            {
                newTurnSet.Add(id);
            }
        }

        // 存储已经找过的回合，用字典应对连续删除下标的问题
        Dictionary<int, TurnInstance> turnSearched = new Dictionary<int, TurnInstance>();
        List<List<uint>> rres = new List<List<uint>>();
        
        // 通过范围找到的id
        foreach (var id in newTurnSet)
        {
            // 已有回合内找人
            for (int i = 0; i < turnInstances.Count; i++)
            {
                // 如果找到了，且回合未被合并过，就添加所有的id，并标记回合已被搜索
                if (turnInstances[i].conActorIDSet.Contains(id) && !turnSearched.ContainsKey(i))
                {
                    rres.Add(turnInstances[i].conActorIDSet.ToList());
                    turnSearched.Add(i, turnInstances[i]);
                }
            }
        }
        
        // 添加最后得到的id，并且移除找过的回合
        foreach (var pair in turnSearched)
        {
            turnInstances.Remove(pair.Value);
        }

        List<uint> res = new List<uint>();
        foreach (var list in rres)
        {
            res.AddRange(list);
        }
        
        TurnManager.Instance.AddTurn(res);
    }


    /// <summary>
    /// 当回合结束时会调用，每回合检查列表是否还有敌人，如果没了就全部退出，并调整模式
    /// </summary>
    public void OnTurnEnd()
    {
        
    }
}