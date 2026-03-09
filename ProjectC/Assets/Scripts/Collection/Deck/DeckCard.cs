using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DeckCard : MonoBehaviour,  IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public GameObject origin;
    public RectTransform canvasParent;

    public TextMeshProUGUI deckCountText;
    public int deckCount = 0;
    private Transform viewPortParent;

    public GameObject card;
    public GameObject deck;

    private RectTransform currentTransform;
    private Vector2 pointerOffset;

    private Vector3 initPosition;

    public void DeckListButton()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.OutputDeck, origin);
        // Destroy(origin);
    }

    public void CurrentDeckCount()
    {
        deckCountText.text = $"X {deckCount}";
    }


    public void DeckListInOut(bool inout)
    {
        card.SetActive(inout);
        deck.SetActive(!inout);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            viewPortParent = origin.transform.parent;
            origin.transform.SetParent(canvasParent);
            currentTransform = origin.GetComponent<RectTransform>();
            initPosition = new Vector3(currentTransform.anchoredPosition.x, currentTransform.anchoredPosition.y, 0);
           
            var canvasGroup = origin.GetComponent<CanvasGroup>();

            if (canvasGroup == null) {
                canvasGroup = origin.AddComponent<CanvasGroup>();
            }

            canvasGroup.blocksRaycasts = false;

            RectTransform buttonRect = this.GetComponent<RectTransform>();
            currentTransform.position = buttonRect.position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasParent, eventData.position, eventData.pressEventCamera, out Vector2 mouseLocalPoint);
            pointerOffset = currentTransform.anchoredPosition - mouseLocalPoint;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(currentTransform != null)
        {
            SetObjectPosition(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("DeckCard에서 드래그 끝");

        var dragComponent = origin.GetComponentInChildren<CollectionCard>(true);

        if (dragComponent.card.activeSelf)
        {
            var eventManager = Locator<EventManager>.Get();
            eventManager.Notify(ChannelInfo.OutputDeck, origin);
        }
        else
        {
            origin.transform.SetParent(viewPortParent);

            var canvasGroup = origin.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup = origin.AddComponent<CanvasGroup>();

            canvasGroup.blocksRaycasts = true;

            currentTransform.anchoredPosition = initPosition;
        }
    }

    private void SetObjectPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasParent, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        currentTransform.anchoredPosition = localPoint + pointerOffset;
    }
}
