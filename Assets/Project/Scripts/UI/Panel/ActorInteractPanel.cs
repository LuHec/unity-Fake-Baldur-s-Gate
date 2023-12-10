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

    protected override void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    public void UpdatePosition(Vector2 position)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIPanelManager.Instance.CanvasRectTransform, position,
            mainCamera, out Vector2 recPos);
        recPos += offset;
        rectTransform.anchoredPosition = recPos;
    }


    private void AdjustLength(float length)
    {
    }
}