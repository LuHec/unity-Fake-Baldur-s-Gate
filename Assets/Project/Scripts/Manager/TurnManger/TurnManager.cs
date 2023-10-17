using System;
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
    private HashSet<uint> globalPlayerControlledSet;
    private HashSet<uint> globalSystemControlledSet;

    private HashSet<TurnInstance> _turnInstancesSet;
    private CommandCenter _commandCenter;
    private MessageCenter _messageCenter;
    private ActorsManagerCenter _actorsManagerCenter;
    private Character currConChara;
    private bool runTurn = true;

    public Character CurrConChara => currConChara;
    public EventHandler onConCharaChanged;
    public int TurnCount => _turnInstancesSet.Count;

    public void Init(CommandCenter commandCenter, ActorsManagerCenter actorsManagerCenter)
    {
        _actorsManagerCenter = actorsManagerCenter;
        _commandCenter = commandCenter;
        _messageCenter = MessageCenter.Instance;
        // _turnInstances = new List<TurnInstance>();
        _turnInstancesSet = new HashSet<TurnInstance>();
        globalPlayerControlledSet = new HashSet<uint>();

        MessageCenter.Instance.SubmitUpdateTurn(OnMessageCenterUpdateTurn);
        MessageCenter.Instance.SubmitActorDie(OnActorDie);
    }


    /// <summary>
    /// 加入新的回合。可能会有被重组的id，因此会进行去重
    /// </summary>
    /// <param name="conActorDynamic_id"></param>
    public void AddTurn(List<uint> conActorDynamic_id)
    {
        if (conActorDynamic_id.Count == 0) return;

        foreach (uint id in conActorDynamic_id)
        {
            if (_actorsManagerCenter.GetActorByDynamicId(id).GetActorType() == ActorEnumType.ActorType.Character)
            {
                if (!globalPlayerControlledSet.Contains(id))
                    globalPlayerControlledSet.Add(id);
            }
            else
            {
                if (!globalSystemControlledSet.Contains(id))
                    globalSystemControlledSet.Add(id);
            }
        }

        _turnInstancesSet.Add(new TurnInstance(_actorsManagerCenter, _commandCenter, conActorDynamic_id));
    }

    public bool AddTurn(TurnInstance turnInstance)
    {
        if (turnInstance == null) return false;

        return _turnInstancesSet.Add(turnInstance);
    }

    public bool RemoveTurn(TurnInstance turnInstance)
    {
        foreach (uint id in turnInstance.ConActorDynamicIDSet)
        {
            _actorsManagerCenter.GetActorByDynamicId(id).InitTurnIntance(null);
        }

        return _turnInstancesSet.Remove(turnInstance);
    }

    public bool RemoveActorByDynamicID(uint id)
    {
        foreach (var turn in _turnInstancesSet)
        {
            if (turn._conActorDynamicIDSet.Contains(id))
            {
                turn.RemoveActorByDynamicId(id);
                return true;
            }
        }

        return false;
    }

    public void RunTurn()
    {
        if (runTurn)
        {
            foreach (TurnInstance turnInstance in _turnInstancesSet)
            {
                turnInstance.RunTurn();

                if (!runTurn)
                {
                    // 更新回合在turnInstance.RunTurn()内部完成，这里强制结束回合运行，直接进入下一帧，重新运行回合实例
                    // 每个回合实例的回合数依然是正常保存的
                    Debug.Log("turn end");
                    runTurn = true;
                    break;
                }
            }
        }
    }

    public void Run3rdMode()
    {
        foreach (var id in globalPlayerControlledSet)
        {
            // if()
        }
    }

    #region #Listener

    public void OnMessageCenterUpdateTurn(Object sender, EventArgsType.UpdateTurnManagerMessage message)
    {
        foreach (var id in message.newTurn._conActorDynamicIDs)
        {
            Debug.Log(id);
        }
        
        // 如果回合不是原来就有的，就需要中断回合
        if (AddTurn(message.newTurn))
            ForceQuitTurn();
        
        // 如果有回合要移除，就要中断回合
        if (message.turnRemoveSet.Count != 0)
        {
            ForceQuitTurn();
            Debug.Log("Remove Turn");
            foreach (var instance in message.turnRemoveSet)
            {
                _turnInstancesSet.Remove(instance);
            }
        }
    }

    private void ForceQuitTurn()
    {
        runTurn = false;
    }

    private void OnActorDie(Object sender, EventArgsType.ActorDieMessage message)
    {
        RemoveActorByDynamicID(message.dead_dynamic_id);
    }

    #endregion
}