using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Vector2Int CardSize = new(1, 1);
    public GameObject BG;
    public GameObject Text;
    private Vector2 OldPosition;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void InitializeCard(Vector2Int? InCardSize = null)
    {
        CardSize = InCardSize ?? new Vector2Int(1,1);
        Text text = Text.GetComponent<Text>();
        text.text = CardSize.x.ToString() + "x" + CardSize.y.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DeckManager deckManager = transform.GetComponentInParent<DeckManager>();
        if (deckManager)
        {
            deckManager.UpdateSelectedCard(this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DeckManager deckManager = transform.GetComponentInParent<DeckManager>();
        if (deckManager)
        {
            deckManager.UpdateSelectedCard(this);
        }


        OldPosition = transform.GetComponent<RectTransform>().anchoredPosition;

        GetComponent<CanvasGroup>().blocksRaycasts = false; // disable registering hit on item
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool shouldDelete = false;
        Debug.Log(eventData.pointerCurrentRaycast.gameObject);
        if (eventData.pointerCurrentRaycast.gameObject == null)
        {
            return;
        }

        InventoryGridSlot slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<InventoryGridSlot>();
        if (slot)
        {
            slot.TryAddSelectedCardInSlot();
            shouldDelete = true;
        }
        else 
        {
            this.transform.GetComponent<RectTransform>().anchoredPosition = OldPosition; //back to old pos
            GetComponent<CanvasGroup>().blocksRaycasts = true; //register hit on item again
        }

        DeckManager deckManager = transform.GetComponentInParent<DeckManager>();
        if (deckManager)
        {
            deckManager.UpdateSelectedCard(null);
        }

        Destroy(gameObject);
    }
}
