using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ActorInteractPanel : UIPanelBase
{
    [SerializeField] private Vector2 offset = new Vector2 { x = 100f, y = 2f };
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private Button attackBtn;
    private RectTransform rectTransform;
    private Camera mainCamera;
    // private PlayerController playerController = new PlayerController();

    // 处理交互
    private GameActor CurrentControlActor =>
        ActorsManagerCenter.Instance.GetActorByDynamicId(TurnManager.Instance.GetCurrentPlayerId());

    private GameActor interactActor;

    protected override void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        
        attackBtn.onClick.AddListener(() =>
        {
            var playerInputHandler = PlayerInputHandler.Instance;
            Debug.Log("BTN");
            UIPanelManager.Instance.HidePanel<ActorInteractPanel>();
            Character character = CurrentControlActor as Character;
            if (character != null &&
                character.abilitySystem.TryActiveAbility(
                    "Ga_Attack",
                    ref playerInputHandler.playerConfirmHandler,
                    ref playerInputHandler.playerCancelHandler,
                    ref playerInputHandler.playerMousePositionHandler))
            {
                Debug.Log("Active Ability");
            }
            else
            {
                Debug.Log("Active Failed");
            }
        });
    }

    /// <summary>
    /// 更新位置以及交互对象
    /// </summary>
    /// <param name="position">显示的位置</param>
    /// <param name="actorId">交互的对象</param>
    public void UpdatePanel(Vector2 position, uint actorId)
    {
        interactActor = ActorsManagerCenter.Instance.GetActorByDynamicId(actorId);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIPanelManager.Instance.CanvasRectTransform, position,
            mainCamera, out Vector2 recPos);
        recPos += offset;
        rectTransform.anchoredPosition = recPos;
    }

    public override void HideSelf()
    {
        base.HideSelf();

        interactActor = null;
    }

    private void AdjustLength(float length)
    {
    }
}