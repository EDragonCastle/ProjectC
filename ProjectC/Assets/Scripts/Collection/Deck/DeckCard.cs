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

    private Card cardInstance;
    private int curIndex;

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

    public void CurrentDeckCount(int count)
    {
        deckCountText.text = $"X {count}";
        deckCount = count;
    }


    public void DeckListInOut(bool inout)
    {
        if (cardInstance == null)
        {
            card.SetActive(inout);
            deck.SetActive(!inout);
        }
        else
        {
            cardInstance.ActiveCards(inout);
        }
    }

    // 자신이 deck count가 2개면 새로운 object를 만들어야 한다. 
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            viewPortParent = origin.transform.parent;
            curIndex = origin.transform.GetSiblingIndex();
            // deckCount 1개면 원본을 넣어주고 아니라면 새로 생성한다.
            if(deckCount == 1) {
                origin.transform.SetParent(canvasParent);
                currentTransform = origin.GetComponent<RectTransform>();

                var canvasGroup = origin.GetComponent<CanvasGroup>();

                if (canvasGroup == null) {
                    canvasGroup = origin.AddComponent<CanvasGroup>();
                }

                canvasGroup.blocksRaycasts = false;
            }
            else {
                var factory = Locator<Factory>.Get();
                Card cardComponent = origin.GetComponent<Card>();
               
                cardInstance = factory.Create(cardComponent, canvasParent);
                cardInstance.ActiveCards(false);


                currentTransform = cardInstance.GetComponent<RectTransform>();
                deckCount--;
                CurrentDeckCount(deckCount);

                var canvasGroup = cardInstance.gameObject.GetComponent<CanvasGroup>();

                if (canvasGroup == null)
                {
                    canvasGroup = cardInstance.gameObject.AddComponent<CanvasGroup>();
                }

                canvasGroup.blocksRaycasts = false;
            }
          

            initPosition = new Vector3(currentTransform.anchoredPosition.x, currentTransform.anchoredPosition.y, 0);

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
        Card destoryObjectComponent = null;
        if (cardInstance != null)
            destoryObjectComponent = cardInstance.GetComponent<Card>();
        else
            destoryObjectComponent = origin.GetComponent<Card>();

        if (destoryObjectComponent.cardObject.activeSelf)
        {
            var eventManager = Locator<EventManager>.Get();
            eventManager.Notify(ChannelInfo.OutputDeck, origin);

            if(cardInstance != null)
            {
                var factory = Locator<Factory>.Get();
                factory.Release(cardInstance);
                cardInstance = null;
            }
        }
        else
        {
            origin.transform.SetParent(viewPortParent);
            origin.transform.SetSiblingIndex(curIndex);

            var canvasGroup = origin.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup = origin.AddComponent<CanvasGroup>();

            canvasGroup.blocksRaycasts = true;

            currentTransform.anchoredPosition = initPosition;

            if(cardInstance != null)
            {
                var factory = Locator<Factory>.Get();
                factory.Release(cardInstance);
                cardInstance = null;
            }
            deckCount++;
            CurrentDeckCount(deckCount);
        }
        currentTransform = null;
    }

    private void SetObjectPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasParent, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        currentTransform.anchoredPosition = localPoint + pointerOffset;
    }
}
