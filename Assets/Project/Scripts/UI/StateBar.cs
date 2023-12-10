using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateBar : MonoBehaviour
{
    [SerializeField] float fillSpeed = 0.1f;
    [SerializeField] Image fillImageFront;
    [SerializeField] Image fillImageBack;
    [SerializeField] float waitForDealyFillTime = 0.5f;
    [SerializeField] bool waitForDealy = true;
    
    private Canvas canvas;
    private RectTransform canvasRect;
    private RectTransform headPos;
    private WaitForSeconds waitForDealyFill;
    private Coroutine coroutine;
    private float t;
    protected float currentFillAmount;
    protected float targetFillAmount;

    void Awake()
    {
        headPos = GetComponentInChildren<RectTransform>();
        canvasRect = GetComponent<RectTransform>();
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        waitForDealyFill = new WaitForSeconds(waitForDealyFillTime);
    }

    void Update()
    {
        // transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        transform.LookAt(Camera.main.transform);
        transform.forward = Vector3.up;
        
        // // 先将3D坐标转换成屏幕坐标
        // Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        //
        // // 再将屏幕坐标转换成UGUI坐标
        // Vector2 localPoint;
        //
        // if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, Camera.main, out localPoint))
        // {
        //     headPos.anchoredPosition = localPoint;
        // }
    }
    
    void OnDisable()
    {
        StopAllCoroutines();
    }
    
    
    public virtual void Initialize(float currentValue, float maxValue)
    {
        currentFillAmount = currentValue / maxValue;
        targetFillAmount = currentFillAmount;
        fillImageBack.fillAmount = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
    }

    public void UpdateStates(float currentValue, float maxValue)
    {
        targetFillAmount = currentValue / maxValue;

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        if (currentFillAmount > targetFillAmount)
        {
            fillImageFront.fillAmount = targetFillAmount;
            coroutine = StartCoroutine(BufferFillingCoroutine(fillImageBack));
        }
        else if (currentFillAmount < targetFillAmount)
        {
            fillImageBack.fillAmount = targetFillAmount;
            coroutine = StartCoroutine(BufferFillingCoroutine(fillImageFront));
        }
    }

    protected virtual IEnumerator BufferFillingCoroutine(Image image)
    {
        if (waitForDealy)
        {
            yield return waitForDealyFill;
        }
        
        t = 0;
        
        while (t < 1f)
        {
            t += Time.deltaTime * fillSpeed;
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, t);
            image.fillAmount = currentFillAmount;
            yield return null;
        }
    }
}
