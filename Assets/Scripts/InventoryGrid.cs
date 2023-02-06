using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryGrid : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int Rows = 9;
    public int Columns = 4;
    public int CellSize = 32;

    [SerializeField]
    GameObject slotLayer;

    [SerializeField]
    GameObject cursorLayer;

    [SerializeField]
    GameObject itemsLayer;

    [SerializeField]
    GameObject slotPrefab;

    [SerializeField]
    GameObject cursorPrefab;

    [SerializeField]
    GameObject itemPrefab;

    private bool IsDragging = false;
    private List<InventoryGridSlot> InventoryGridSlots = new List<InventoryGridSlot>();
    private List<GameObject> ItemList = new List<GameObject>();
    private GameObject CursorRef = null;
    private Card SelectedCard;

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
    }

    internal void UpdateSelectedCard(Card inSelectedCard)
    {
        SelectedCard = inSelectedCard;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDragging)
        {
            //Debug.Log("OnDrag");
            IsDragging = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;
        //Debug.Log("OnEndDrag");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorRef.SetActive(true);

        //Debug.Log("OnPointerEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorRef.SetActive(false);

        //Debug.Log("OnPointerExit");
    }

    private bool OutOfBounds(Vector2 leftTop, Vector2 bottomRight)
    {
        return (leftTop.x < 0) ||
            (leftTop.y > 0) ||
            (bottomRight.x > (CellSize * Columns)) ||
            (bottomRight.y < (CellSize * Rows * -1));
    }

    private bool DoOverlap(Vector2 leftTopFirst, Vector2 bottomRightFirst, Vector2 leftTopSecond, Vector2 bottomRightSecond)
    {
        Debug.Log(
"l1" + leftTopFirst +
"r1" + bottomRightFirst +
"l2" + leftTopSecond +
"r2" + bottomRightSecond
);
        if (leftTopFirst.x == bottomRightFirst.x || leftTopFirst.y == bottomRightFirst.y || bottomRightSecond.x == leftTopSecond.x || leftTopSecond.y == bottomRightSecond.y)
            return false;

        // If one rectangle is on left side of other
        if (leftTopFirst.x >= bottomRightSecond.x || leftTopSecond.x >= bottomRightFirst.x)
            return false;

        // If one rectangle is above other
        if (bottomRightFirst.y >= leftTopSecond.y || bottomRightSecond.y >= leftTopFirst.y)
            return false;

        return true;
    }

    internal void AddItem(InventoryGridSlot inventoryGridSlot)
    {
        int sizeX = SelectedCard.CardSize.x;
        int sizeY = SelectedCard.CardSize.y;

        int index = InventoryGridSlots.IndexOf(inventoryGridSlot);
        int row = index / Columns;
        int col = index % Columns;

        Debug.Log("Add new item from (" + col * CellSize + "," + row * CellSize * -1 + ") to (" + (col + sizeX) * CellSize + "," + (row + sizeY) * CellSize * -1 + ")");
        Debug.Log("Items in list");
        Debug.Log("Table bounds (" + 0 + "," + 0 + ") (" + Columns * CellSize + "," + Rows * CellSize + ")");

        if ( OutOfBounds(new Vector2(col * CellSize, row * CellSize * -1), new Vector2((col + sizeX) * CellSize, (row + sizeY) * CellSize * - 1)) )
        {
            Debug.Log("Out of the table. ABORT!");
            return;
        }

        foreach (GameObject item in ItemList)
        {
            RectTransform itemRectTransform = item.GetComponent<RectTransform>();
            Debug.Log("Item " + item + " already at (" + (itemRectTransform.localPosition.x) + "," + (itemRectTransform.localPosition.y) + ") to (" + (itemRectTransform.localPosition.x + itemRectTransform.sizeDelta.x) + "," + (itemRectTransform.localPosition.y - itemRectTransform.sizeDelta.y) + ")");

            if ( DoOverlap(
                    new Vector2(col * CellSize, row * CellSize * -1),
                    new Vector2((col + sizeX) * CellSize, (row + sizeY) * CellSize * -1),
                    new Vector2(itemRectTransform.localPosition.x, itemRectTransform.localPosition.y),
                    new Vector2(itemRectTransform.localPosition.x + itemRectTransform.sizeDelta.x, itemRectTransform.localPosition.y - itemRectTransform.sizeDelta.y)
                )
            )
            {
                Debug.Log("Overlaps item!");
                return;
            }
        }


        var itemRef = Instantiate(itemPrefab, itemsLayer.transform);  //generate the slots grid.
        RectTransform itemRefRectTransform = itemRef.GetComponent<RectTransform>();
        itemRefRectTransform.localPosition = new Vector3(col * CellSize, row * CellSize * -1);
        itemRefRectTransform.sizeDelta = new Vector2(sizeX * CellSize, sizeY * CellSize);

        ItemList.Add(itemRef);

    }


    // Pre initialization
    void Awake()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        RectTransform slotLayerRectTransform = slotLayer.GetComponent<RectTransform>();
        RectTransform cursorLayerRectTransform = cursorLayer.GetComponent<RectTransform>();
        RectTransform itemsLayerRectTransform = itemsLayer.GetComponent<RectTransform>();
        GridLayoutGroup gridLayoutGroup = slotLayer.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.cellSize = new Vector2(CellSize, CellSize);
        //Debug.Log("RectTransform: " + slotLayerRectTransform);
        //Debug.Log("GridLayout: CellSize.X:" + CellSize + " CellSize.Y:" + CellSize);
        slotLayerRectTransform.sizeDelta = new Vector2(Columns * CellSize, Rows * CellSize);
        cursorLayerRectTransform.sizeDelta = new Vector2(Columns * CellSize, Rows * CellSize);
        itemsLayerRectTransform.sizeDelta = new Vector2(Columns * CellSize, Rows * CellSize);

        CursorRef = Instantiate(cursorPrefab, cursorLayer.transform);  //generate the slots grid.
        CursorRef.GetComponent<RectTransform>().localPosition = new Vector3();
        CursorRef.SetActive(false);

        for (int i = 0; i < Rows * Columns; i++)
        {
            var itemUI = Instantiate(slotPrefab, slotLayer.transform);  //generate the slots grid.
            InventoryGridSlot gridTableSlot = itemUI.GetComponent<InventoryGridSlot>();
            InventoryGridSlots.Add(gridTableSlot);
            //Debug.Log("Row: " + i / Columns + " Col: " + i % Columns);
            gridTableSlot.IsEven = (((i / Columns) % 2) + (((i % Columns) % 2))) % 2 == 0;
            gridTableSlot.UpdateBGColor();
        }
    }

    public int OnCursorOverSlot(InventoryGridSlot child)
    {
        int index = InventoryGridSlots.IndexOf(child);
        int row = index / Columns;
        int col = index % Columns;
        //Debug.Log("Row: " + row + " Col: " + col);

        CursorRef.GetComponent<RectTransform>().localPosition = new Vector3(col * CellSize, row * CellSize * -1);

        return index;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
