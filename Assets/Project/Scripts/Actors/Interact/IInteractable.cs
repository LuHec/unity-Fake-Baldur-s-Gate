using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public interface IInteractable : IPointerEnterHandler, IPointerExitHandler
{
    public void GetInteractInfo();
}