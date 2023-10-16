using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class Projectile : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 20f;
    [SerializeField] private GameObject hitVFX;

    /// <summary>
    /// 向目标移动
    /// </summary>
    /// <param name="target">目标</param>
    /// <param name="onAttachTarget">接触到目标后回调，默认不产生回调</param>
    public void StartMove(Vector3 target, Action onAttachTarget = null)
    {
        Vector3 moveDirection = (target - transform.position).normalized;
        transform.forward = moveDirection;
        StartCoroutine(MoveDirectlyCoroutine(target, moveDirection, onAttachTarget));
    }

    IEnumerator MoveDirectlyCoroutine(Vector3 target, Vector3 moveDirection, Action onAttachTarget)
    {
        while (Vector3.SqrMagnitude(transform.position - target) > 0.1f)
        {
            // transform.position = Vector3.MoveTowards(
            //     transform.position, target, moveSpeed);
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

            yield return null;
        }

        if (!ReferenceEquals(null, hitVFX))
        {
            GameObject.Instantiate(hitVFX, target, quaternion.identity);
        }
        
        onAttachTarget?.Invoke();
        Destroy(this.gameObject);
    }
}