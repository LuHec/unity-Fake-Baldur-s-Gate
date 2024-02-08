using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Test : MonoBehaviour
{
    public float radius = 3f;

    private void Start()
    {
        Instantiate(
            Resources.Load<GameObject>("Actors/Indicators/View/View_SphereIndicator"),
            transform.position,
            Quaternion.Euler(90, 0, 0));
    }


    void OnDrawGizmos()
    {
        // 在Scene视图中绘制球体范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);  
    }
}