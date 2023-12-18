using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ActorInteractPanel : UIPanelBase
{
    [SerializeField] private Vector2 offset = new Vector2 { x = 100f, y = 2f };
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    private RectTransform rectTransform;
    private Camera mainCamera;

    [SerializeField] private Button attackBtn;

    // 处理交互
    private GameActor CurrentControlActor =>
        ActorsManagerCenter.Instance.GetActorByDynamicId(TurnManager.Instance.GetCurrentPlayerId());

    private GameActor interactActor;

    public bool CanGenerateCommand()
    {
        if (CurrentControlActor == null) return false;

        if (CurrentControlActor.CurrentTurn == null) return true;

        // 当前控制的角色处在的回合轮到玩家输入，且当前命令为空，或者有命令时已经执行完毕
        if (CurrentControlActor.CurrentTurn.currentActorId == CurrentControlActor.DynamicId &&
            CurrentControlActor.GetCommand() == null)
            return true;

        return false;
    }

    protected override void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;

        
        attackBtn.onClick.AddListener(() =>
        {
            if (interactActor != null && CanGenerateCommand())
            {
                // CurrentControlActor.AddCommand(CommandCenter.Instance.GetAttackActorCommand(interactActor));
                CurrentControlActor.AddCommand(CommandCenter.Instance.GetAbilityCommand(CurrentControlActor, nameof(Ga_Attack)));
            }
            
            UIPanelManager.Instance.HidePanel<ActorInteractPanel>();
        });
    }

    /// <summary>
    /// 更新位置以及交互对象
    /// </summary>
    /// <param name="position">显示的位置</param>
    /// <param name="actor">交互的对象</param>
    public void UpdatePanel(Vector2 position, uint actorId)
    {
        interactActor = ActorsManagerCenter.Instance.GetActorByDynamicId(actorId);
        Debug.Log(interactActor == null);

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