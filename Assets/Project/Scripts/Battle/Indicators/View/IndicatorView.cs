using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class IndicatorView
{
    public enum IndicatorType
    {
        POINTER = 0,
        CIRCULAR,
        RECT,
        COUNT
    }

    private GameObject[] indicatorViews = new GameObject[(int)IndicatorType.COUNT];

    private IndicatorType currentIndicator = IndicatorType.POINTER;

    public IndicatorView()
    {
        indicatorViews[(int)IndicatorType.CIRCULAR] = Object.Instantiate(
            Resources.Load<GameObject>("Actors/Indicators/View/View_SphereIndicator"), Vector3.zero,
            Quaternion.Euler(90, 0, 0));
        indicatorViews[(int)IndicatorType.CIRCULAR].SetActive(false);
    }

    public void MoveIndicator(Vector3 position)
    {
        indicatorViews[(int)currentIndicator].transform.position = position;
    }

    public void ShowIndicator(IndicatorType indicatorType)
    {
        HideIndicator();
        currentIndicator = indicatorType;
        indicatorViews[(int)currentIndicator].SetActive(true);
    }


    public void HideIndicator()
    {
        indicatorViews[(int)currentIndicator].SetActive(false);
    }
}