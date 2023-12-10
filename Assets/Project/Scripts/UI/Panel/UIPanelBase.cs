using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPanelBase : MonoBehaviour
{
    protected CanvasGroup canvasGroup;

    protected void Awake()
    {
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
        Init();
    }

    protected abstract void Init();
    
    public virtual void HideSelf()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public virtual void ShowSelf()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
