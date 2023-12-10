using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MouseInfoPanel : UIPanelBase
{
    [SerializeField] private Vector2 offset = new Vector2 { x = 20f, y = 50f };
    [SerializeField] private TMP_Text infoText;

    private RectTransform rectTransform;
    private Camera mainCamera;

    protected override void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    public void UpdateInfoPanel(string text, Vector3 position)
    {
        var screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIPanelManager.Instance.CanvasRectTransform,
            screenPoint, Camera.main, out Vector2 rectPoint);
        rectPoint += offset;
        rectTransform.anchoredPosition = rectPoint;

        infoText.text = text;
    }
}