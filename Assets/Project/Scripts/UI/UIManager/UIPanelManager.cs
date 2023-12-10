using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIPanelManager : Singleton<UIPanelManager>
{
    [SerializeField] private GameObject canvas; 
    
    public RectTransform CanvasRectTransform => canvasRectTransform;
    public CanvasScaler CanScaler => canvasScaler;
    private RectTransform canvasRectTransform;
    private CanvasScaler canvasScaler;

    private Dictionary<string, UIPanelBase> panelDict;

    protected override void Init()
    {
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        canvasScaler = canvas.GetComponent<CanvasScaler>();
        panelDict = new Dictionary<string, UIPanelBase>();
    }

    public T ShowPanel<T>() where T : UIPanelBase
    {
        string panelName = typeof(T).Name;

        if (!panelDict.ContainsKey(panelName))
        {
            // 获取预制体并设置父级为canvas
            GameObject panel = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName));
            panel.transform.SetParent(canvasRectTransform, false);
            panelDict[panelName] = panel.GetComponent<T>();
        }

        panelDict[panelName].ShowSelf();

        return panelDict[panelName] as T;
    }

    public void HidePanel<T>(bool needDelete = true) where T : UIPanelBase
    {
        string panelName = typeof(T).Name;

        if (panelDict.ContainsKey(panelName))
        {
            panelDict[panelName].HideSelf();
            if (needDelete)
            {
                Destroy(panelDict[panelName].gameObject);
                panelDict.Remove(panelName);
            }
        }
    }

    public T GetPanel<T>() where T : UIPanelBase
    {
        string panelName = typeof(T).Name;

        if (panelDict.ContainsKey(panelName)) return panelDict[panelName] as T;

        return null;
    }
}