using UnityEngine;
using UnityEngine.EventSystems;

public class DeckListPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(eventData.dragging)
        {
            var card = eventData.pointerDrag;
            if (card == null)
                return;

            var cardComponent = card.GetComponent<CollectionCard>();
            if(cardComponent != null)
                cardComponent.DeckListInOut(false);

            var deckComponent = card.GetComponent<DeckCard>();
            if (deckComponent != null)
            {
                deckComponent.DeckListInOut(false);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            var card = eventData.pointerDrag;
            if (card == null)
                return;

            var cardComponent = card.GetComponent<CollectionCard>();
            if(cardComponent != null)
                cardComponent.DeckListInOut(true);

            var deckComponent = card.GetComponent<DeckCard>();
            if(deckComponent != null)
            {
                deckComponent.DeckListInOut(true);
            }
                
        }
    }

}
