using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    TrailRenderer trail;
    
    [SerializeField] float moveSpeed = 10f;

    //移动方向
    [SerializeField] Vector2 moveDirection;

    void Awake()
    {
        trail = GetComponentInChildren<TrailRenderer>();
    }
  
    void OnEnable()
    {
        StartCoroutine(nameof(MoveDirectly));
    }

    IEnumerator MoveDirectly()
    {
        while (gameObject.activeSelf)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

            yield return null;
        }
    }
}