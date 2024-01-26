using Cysharp.Threading.Tasks;
using EActorTargetConfirmation;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointerIndicator : IndicatorBase
{
    public PointerIndicator(GameActor owner) : base(owner)
    {
    }

    // 指向型只能使用Instant,手动触发targetReady
    public override async UniTask WaitTargetData(AbilityBase ability, EActorTargetConfirmation.Type confirmType)
    {
        if (confirmType == Type.USER_CONFIRMED)
        {
            PlayerInput.Instance.PlayInputAction.Click.started += GetPlayerMouseWorldPositionAndConfirm;
            PlayerInput.Instance.PlayInputAction.RightClick.started += ctx => { CancelTargeting(); };
        }

        await Tick();
    }

    protected override void ConfirmTargetAndContinue()
    {
        // 用直接得到的列表
    }

    protected override async UniTask Tick()
    {
        while (!bTargetReady)
        {
            await UniTask.Yield();
        }
    }

    protected override void GetPlayerMouseWorldPositionAndConfirm(InputAction.CallbackContext context)
    {
        Vector3 worldPosition;
        PlayerInput playerInput = PlayerInput.Instance;
        Ray ray = playerInput.MainCamera.ScreenPointToRay(playerInput.MousePos);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, LayerMask.GetMask("Actor")))
        {
            GameActor target = raycastHit.transform.GetComponent<GameActor>();
            if (AddTarget(target))
            {
                ConfirmTargetAndContinue();
            }
        }
    }
}