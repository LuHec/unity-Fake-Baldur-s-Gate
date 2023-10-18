using System;
using System.Collections;
using System.Collections.Generic;
using LuHec.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

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
        _turnManager = TurnManager.Instance;
        InitMap();
        InitSystem();
        
        // _turnManager.AddTurn(_actorsManagerCenter.GetAllConActorsDynamicId());
        _turnManager.InitActorContainer(_actorsManagerCenter.LoadPlayerActor());
    }


    private void Update()
    {
        if (MessageCenter.Instance.globalState.EditMode == false)
        {
            _turnManager.RunTurn();

            if (PlayerInput.Instance.IsRClick)
            {
                TurnManager.Instance.AddFreeModeActorById(_actorsManagerCenter.LoadActorTest(PlayerInput.Instance.GetMouse3DPosition(LayerMask.GetMask("Default"))));
            }
        }
    }

    #endregion


    #region #Init Functions

    void InitSystem()
    {
        _actorsManagerCenter = ActorsManagerCenter.Instance;
        _commandCenter = CommandCenter.Instance;
        _messageCenter = MessageCenter.Instance;
        
        _actorsManagerCenter.Init();
        _turnManager.Init();
        _messageCenter.Init();
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