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
    private HashSet<uint> globalFreeModeActorIdSet;

    private HashSet<TurnInstance> _turnInstancesSet;
    private CommandCenter _commandCenter;
    public ActorsManagerCenter _actorsManagerCenter;
    private Character currConChara;
    private bool runTurn = true;
    private PlayerActorContainer _playerActorContainer;

    public Character CurrConChara => currConChara;
    public EventHandler onConCharaChanged;
    public int TurnCount => _turnInstancesSet.Count;

    public void Init()
    {
        _actorsManagerCenter = ActorsManagerCenter.Instance;
        _commandCenter = CommandCenter.Instance;
        _turnInstancesSet = new HashSet<TurnInstance>();
        globalPlayerControlledSet = new HashSet<uint>();
        globalFreeModeActorIdSet = new HashSet<uint>();

        MessageCenter.Instance.SubmitUpdateTurn(OnMessageCenterUpdateTurn);
        MessageCenter.Instance.SubmitActorDie(OnActorDie);
    }

    public void InitActorContainer(List<uint> playerList)
    {
        _playerActorContainer = new PlayerActorContainer(playerList);
        foreach (var id in playerList)
        {
            globalFreeModeActorIdSet.Add(id);
        }
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
            globalFreeModeActorIdSet.Remove(id);
        }
        _turnInstancesSet.Add(new TurnInstance(_actorsManagerCenter, _commandCenter, conActorDynamic_id));
    }

    public bool QueryFreeModeByActorId(uint id)
    {
        return globalFreeModeActorIdSet.Contains(id);
    }

    public uint GetCurrentPlayerId()
    {
        return _playerActorContainer.GetCurrentPlayer;
    }

    /// <summary>
    /// 添加回合会将id从自由列表移除，并初始化角色的回合
    /// </summary>
    /// <param name="turnInstance"></param>
    /// <returns></returns>
    public bool AddTurn(TurnInstance turnInstance)
    {
        if (turnInstance == null) return false;
        foreach (var id in turnInstance.ConActorDynamicIDs)
        {
            globalFreeModeActorIdSet.Remove(id);
            ActorsManagerCenter.Instance.GetActorByDynamicId(id).InitTurnIntance(turnInstance);
        }

        return _turnInstancesSet.Add(turnInstance);
    }

    public bool RemoveTurn(TurnInstance turnInstance)
    {
        foreach (uint id in turnInstance.ConActorDynamicIDSet)
        {
            _actorsManagerCenter.GetActorByDynamicId(id).InitTurnIntance(null);
            globalFreeModeActorIdSet.Add(id);
        }

        return _turnInstancesSet.Remove(turnInstance);
    }

    public bool RemoveActorByDynamicID(uint id, bool isDead = false)
    {
        foreach (var turn in _turnInstancesSet)
        {
            if (turn._conActorDynamicIDSet.Contains(id))
            {
                turn.RemoveActorByDynamicId(id);
                return true;
            }
        }

        if (!isDead)
        {
            globalFreeModeActorIdSet.Add(id);
        }

        return false;
    }

    public void RunTurn()
    {
        // 运行自由模式的actor
        RunFreeMode();

        // 运行回合制模式actor
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
        Debug.Log(globalFreeModeActorIdSet.Count);
        foreach (var id in globalFreeModeActorIdSet)
        {
            var character = ActorsManagerCenter.Instance.GetActorByDynamicId(id) as Character;
            CommandCenter.Instance.AddCommand(character.GenCommandInstance(), character);
            _commandCenter.Excute(character.GenCommandInstance(), character, () => { character.ClearCommandCache(); });

            // 可能会被合并回合打断
            if (runTurn == false) break;
        }
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
        // 如果有回合要移除，就要中断回合
        if (message.turnRemoveSet.Count != 0)
        {
            ForceQuitTurn();
            Debug.Log("Remove Turn");
            foreach (var instance in message.turnRemoveSet)
            {
                RemoveTurn(instance);
                _turnInstancesSet.Remove(instance);
            }
        }

        // 如果回合不是原来就有的，就需要中断回合
        if (AddTurn(message.newTurn))
            ForceQuitTurn();
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