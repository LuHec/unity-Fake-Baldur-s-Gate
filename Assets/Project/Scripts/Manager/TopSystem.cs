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
        InitMap();
        InitSystem();
        _turnManager = TurnManager.Instance;
        _turnManager.Init(_commandCenter, _actorsManagerCenter);
        _messageCenter.Init(_actorsManagerCenter);

        _turnManager.AddTurn(_actorsManagerCenter.GetAllConActorsDynamicId());
    }


    private void Update()
    {
        if (MessageCenter.Instance.globalState.EditMode == false)
        {
            _turnManager.RunTurn();

            if (PlayerInput.Instance.IsRClick)
            {
                // test
                List<uint> ids = new List<uint>();
                ids.Add(_actorsManagerCenter.LoadActorTest(new Vector3(5, 1, 5)));
                // ids.Add(_actorsManagerCenter.LoadActorTest(new Vector3(6, 1, 6)));

                TurnManager.Instance.AddTurn(ids);
            }
        }
    }

    #endregion


    #region #Init Functions

    void InitSystem()
    {
        _actorsManagerCenter = new ActorsManagerCenter();
        _commandCenter = new CommandCenter(_actorsManagerCenter);
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