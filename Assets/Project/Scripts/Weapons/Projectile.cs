using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;

    //移动方向
    [SerializeField] Vector2 moveDirection;

    //对象每次被激活都会调用
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