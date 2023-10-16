using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;

/// <summary>
/// 回合管理器
/// </summary>
public class TurnManager : Singleton<TurnManager>
{
    private HashSet<uint> globalPlayerControlledSet;
    private HashSet<uint> globalSystemControlledSet;

    private List<TurnInstance> _turnInstances;
    private CommandCenter _commandCenter;
    private MessageCenter _messageCenter;
    private ActorsManagerCenter _actorsManagerCenter;
    private Character currConChara;
    public Character CurrConChara => currConChara;
    public EventHandler onConCharaChanged;
    public List<TurnInstance> turnInstances => _turnInstances; 

    public void Init(CommandCenter commandCenter, ActorsManagerCenter actorsManagerCenter)
    {
        _actorsManagerCenter = actorsManagerCenter;
        _commandCenter = commandCenter;
        _messageCenter = MessageCenter.Instance;
        _turnInstances = new List<TurnInstance>();
        globalPlayerControlledSet = new HashSet<uint>();
    }


    /// <summary>
    /// 加入新的回合。可能会有被重组的id，因此会进行去重
    /// </summary>
    /// <param name="conActorDynamic_id"></param>
    public void AddTurn(List<uint> conActorDynamic_id)
    {
        foreach (uint id in conActorDynamic_id)
        {
            if (_actorsManagerCenter.GetActorByDynamicId(id).GetActorType() == ActorEnumType.ActorType.Character)
            {
                if(!globalPlayerControlledSet.Contains(id))
                    globalPlayerControlledSet.Add(id);
            }
            else
            {
                if(!globalSystemControlledSet.Contains(id))
                    globalSystemControlledSet.Add(id);
            }
        }

        _turnInstances.Add(new TurnInstance(_actorsManagerCenter, _commandCenter, conActorDynamic_id));
    }

    public void RemoveTurn(int idx)
    {
        _turnInstances.RemoveAt(idx);
    }

    public void RunTurn()
    {
        foreach (TurnInstance turnInstance in _turnInstances)
        {
            turnInstance.RunTurn();
        }
    }

    public void Run3rdMode()
    {
        foreach (var id in globalPlayerControlledSet)
        {
            // if()
        }
    }
}