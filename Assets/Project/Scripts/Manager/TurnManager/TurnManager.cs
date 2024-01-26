using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using Object = System.Object;

/// <summary>
/// 回合管理器
/// </summary>
public class TurnManager : Singleton<TurnManager>
{
    [Tooltip("参数")] public float WaitTime = 0.5f;

    // ------------------------------------------------------------------------------
    // Containers
    private HashSet<uint> globalFreeModeActorIdSet = new HashSet<uint>();
    private HashSet<TurnInstance> turnInstancesSet = new HashSet<TurnInstance>();

    private ActorsManagerCenter actorsManagerCenter;
    private PlayerActorContainer playerActorContainer;

    // ------------------------------------------------------------------------------
    // Update Info
    private List<TurnInstance> turnsNeedRemove = new List<TurnInstance>();

    private List<TurnInstance> turnsNeedAdd = new List<TurnInstance>();

    public EventHandler<int> onConCharaChanged;
    public int TurnCount => turnInstancesSet.Count;

    public void Init()
    {
        actorsManagerCenter = ActorsManagerCenter.Instance;

        MessageCenter.Instance.SubmitUpdateTurn(OnMessageCenterUpdateTurn);
        MessageCenter.Instance.SubmitActorDie(OnActorDie);
    }

    public void InitActorContainer(List<uint> playerList)
    {
        playerActorContainer = new PlayerActorContainer(playerList);
        foreach (var id in playerList)
        {
            globalFreeModeActorIdSet.Add(id);
        }
    }

    public void SelectPlayer(int playerIdx)
    {
        playerActorContainer.ChangePlayerByIdx(playerIdx);
    }

    public uint GetCurrentPlayerId()
    {
        return playerActorContainer.GetCurrentPlayer;
    }

    public List<uint> GetAllPlayersIdList()
    {
        return playerActorContainer.PlayerActorsIdList;
    }

    /// <summary>
    /// 添加回合会将id从自由列表移除，并初始化角色的回合
    /// </summary>
    /// <param name="turnInstance"></param>
    /// <returns></returns>
    public void AddTurn(TurnInstance turnInstance)
    {
        foreach (var turnItem in turnInstance.turnItemsLists)
        {
            if (turnItem.BIsCharacter)
            {
                globalFreeModeActorIdSet.Remove(turnItem.character.DynamicId);
                ActorsManagerCenter.Instance.GetActorByDynamicId(turnItem.character.DynamicId)
                    .SetTurnIntance(turnInstance);
            }
        }

        turnInstancesSet.Add(turnInstance);
    }

    public void RemoveTurn(TurnInstance turnInstance)
    {
        foreach (var turnItem in turnInstance.turnItemsLists)
        {
            if (turnItem.BIsCharacter)
            {
                actorsManagerCenter.GetActorByDynamicId(turnItem.character.DynamicId).SetTurnIntance(null);
                globalFreeModeActorIdSet.Add(turnItem.character.DynamicId);
            }
        }

        turnInstancesSet.Remove(turnInstance);
    }

    public bool RemoveActorByDynamicID(uint id, bool isDead = false)
    {
        foreach (var turn in turnInstancesSet)
        {
            if (turn.Contain(id))
            {
                turn.RemoveActorByDynamicId(id);
                if (isDead) ActorsManagerCenter.Instance.RemoveConActorByDynamicId(id);
                return true;
            }
        }

        return false;
    }

    public void RunTurn()
    {
        RunFreeMode();
        RunTurnMode();

        HandlerTurnsInTheEnd();
    }

    public bool AddFreeModeActorById(uint id)
    {
        return globalFreeModeActorIdSet.Add(id);
    }

    public bool RemoveFreeModeActorById(uint id)
    {
        return globalFreeModeActorIdSet.Remove(id);
    }

    private void RunFreeMode()
    {
        foreach (var id in globalFreeModeActorIdSet)
        {
            var actor = ActorsManagerCenter.Instance.GetActorByDynamicId(id);
            actor.ActorUpdate();
        }
    }

    private void RunTurnMode()
    {
        foreach (TurnInstance turnInstance in turnInstancesSet)
        {
            turnInstance.UpdateTurn();
        }
    }

    private void HandlerTurnsInTheEnd()
    {
        foreach (var turn in turnsNeedRemove)
        {
            RemoveTurn(turn);
        }

        foreach (var turn in turnsNeedAdd)
        {
            AddTurn(turn);
        }

        turnsNeedRemove.Clear();
        turnsNeedAdd.Clear();
    }

    #region #Listener

    /// <summary>
    /// 为了处理加入和移出自由列表的问题，统一设定为退出时清除回合，加入到自由列表
    /// 加入回合时退出自由列表，初始化回合
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    public void OnMessageCenterUpdateTurn(Object sender, EventArgsType.UpdateTurnManagerMessage message)
    {
        if (message.turnRemoveSet != null && message.turnRemoveSet.Count != 0)
        {
            Debug.Log("Remove Turn");

            foreach (var turnNeedRemove in message.turnRemoveSet)
            {
                turnsNeedRemove.Add(turnNeedRemove);
            }
        }

        turnsNeedAdd.Add(message.newTurn);
    }

    private void OnActorDie(Object sender, EventArgsType.ActorDieMessage message)
    {
        RemoveActorByDynamicID(message.dead_dynamic_id, true);
    }

    #endregion
}