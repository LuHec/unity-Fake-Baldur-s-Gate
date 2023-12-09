using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;


public class MessageCenter : Singleton<MessageCenter>
{
    [SerializeField] private float maxEnemySearchDistance = 20f;
    public GlobalState globalState;
    private ActorsManagerCenter actorsManagerCenter;

    private void Start()
    {
        globalState = new GlobalState();
    }

    public void Init()
    {
        actorsManagerCenter = ActorsManagerCenter.Instance;
    }

    #region #delegate

    public event EventHandler<EventArgsType.UpdateTurnManagerMessage> TurnUpdateHandler;
    public event EventHandler<EventArgsType.ActorDieMessage> ActorDieHandler;
    public event EventHandler<EventArgsType.PlayerBackTurnMessage> BackTurnHandler;
    public event EventHandler<EventArgsType.PlayerSelectMessage> PlayerSelectHandler; 

    #endregion

    #region #Listener

    public void ListenOnActorDied(ref EventHandler<EventArgsType.ActorDieMessage> handler)
    {
        handler += OnActorDied;
    }

    public void ListenOnActorAttacking(ref EventHandler<EventArgsType.ActorAttackingMessage> handler)
    {
        handler += OnActorAttacked;
    }

    public void ListenOnTurnNeedRemove(ref EventHandler<EventArgsType.TurnNeedRemoveMessage> handler)
    {
        handler += OnTurnNeedRemove;
    }

    public void ListenOnGameModeSwitch(ref EventHandler<EventArgsType.GameModeSwitchMessage> handler)
    {
        handler += OnGameModeSwitch;
    }

    public void ListenOnPlayerSelect(ref EventHandler<EventArgsType.PlayerSelectMessage> handler)
    {
        handler += OnPlayerSelect;
    }

    public void ListenOnPlayerBackTurn(ref EventHandler<EventArgsType.PlayerBackTurnMessage> handler)
    {
        handler += OnPlayerBackTurn;
    }
    
    private void OnActorDied(object sender, EventArgsType.ActorDieMessage message)
    {
        Debug.Log("Id " + "Died");
        //     
        ActorDieHandler?.Invoke(this, message);

        // 从地图上清理掉
        var gridObject = MapSystem.Instance.GetGridObject(ActorsManagerCenter.Instance
            .GetActorByDynamicId(message.dead_dynamic_id).transform.position);
        gridObject.ClearActor();
    }


    /// <summary>
    /// 暂时只播报玩家控制角色
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    private void OnActorAttacked(object sender, EventArgsType.ActorAttackingMessage message)
    {
        Debug.Log("attacker: " + message.attacker_dynamic_id + " attacked :" + message.attacked_dynamic_id);
        AddNewTurnByAttack(message.attacker_dynamic_id, message.attacked_dynamic_id);
    }

    private void OnTurnNeedRemove(object sender, EventArgsType.TurnNeedRemoveMessage message)
    {
        TurnInstance turnInstance = message.turnInstance;
        var removeSet = new HashSet<TurnInstance>();

        removeSet.Add(turnInstance);
        UpdateTurn(new EventArgsType.UpdateTurnManagerMessage(null, removeSet));
    }

    /// <summary>
    /// 由ui端调用，切换游戏模式
    /// </summary>
    private void OnGameModeSwitch(object sender, EventArgsType.GameModeSwitchMessage message)
    {
        var currentPlayerTurn = ActorsManagerCenter.Instance
            .GetActorByDynamicId(TurnManager.Instance.GetCurrentPlayerId())
            .CurrentTurn;

        // 检查切换类型
        if (message.gameMode == EventArgsType.GameModeSwitchMessage.GameMode.Turn)
        {
            // 检查是否已经在回合内
            if (currentPlayerTurn != null)
            {
                Debug.Log("Already In turn!");
                return;
            }

            // 获取玩家列表
            var playerActorList = TurnManager.Instance.GetAllPlayerIdList();

            // 通过就进行添加到回合内
            currentPlayerTurn = new TurnInstance(playerActorList, true);
            UpdateTurn(new EventArgsType.UpdateTurnManagerMessage(currentPlayerTurn, null));


            Debug.Log("Switch");
        }
        else
        {
            var removeSet = new HashSet<TurnInstance>();

            // 找到角色所在回合
            removeSet.Add(currentPlayerTurn);

            UpdateTurn(new EventArgsType.UpdateTurnManagerMessage(null, removeSet));

            Debug.Log("Free from turn!");
        }
    }

