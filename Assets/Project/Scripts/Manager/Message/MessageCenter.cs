using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;


public class MessageCenter : Singleton<MessageCenter>
{
    [SerializeField] private float maxEnemySearchDistance = 20f;
    private MapSystem _mapSystem;
    public GlobalState globalState;
    private ActorsManagerCenter _actorsManagerCenter;

    #region #delegate

    public event EventHandler<EventArgsType.UpdateTurnManagerMessage> TurnUpdateHandler;
    public event EventHandler<EventArgsType.ActorDieMessage> ActorDieHandler;

    #endregion

    private void Start()
    {
        _mapSystem = MapSystem.Instance;
        globalState = new GlobalState();
    }

    public void Init(ActorsManagerCenter actorsManagerCenter)
    {
        _actorsManagerCenter = actorsManagerCenter;
    }

    #region #Listener

    public void ListenOnActorDied(ref EventHandler<EventArgsType.ActorDieMessage> handler)
    {
        handler += OnActorDied;
    }

    public void ListenOnActorAttacking(ref EventHandler<EventArgsType.ActorAttackingMessage> handler)
    {
        handler += OnActorAttacked;
    }

    public void OnActorDied(object sender, EventArgsType.ActorDieMessage message)
    {
        Debug.Log("Id " + "Died");
        //     
        OnActorDie(message);
    }

    /// <summary>
    /// 暂时只播报玩家控制角色
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    public void OnActorAttacked(object sender, EventArgsType.ActorAttackingMessage message)
    {
        Debug.Log("attacker: " + message.attacker_dynamic_id + " attacked :" + message.attacked_dynamic_id);
        AddNewTurnByAttack(message.attacker_dynamic_id, message.attacked_dynamic_id);
    }

    #endregion

    #region #Sender

    public void SubmitUpdateTurn(EventHandler<EventArgsType.UpdateTurnManagerMessage> listener)
    {
        TurnUpdateHandler += listener;
    }

    private void UpdateTurn(EventArgsType.UpdateTurnManagerMessage message)
    {
        TurnUpdateHandler?.Invoke(this, message);
    }

    public void SubmitActorDie(EventHandler<EventArgsType.ActorDieMessage> listener)
    {
        ActorDieHandler += listener;
    }

    private void OnActorDie(EventArgsType.ActorDieMessage message)
    {
        ActorDieHandler?.Invoke(this, message);
    }

    #endregion

    /// <summary>
    /// 合并两个回合，回合2的所有actor都会被改成回合1
    /// </summary>
    /// <param name="newTurn"></param>
    /// <param name="oldTurn"></param>
    /// <param name="idSet"></param>
    private void MergeTurn(TurnInstance newTurn, TurnInstance oldTurn, HashSet<uint> idSet,
        HashSet<TurnInstance> removeSet)
    {
        // 把旧回合的所有id都取出来放到新回合
        List<uint> oldIdList = oldTurn.ConActorDynamicIDs;
        foreach (uint id in oldIdList)
        {
            if (idSet.Add(id))
            {
                removeSet.Add(_actorsManagerCenter.GetActorByDynamicId(id).CurrentTurn);
                newTurn.AddActorByDynamicId(id);
            }
        }
    }

    private List<uint> FindAllActorIdInDistance(uint centerId, float distance)
    {
        List<uint> resList = new List<uint>();

        // 从直接ActorManager的列表里找，效率更高
        var idList = _actorsManagerCenter.GetAllConActorsDynamicId();

        GameActor centerActor = _actorsManagerCenter.GetActorByDynamicId(centerId);

        // 如果满足距离条件则加入List
        foreach (uint id in idList)
        {
            if (id != centerId)
            {
                if (Vector3.Distance(centerActor.transform.position,
                        _actorsManagerCenter.GetActorByDynamicId(id).transform.position) <= distance)
                {
                    resList.Add(id);
                }
            }
        }

        return resList;
    }

    private void AddNewTurnByAttack(uint attackerId, uint attackedId)
    {
        GameActor attacker = _actorsManagerCenter.GetActorByDynamicId(attackerId);

        // 受到攻击的对象可能不在侦测范围内
        GameActor beAttacked = _actorsManagerCenter.GetActorByDynamicId(attackedId);

        // 判断是否需要移除
        bool needRemove = false;

        // 更新Instance
        TurnInstance newTurn = attacker.CurrentTurn;

        // 去重
        HashSet<uint> idSet = new HashSet<uint>();
        List<uint> distIdList = FindAllActorIdInDistance(attackerId, maxEnemySearchDistance);

        // 需要去除的回合
        HashSet<TurnInstance> removeSet = new HashSet<TurnInstance>();

        if (newTurn == null)
        {
            newTurn = new TurnInstance();
            newTurn.AddActorByDynamicId(attackerId);
        }

        idSet.Add(attackerId);

        // 被攻击对象可能已经死亡
        if (beAttacked)
        {
            if (beAttacked.CurrentTurn == null)
            {
                newTurn.AddActorByDynamicId(beAttacked.Dynamic_Id);
                idSet.Add(beAttacked.Dynamic_Id);
            }
            else
            {
                if (beAttacked.CurrentTurn != newTurn)
                {
                    MergeTurn(newTurn, beAttacked.CurrentTurn, idSet, removeSet);
                }
            }
        }

        // 范围内
        foreach (uint id in distIdList)
        {
            GameActor distActor = _actorsManagerCenter.GetActorByDynamicId(id);

            // 如果不在回合内则直接加入
            if (distActor.CurrentTurn == null)
            {
                newTurn.AddActorByDynamicId(distActor.Dynamic_Id);
                idSet.Add(distActor.id);
            }
            else
            {
                // 如果在回合内，则根据是否为一个回合进行合并
                if (distActor.CurrentTurn != newTurn)
                {
                    // 合并需要移除回合
                    MergeTurn(attacker.CurrentTurn, distActor.CurrentTurn, idSet, removeSet);
                }
                // 在一个回合就不用管了
            }
        }

        // newTurn会检测是否为已有回合；removeSet如果传空什么也不会做
        EventArgsType.UpdateTurnManagerMessage message = new EventArgsType.UpdateTurnManagerMessage(newTurn, removeSet);

        Debug.Log("TurnCount: " + TurnManager.Instance.TurnCount + "NewTurn: " + newTurn.ConActorDynamicIDs.Count);

        // // 如果没有新的回合添加，就传空信息
        // if (needRemove || oldTurnInstance != newTurn)
        //     message = new EventArgsType.UpdateTurnManagerMessage(idSet, removeSet);
        UpdateTurn(message);
    }

    /// <summary>
    /// 当回合结束时会调用，每回合检查列表是否还有敌人，如果没了就全部退出，并调整模式
    /// </summary>
    public void OnTurnEnd()
    {
    }
}