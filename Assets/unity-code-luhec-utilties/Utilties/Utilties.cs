using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuHec.Utils
{
    public static class Utilties
    {
        public static Vector3 GetMouse3DPosition(string mouseLayerMask)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, LayerMask.GetMask(mouseLayerMask)))
            {
                Debug.DrawLine(ray.origin, ray.direction * 999f, Color.red);
                return raycastHit.point;
            }
            else
            {
                Debug.Log("null");
                return Vector3.zero;
            }
        }
        
        public static Vector3 GetMouse3DPositionNew(string mouseLayerMask, PlayerInput playerInput)
        {
            Ray ray = Camera.main.ScreenPointToRay(playerInput.MousePos);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, LayerMask.GetMask(mouseLayerMask)))
            {
                Debug.DrawLine(ray.origin, ray.direction * 999f, Color.red);
                return raycastHit.point;
            }
            else
            {
                Debug.Log("null");
                return Vector3.zero;
            }
        }
    }
}