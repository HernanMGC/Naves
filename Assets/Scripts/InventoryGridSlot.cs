using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryGridSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Start is called before the first frame update
    public bool IsEven = true;

    void Start()
    {
        UpdateBGColor();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("OnPointerEnter 2");
        //Debug.Log(eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<InventoryGrid>());
        eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<InventoryGrid>().OnCursorOverSlot(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("OnPointerExit 2");
    }

    public void UpdateBGColor()
    {
        if (!IsEven)
        {
            Image image = GetComponent(typeof(Image)) as Image;
            image.color = Color.black;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("OnPinterClick 2");
        TryAddSelectedCardInSlot();
    }

    public void TryAddSelectedCardInSlot()
    {
        GetComponentInParent<InventoryGrid>().AddItem(this);
    }
}
