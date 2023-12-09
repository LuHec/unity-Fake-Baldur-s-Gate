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
    private CommandCenter commandCenter;
    private ActorsManagerCenter actorsManagerCenter;
    private PathFinding pathFinding;
    private MessageCenter messageCenter;
    private TurnManager turnManager;
    private MapSystem mapSystem;

    #region #System Functions

    /// <summary>
    /// 初始化系统
    /// </summary>
    private void Start()
    {
        turnManager = TurnManager.Instance;
        InitMap();
        InitSystem();
        
        // _turnManager.AddTurn(_actorsManagerCenter.GetAllConActorsDynamicId());
        turnManager.InitActorContainer(actorsManagerCenter.LoadPlayerActor());
    }


    private void Update()
    {
        if (MessageCenter.Instance.globalState.EditMode == false)
        {
            turnManager.RunTurn();

            if (PlayerInput.Instance.IsRClick)
            {
                TurnManager.Instance.AddFreeModeActorById(actorsManagerCenter.LoadActorTest(PlayerInput.Instance.GetMouse3DPosition(LayerMask.GetMask("Default"))));
            }
        }
    }

    #endregion


    #region #Init Functions

    void InitSystem()
    {
        actorsManagerCenter = ActorsManagerCenter.Instance;
        commandCenter = CommandCenter.Instance;
        messageCenter = MessageCenter.Instance;
        
        actorsManagerCenter.Init();
        turnManager.Init();
        messageCenter.Init();
    }

    void InitMap()
    {
        mapSystem = MapSystem.Instance;
        pathFinding = PathFinding.Instance;

        pathFinding.Init(mapSystem.GetGrid());
    }

    #endregion

    #region #Custom Functions

    #endregion
}