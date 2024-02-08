using System;
using UnityEngine;

public class IndicatorInstance : MonoBehaviour
{
    private IndicatorBase indicator;

    public void PrepareForData(IndicatorBase oIndicator)
    {
        this.indicator = oIndicator;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Actor"))
        {
            Debug.Log("collision");
            var actor = collision.transform.GetComponent<GameActor>();
            indicator.AddTarget(actor);
        }
    }
}