using UnityEngine;
using UnityEngine.EventSystems;

public class CollectionCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // card data 
    public GameObject origin;
    public GameObject pageObject;
    public RectTransform collectionCanvas;
    public GameObject explanationCard;

    public GameObject card;
    public GameObject deck;
    
    private RectTransform currentTransform;
    private Vector2 pointerOffset;

    private GameObject draggingObject;
   
    public void DeckListInOut(bool inout)
    {
        // 하스스톤을 덱 리스트에 넣을 때 확인해보니 바로 바뀌는 건 맞다. 
        // object 형태에 따라 effect가 추가가 된 것일 뿐이다.
        var cardComponent = draggingObject.GetComponentInChildren<CollectionCard>(true);
        cardComponent.card.SetActive(inout);
        cardComponent.deck.SetActive(!inout);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            var pageInformation = pageObject.GetComponent<PageInformation>();
            if (pageInformation != null)
            {
                if (!pageInformation.isSelectingDeckList)
                {
                    // Error Message를 띄워야 할 것 같은데?
                    Debug.Log("덱 리스트에 카드를 넣을 수 없습니다.");
                    return;
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Card를 우클릭했다.");
            explanationCard.SetActive(true);

            var cardComponent = origin.GetComponent<Card>();
            var collectionCardData = cardComponent.GetCollectionCardData();
            var explanationComponent = explanationCard.GetComponentInChildren<ExplanationCard>(true);
            RectTransform buttonRect = this.GetComponent<RectTransform>();
            explanationComponent.SetUp(buttonRect.position, collectionCardData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            var pageInformation = pageObject.GetComponent<PageInformation>();
            if (pageInformation != null)
            {
                if (!pageInformation.isSelectingDeckList)
                    return;
            }

            if(draggingObject == null)
            { 
                draggingObject = Instantiate(origin, collectionCanvas);

                var canvasGroup = draggingObject.GetComponent<CanvasGroup>();
               

                if (canvasGroup == null)
                    canvasGroup = draggingObject.AddComponent<CanvasGroup>();

                canvasGroup.blocksRaycasts = true;

                var eventManager = Locator<EventManager>.Get();
                DeckListInOut(false);

                eventManager.Notify(ChannelInfo.InputDeck, draggingObject);
                
                draggingObject = null;
            }
            currentTransform = null;
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Card를 우클릭이 끝났다.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            var pageInformation = pageObject.GetComponent<PageInformation>();
            if (pageInformation != null)
            {
                if (!pageInformation.isSelectingDeckList)
                    return;
            }

            Debug.Log("Card를 좌클릭에서 드래그를 시작했다.");
            GameObject cardInstance = Instantiate(origin, collectionCanvas);
            
            draggingObject = cardInstance;
            currentTransform = cardInstance.GetComponent<RectTransform>();
            var canvasGroup = cardInstance.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                canvasGroup = cardInstance.AddComponent<CanvasGroup>();
            }

            canvasGroup.blocksRaycasts = false;

            RectTransform buttonRect = this.GetComponent<RectTransform>();
            currentTransform.position = buttonRect.position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(collectionCanvas, eventData.position, eventData.pressEventCamera, out Vector2 mouseLocalPoint);

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
        if(draggingObject != null)
        {
            Debug.Log("드래그 Object가 끝났다");

            var dragComponent = draggingObject.GetComponentInChildren<CollectionCard>(true);
            
            if(dragComponent.deck.activeSelf)
            {
                var canvasGroup = draggingObject.GetComponent<CanvasGroup>();

                if (canvasGroup == null)
                    canvasGroup = draggingObject.AddComponent<CanvasGroup>();

                canvasGroup.blocksRaycasts = true;

                var eventManager = Locator<EventManager>.Get();
                DeckListInOut(false);
                eventManager.Notify(ChannelInfo.InputDeck, draggingObject);
            }
            else
            {
                Destroy(draggingObject);
            }

            draggingObject = null;
        }
    }



    private void SetObjectPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(collectionCanvas, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        currentTransform.anchoredPosition = localPoint + pointerOffset;
    }

}
