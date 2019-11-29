using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent OnDoubleClickEvent;
    public UnityEvent OnSingleClickEvent;
    public UnityEvent OnMultiClickEvent;
    public void OnPointerClick(PointerEventData eventData)
    {
        int clickCount = eventData.clickCount;

        if (clickCount == 1)
            OnSingleClick();
        else if (clickCount == 2)
            OnDoubleClick();
        else if (clickCount > 2)
            OnMultiClick();
    }

    void OnSingleClick()
    {
        Debug.Log("Single Clicked");
        OnSingleClickEvent.Invoke();
    }

    void OnDoubleClick()
    {
        Debug.Log("Double Clicked");
        OnDoubleClickEvent.Invoke();
    }

    void OnMultiClick()
    {
        Debug.Log("MultiClick Clicked");
        OnMultiClickEvent.Invoke();
    }
}