    public void OnPlayerSelect(object sender, EventArgsType.PlayerSelectMessage message)
    {
        TurnManager.Instance.SelectPlayer(message.PlayerIdx);
        PlayerSelectHandler(sender, message);
    }

    public void OnPlayerBackTurn(object sender, EventArgsType.PlayerBackTurnMessage message)
    {
        // 找到当前操控角色所在回合，back ptr，undo并弹出所有状态
        Character player = ActorsManagerCenter.Instance.GetActorByDynamicId(TurnManager.Instance.GetCurrentPlayerId()) as Character;

        var turnInstance = player.CurrentTurn;
        if (turnInstance != null)
        {
            for (int i = 0; i < message.backCount; i++)
            {
                turnInstance.BackTurn();
            }
        }
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

    public void SubmitPlayerSelectActor(EventHandler<EventArgsType.PlayerSelectMessage> listener)
    {
        PlayerSelectHandler += listener;
    }

    #endregion

    #region #HelpFunctions

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
                removeSet.Add(actorsManagerCenter.GetActorByDynamicId(id).CurrentTurn);
                newTurn.AddActorByDynamicId(id);
            }
        }
    }

    private List<uint> FindAllActorIdInDistance(uint centerId, float distance)
    {
        List<uint> resList = new List<uint>();

        // 从直接ActorManager的列表里找，效率更高
        var idList = actorsManagerCenter.GetAllConActorsDynamicId();

        GameActor centerActor = actorsManagerCenter.GetActorByDynamicId(centerId);

        // 如果满足距离条件则加入List
        foreach (uint id in idList)
        {
            if (id != centerId)
            {
                if (Vector3.Distance(centerActor.transform.position,
                        actorsManagerCenter.GetActorByDynamicId(id).transform.position) <= distance)
                {
                    resList.Add(id);
                }
            }
        }

        return resList;
    }

    private void AddNewTurnByAttack(uint attackerId, uint attackedId)
    {
        GameActor attacker = actorsManagerCenter.GetActorByDynamicId(attackerId);

        // 非玩家角色不进行播报
        if (actorsManagerCenter.GetActorByDynamicId(attackerId).GetActorStateTag() !=
            ActorEnumType.ActorStateTag.Player) return;

        // 受到攻击的对象可能不在侦测范围内
        GameActor beAttacked = actorsManagerCenter.GetActorByDynamicId(attackedId);

        // 更新Instance
        TurnInstance newTurn = attacker.CurrentTurn;

        // 去重
        HashSet<uint> idSet = new HashSet<uint>();
        List<uint> distIdList = FindAllActorIdInDistance(attackerId, maxEnemySearchDistance);

        // 需要去除的回合
        HashSet<TurnInstance> removeSet = new HashSet<TurnInstance>();

        // 如果攻击者不在回合内，就创建回合
        if (newTurn == null)
        {
            newTurn = new TurnInstance();
            newTurn.AddActorByDynamicId(attackerId);
        }

        // 不管怎样先把回合类型设置为非手动回合模式
        newTurn.SetGameTurnMode(false);
        
        // 把攻击方加入到集合中
        idSet.Add(attackerId);

        // 被攻击对象可能已经死亡
        if (beAttacked)
        {
            if (beAttacked.CurrentTurn == null)
            {
                newTurn.AddActorByDynamicId(beAttacked.DynamicId);
                idSet.Add(beAttacked.DynamicId);
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
            GameActor distActor = actorsManagerCenter.GetActorByDynamicId(id);

            // 如果不在回合内则直接加入
            if (distActor.CurrentTurn == null)
            {
                newTurn.AddActorByDynamicId(distActor.DynamicId);
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

        // Debug.Log("TurnCount: " + TurnManager.Instance.TurnCount + "NewTurn: " + newTurn.ConActorDynamicIDs.Count);

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

    #endregion
}