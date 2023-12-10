using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractScript : MonoBehaviour
{
    // 初始化参数
    private Camera mainCamera;
    
    // 交互对象
    private GameActor currentInteractActor;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (PlayerInput.Instance.IsRClick)
        {
            if (IsClickActor())
            {
                UIPanelManager.Instance.ShowPanel<ActorInteractPanel>().UpdatePosition(PlayerInput.Instance.MousePos);
            }
        }

        if (PlayerInput.Instance.IsLClick && !EventSystem.current.IsPointerOverGameObject())
        {
            UIPanelManager.Instance.HidePanel<ActorInteractPanel>();
        }
    }

    public GameActor IsClickActor()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInput.Instance.MousePos);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, LayerMask.GetMask("Actor")))
        {
            return raycastHit.transform.GetComponent<GameActor>();
        }

        return null;
    }
}