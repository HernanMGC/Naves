using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{

    [SerializeField]
    GameObject cardPrefab;

    [SerializeField]
    GameObject inventoryRef;

    // Start is called before the first frame update
    void Start()
    {
        var card1 = Instantiate(cardPrefab, transform);
        Card card1Component = card1.GetComponent<Card>();
        card1Component.InitializeCard(new(1, 1));

        var card2 = Instantiate(cardPrefab, transform);
        Card card2Component = card2.GetComponent<Card>();
        card2Component.InitializeCard(new(1, 2));

        var card3 = Instantiate(cardPrefab, transform);
        Card card3Component = card3.GetComponent<Card>();
        card3Component.InitializeCard(new(2, 2));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSelectedCard(Card inSelectedCard)
    {
        inventoryRef.GetComponent<InventoryGrid>().UpdateSelectedCard(inSelectedCard);
    }
}
