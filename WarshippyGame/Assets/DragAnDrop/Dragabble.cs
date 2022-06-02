using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using DragAnDrop;

public interface IDraggable
{
    public void OnDragEnd();
}

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    [HideInInspector]
    public Transform parentToReturnTo = null;
    [HideInInspector]
    public Transform placeHolderParent = null;

    private GameObject placeHolder = null;

    public int boatIndex;
    public void OnBeginDrag(PointerEventData eventData) {
        placeHolder = new GameObject();
        placeHolder.transform.SetParent( this.transform.parent );
        LayoutElement le = placeHolder.AddComponent<LayoutElement>();
        le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        placeHolder.transform.SetSiblingIndex(this.transform.GetSiblingIndex() );

        parentToReturnTo = this.transform.parent;
        placeHolderParent = parentToReturnTo;
        this.transform.SetParent(this.transform.parent.parent);

        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
        
        
    }

    public void OnDrag(PointerEventData eventData) {
        
        this.transform.position = eventData.position;
        // Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // this.GetComponent<RectTransform>().anchoredPosition = new Vector3(MousePos.x, MousePos.y, 0);

        // this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0);
        
        Debug.Log(eventData.position.ToString());
        // Debug.Log(MousePos);
        // transform.position = (Vector3)eventData.delta;
        if (placeHolder.transform.parent != placeHolderParent)
        {
            placeHolder.transform.SetParent(placeHolderParent);
        }
        
        // this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0);
        
        int newSiblingIndex = placeHolderParent.childCount;

        for (int i = 0; i < placeHolderParent.childCount; i++)
        {
            if (this.transform.position.x < placeHolderParent.GetChild(i).position.x)
            {
                newSiblingIndex = i;

                if (placeHolder.transform.GetSiblingIndex() < newSiblingIndex)
                    newSiblingIndex--;
                
                break;
            }
        }

        placeHolder.transform.SetSiblingIndex(newSiblingIndex);
    }


    public void OnEndDrag(PointerEventData eventData) {
        
        this.transform.SetParent(parentToReturnTo);
        this.transform.localPosition = new Vector3(0,0,0);
        this.transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());
        this.GetComponent<CanvasGroup>().blocksRaycasts = true;
        
        Debug.Log(this.transform.parent.name);

        if (this.transform.parent.name != "BoatsSlot")
        {
            this.GetComponent<IDraggable>().OnDragEnd();
        }
            
        
        Destroy(placeHolder);
    }
}