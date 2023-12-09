using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    private Transform actorTransform;

    private void Start()
    {
        // 锁定在当前选择的角色上
        MessageCenter.Instance.SubmitPlayerSelectActor((sender, message) =>
            {
                actorTransform = ActorsManagerCenter.Instance
                    .GetActorByDynamicId(TurnManager.Instance.GetCurrentPlayerId()).transform;
            }
        );
        
        actorTransform = ActorsManagerCenter.Instance
            .GetActorByDynamicId(TurnManager.Instance.GetCurrentPlayerId()).transform;
    }

    private void Update()
    {
        LockPosition();
    }

    void LockPosition()
    {
        Vector3 pos = transform.position;
        pos.x = actorTransform.position.x;
        pos.z = actorTransform.position.z;

        transform.position = pos;
    }
}