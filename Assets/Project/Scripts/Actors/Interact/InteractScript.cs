using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractScript : MonoBehaviour
{
    #region Params

    // 初始化参数
    private Camera mainCamera;
    private PlayerInput playerInput;

    // 交互对象
    public GameActor CurrentControlActor =>
        ActorsManagerCenter.Instance.GetActorByDynamicId(TurnManager.Instance.GetCurrentPlayerId());

    private GameActor currentInteractActor;

    // 实时参数
    public bool CanGenerateCommand()
    {
        if (CurrentControlActor == null) return false;

        // 暂时禁用了自由模式打断，有bug
        if (CurrentControlActor.CurrentTurn == null)
        {
            if (CurrentControlActor.GetCommand() == null)
                return true;
            return false;
        }


        // 当前控制的角色处在的回合轮到玩家输入，且当前命令为空，或者有命令时已经执行完毕
        if (CurrentControlActor.CurrentTurn.currentActorId == CurrentControlActor.DynamicId &&
            CurrentControlActor.CurrentTurn.CurrentTurnState == TurnInstance.TurnState.TURN_WAIT_COMMAND)
            return true;

        return false;
    }

    public GameActor IsOnActor()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInput.Instance.MousePos);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, LayerMask.GetMask("Actor")))
        {
            return raycastHit.transform.GetComponent<GameActor>();
        }

        return null;
    }

    #endregion

    #region GameLogic

    private void Start()
    {
        mainCamera = Camera.main;
        playerInput = PlayerInput.Instance;
    }

    /// <summary>
    /// 处理所有输入，只有当前控制角色处在自由模式，或者是当前回合执行对象时，才会产生新的命令
    /// </summary>
    private void Update()
    {
        if (PlayerInput.Instance.IsLClick && !EventSystem.current.IsPointerOverGameObject())
        {
            // 左键时，需要关闭交互窗口
            UIPanelManager.Instance.HidePanel<ActorInteractPanel>();

            // 执行左键命令
            if (CanGenerateCommand())
            {
                GameActor actor = IsOnActor();
                if (actor != null && actor != CurrentControlActor)
                {
                    AddCommand(CommandCenter.Instance.GetAttackActorCommand(actor));
                }
                else
                {
                    Vector3 position = playerInput.GetMouse3DPosition(LayerMask.GetMask("Default"));
                    AddCommand(CommandCenter.Instance.GetMoveActorCommand(CurrentControlActor, position));
                }
            }
        }

        if (PlayerInput.Instance.IsRClick)
        {
            GameActor actor = IsOnActor();
            if (actor != null && CanGenerateCommand())
            {
                UIPanelManager.Instance.ShowPanel<ActorInteractPanel>()
                    .UpdatePanel(PlayerInput.Instance.MousePos, actor.DynamicId);
            }
        }

        if (PlayerInput.Instance.IsSpace)
        {
            Character character = CurrentControlActor as Character;
            if (CanGenerateCommand() && character.abilitySystem.CanActiveAbility("Ga_Heal"))
            {
                CurrentControlActor.AddCommand(new AbilityCommand(CurrentControlActor, "Ga_Heal"));
            }
        }
    }

    #endregion

    #region Command

    private void AddCommand(CommandInstance command)
    {
        CurrentControlActor.AddCommand(command);
    }

    #endregion
}