using System;
using System.Collections;
using System.Collections.Generic;
using LuHec.Utils;
using UnityEngine;

/// <summary>
///  顶层系统
/// </summary>
public class TopSystem : MonoBehaviour
{
    private CommandCenter _commandCenter;
    private ActorsManagerCenter _actorsManagerCenter;
    private PathFinding _pathFinding;
    private MessageCenter _messageCenter;
    private TurnManager _turnManager;
    private MapSystem _mapSystem;

    #region #System Functions

    /// <summary>
    /// 初始化系统
    /// </summary>
    private void Start()
    {
        InitMap();
        InitSystem();

        _turnManager = new TurnManager(_commandCenter);
        _turnManager.AddTurn(_actorsManagerCenter.GetPlayerControlledActorsList(),
            _actorsManagerCenter.GetSystemControlledActorList());
    }


    private void Update()
    {
        _turnManager.RunTurn();
    }

    #endregion


    #region #Init Functions

    void InitSystem()
    {
        _commandCenter = new CommandCenter();
        _actorsManagerCenter = new ActorsManagerCenter();
        _messageCenter = MessageCenter.Instance;
    }

    void InitMap()
    {
        _mapSystem = MapSystem.Instance;
        _pathFinding = PathFinding.Instance;
        
        _pathFinding.Init(_mapSystem.GetGrid());
    }

    #endregion

    #region #Custom Functions

    #endregion
}