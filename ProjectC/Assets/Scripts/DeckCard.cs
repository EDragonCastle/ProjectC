using UnityEngine;
using UnityEngine.EventSystems;

public class DeckCard : MonoBehaviour,  IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public GameObject origin;
    public RectTransform canvasParent;

    private Transform viewPortParent;

    public GameObject card;
    public GameObject deck;

    private RectTransform currentTransform;
    private Vector2 pointerOffset;

    private Vector3 initPosition;

    // 정확히 말하면 click 쪽이니까
    public void DeckListButton()
    {
        Destroy(origin);
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.OutputDeck);
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

            if (canvasGroup == null)
            {
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
            Destroy(origin);
            var eventManager = Locator<EventManager>.Get();
            eventManager.Notify(ChannelInfo.OutputDeck);
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
