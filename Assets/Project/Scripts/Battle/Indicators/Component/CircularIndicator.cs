using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CircularIndicator : IndicatorBase
{
    public CircularIndicator(AbilityBase ownerAbility) : base(ownerAbility)
    {
    }

    public override void ReadyToActive()
    {
        // 圆形范围
        indicatorInstance = Object.Instantiate(
            Resources.Load<GameObject>("Actors/Indicators/View/View_SphereIndicator"),
            ownerAbility.owner.transform.position, Quaternion.Euler(90, 0, 0));
    }

    protected override void ConfirmTargetAndContinue()
    {
        var hitResult = Physics.OverlapSphere(
            indicatorPosition,
            1.5f,
            LayerMask.GetMask("Actor"));
        

        foreach (var result in hitResult)
        {
            var actor = result.GetComponent<GameActor>();
            if (actor != null)
            {
                Debug.Log(actor.DynamicId);
                AddTarget(actor);
            }
        }

        if (targetsList.Count > 0)
        {
            targetDataReadyHandler?.Invoke(
                this,
                new TargetData(
                    true,
                    new List<GameActor>(targetsList)));

            targetsList.Clear();
            UnbindInput();

            Object.Destroy(indicatorInstance);
        }
    }
}