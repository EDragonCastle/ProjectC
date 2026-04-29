using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class BattleCardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // origin을 키우면 안 된다. 버트도 같이 커지기 때문에 그래서 카드 정보만 커져야 한다.
    public RectTransform testCanvas;
    
    public GameObject cardOrigin;
    public GameObject cardParent;
    public GameObject cardInfo;
    private RectTransform CardOriginRect;
    private RectTransform cardInfoRect;
    private Vector2 pointerOffset;

    private Vector3 visualCardPostion;

    // 확대 Scale은 어느정도?
    // 기본 값은 1.2로 하자.
    private readonly Vector3 originScale = new Vector3(1.2f, 1.2f, 1.2f);
    private readonly Vector3 highLightScale = new Vector3(2f, 2f, 2f);

    private Quaternion defaultRotation;

    private Vector3 defaultPosition;

    private bool isInHand = false;
    public Ease ease;
    public float duration = 0.25f;

    private bool isReturnCard = false;
    private bool isDragging = false;

    // 외부에서 설정해줘야 하나?
    public int cardIndex;
    public Vector3 GetCardOriginScale() => originScale;
    public void SetCardTouchEnable(bool input) => isReturnCard = input;


    private void Awake()
    {
        CardOriginRect = cardOrigin.GetComponent<RectTransform>();
        cardInfoRect = cardInfo.GetComponent<RectTransform>();

        // 일단은 awake 시 세팅
        // scale은 infoRect가 조절해야 하는게 맞다.
        defaultRotation = Quaternion.identity;
        cardInfoRect.localScale = originScale;

        var rect = this.GetComponent<RectTransform>();
        visualCardPostion = new Vector3(0, rect.sizeDelta.y, 0);
    }

    public void CardSetUp(Vector3 _defaultPosition, Quaternion _defaultRotation, int index)
    {
        // init setting 및 card 이동
        defaultPosition = _defaultPosition;
        defaultRotation = _defaultRotation;
        cardIndex = index;
        
        HandleParent(false);
        IsHighLightCard(false);

        CardOriginRect.DOKill();

        CardOriginRect.DOLocalMove(defaultPosition, duration).SetEase(Ease.Unset);
        CardOriginRect.DORotateQuaternion(defaultRotation, duration).SetEase(Ease.Unset);
    }

    public async UniTask CardSetUpAsync(Vector3 _defaultPosition, Quaternion _defaultRotation, int index)
    {
        // init setting 및 card 이동
        defaultPosition = _defaultPosition;
        defaultRotation = _defaultRotation;
        cardIndex = index;

        HandleParent(false);
        IsHighLightCard(false);

        CardOriginRect.DOKill();

        await UniTask.WhenAll(
            CardOriginRect.DOLocalMove(defaultPosition, duration).SetEase(Ease.Unset).ToUniTask(),
            CardOriginRect.DORotateQuaternion(defaultRotation, duration).SetEase(Ease.Unset).ToUniTask()
        );
    }

    public async UniTask CardSetUpAsync(Vector3 _defaultPosition, Quaternion _defaultRotation, int index, CancellationToken token)
    {
        // init setting 및 card 이동
        defaultPosition = _defaultPosition;
        defaultRotation = _defaultRotation;
        cardIndex = index;

        HandleParent(false);
        IsHighLightCard(false);

        CardOriginRect.DOKill();

        await UniTask.WhenAll(
            CardOriginRect.DOLocalMove(defaultPosition, duration).SetEase(Ease.Unset).ToUniTask(cancellationToken: token),
            CardOriginRect.DORotateQuaternion(defaultRotation, duration).SetEase(Ease.Unset).ToUniTask(cancellationToken: token)
        );
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isReturnCard || isDragging)
            return;

        Debug.Log("Card Enter");
        HandleParent(true);
        IsHighLightCard(true);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isReturnCard || isDragging)
            return;

        Debug.Log("Card Exit");
        HandleParent(false);
        IsHighLightCard(false);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isReturnCard)
            return;

        // Drag 위치 시작 지점
        Debug.Log("Test Begin Dragging");
        IsHighLightCard(false);

        var uiManager = Locator<UIManager>.Get();
        var canvasObject = uiManager.GetCollectionCanvas();

        if (canvasObject != null)
            testCanvas = canvasObject.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(testCanvas, eventData.position, eventData.pressEventCamera, out Vector2 mouseLocalPoint);
        pointerOffset = CardOriginRect.anchoredPosition - mouseLocalPoint;
        
        defaultPosition = CardOriginRect.localPosition;
        defaultRotation = CardOriginRect.localRotation;

        CardOriginRect.localRotation = Quaternion.identity;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isReturnCard)
            return;

        var uiManager = Locator<UIManager>.Get();
        var canvasObject = uiManager.GetCollectionCanvas();

        if (canvasObject != null)
            testCanvas = canvasObject.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(testCanvas, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        CardOriginRect.anchoredPosition = localPoint + pointerOffset;

        var battleManager = Locator<BattleManager>.Get();
        var handPanel = battleManager.GetHandPanel();
        RectTransform handRect = handPanel.GetComponent<RectTransform>();

        if(RectTransformUtility.RectangleContainsScreenPoint(handRect, eventData.position, eventData.pressEventCamera))
        {
            Debug.Log("핸드 영역 진입");
            /// 여기에 손에 들어 올지 안 들어올 지 표시해줘야 한다.
            isInHand = true;
        }
        else
        {
            Debug.Log("핸드 밖에 있다.");
            isInHand = false;

            // 그럼 여기서 Notify로 카드를 썼다고 알려줘야 할까?
        }

        handPanel.SetActive(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Drag 위치 끝 지점이 손패에 있다면 
        var battleManager = Locator<BattleManager>.Get();
        GameObject handPanel = battleManager.GetHandPanel();
        isDragging = false;

        if (isInHand)
        {
            Debug.Log("손에 다시 되돌아 가야한다.");
            isReturnCard = true;

            DG.Tweening.Sequence sequence = DOTween.Sequence();

            sequence.Append(CardOriginRect.DOLocalMove(defaultPosition, duration)).SetEase(ease)
                    .Join(CardOriginRect.DORotateQuaternion(defaultRotation, duration))
                    .OnComplete(()=> {
                        isReturnCard = false;
                        HandleParent(false);
                    });
        }
        else
        {
            Debug.Log("손패 밖에 있어서 실행해야 한다.");
            // 해당 패널 문제가 아닌것 같다.
            handPanel.SetActive(true);

            Debug.Log("카드 실행");

            // 카드 삭제
            var eventManager = Locator<EventManager>.Get();
            eventManager.Notify(ChannelInfo.UsingBattleCard, cardIndex);

            Destroy(cardOrigin);

            handPanel.SetActive(false);
            
        }

        handPanel.SetActive(false);
    }


    private void IsHighLightCard(bool isHighlight)
    {
        if(isHighlight)
        {
            cardInfoRect.localPosition = Vector3.zero + visualCardPostion;
            cardInfoRect.localRotation = Quaternion.Euler(0, 0, -defaultRotation.eulerAngles.z);

            cardInfoRect.localScale = highLightScale;
        }
        else
        {
            cardInfoRect.localPosition = Vector3.zero;
            cardInfoRect.localRotation = Quaternion.identity;

            cardInfoRect.localScale = originScale;
        }
    }

    private void HandleParent(bool isHand)
    {
        if(isHand)
        {
            var uiManager = Locator<UIManager>.Get();
            var battleCanvas = uiManager.GetCollectionCanvas();
            CardOriginRect.SetParent(battleCanvas.transform);
            //cardInfoRect.SetParent(battleCanvas.transform);
        }
        else
        {
            var battleManager = Locator<BattleManager>.Get();
            var handParent = battleManager.GetHandParent();
            CardOriginRect.SetParent(handParent.transform);
            CardOriginRect.SetSiblingIndex(cardIndex);
            //cardInfoRect.SetParent(cardParent.transform);
            //cardInfoRect.SetSiblingIndex(0);
        }
    }
}




// 어떤 문제를 가지고 있지?
// 카드 드래그시 버벅이는 문제가 잇다.
// 드래그하면서 최상위 canvas로 올렸는데 거기서 생긴 문제같다.
// 그렇다고 canvas만 따로 빼두니 Button이 비활성화되기 때문에 인식이 되지 않는다.