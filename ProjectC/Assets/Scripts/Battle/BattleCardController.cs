using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class BattleCardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public BattleCard battleCard;
    public RectTransform testCanvas;
    
    public GameObject cardOrigin;
    public GameObject cardParent;
    public GameObject cardInfo;
    private RectTransform CardOriginRect;
    private RectTransform cardInfoRect;
    private Vector2 pointerOffset;

    private Vector3 visualCardPostion;

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
    private int battleIndex;
    public Vector3 GetCardOriginScale() => originScale;
    public void SetCardTouchEnable(bool input) => isReturnCard = input;

    public GameObject dummyObject;


    private bool testBattleField;

    // 여기서 잠시 대기
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
            testBattleField = !testBattleField;
    }


    private void OnEnable()
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

    public async UniTask CardPositionSetupAsync(Vector3 _defaultPosition, Quaternion _defaultRotation, int index)
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

    public async UniTask CardPositionSetupAsync(Vector3 _defaultPosition, Quaternion _defaultRotation, int index, CancellationToken token)
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

        // 여기서 Dragging을 허용할 건지 안 할건지 판단해야 한다.
        // cost가 있을 수도 있고 하수인이 가득차서 사용 못 할 수도 있다.
        IsUsingCard();
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

        // Dummy를 생성해야 할 것 같은데?
        dummyObject.SetActive(true);
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

        // 마우스 위치가 하수인 위치에 어딨는지 알아야 한다.
        // 그러면 내 카드가 하수인인지 아닌지 알아야 하는데?

        // 아래 로직은 하수인이면 하수인이 어디 위치해야 할 지 보여주는 로직이다.
        var playerField = battleManager.GetBattleField().GetComponent<BattleField>().playerField;
        var playerFieldTransform = playerField.transform;

        if (dummyObject.transform.parent != playerFieldTransform)
            dummyObject.transform.SetParent(playerFieldTransform);

        int targetIndex = playerFieldTransform.childCount;

        int count = 0;
        for(int i = targetIndex - 1; i >= 0; i--)
        {
            Transform child = playerFieldTransform.GetChild(i);

            if (child == dummyObject.transform) continue;

            RectTransform childRect = child.GetComponent<RectTransform>();

            Vector2 childScreenPosition = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, childRect.position);

            if(eventData.position.x < childScreenPosition.x)
                break;

            count++;
        }

        battleIndex = targetIndex-count-1;
        dummyObject.transform.SetSiblingIndex(battleIndex);
        // 여기는 핸드 영역에 들어오는지 안 들어오는지 확인하는 로직
        if (RectTransformUtility.RectangleContainsScreenPoint(handRect, eventData.position, eventData.pressEventCamera))
        {
            Debug.Log("핸드 영역 진입");
            /// 여기에 손에 들어 올지 안 들어올 지 표시해줘야 한다.
            isInHand = true;
        }
        else
        {
            Debug.Log("핸드 밖에 있다.");
            isInHand = false;
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
            // 카드 삭제
            var eventManager = Locator<EventManager>.Get();

            // 그렇다면 여기서 cardIndex도 중요하지만 하수인 위치도 중요하다.
            // 근데 이 카드가 하수인인지 주문인지 무기인지 모른채로 넘어간다.
            BattleFieldObjectInformation battleInfo = new BattleFieldObjectInformation();
            battleInfo.card = cardOrigin;
            battleInfo.cardType = battleCard.GetBattleCardType();
            battleInfo.ability = battleCard.GetAbilityData();
            battleInfo.usingIndex = cardIndex;

            //battleInfo.isPlayer = true;
            
            battleInfo.isPlayer = testBattleField;
            battleInfo.battleIndex = battleIndex;

            //여기서는 위치 옮기는 것만 되는게 맞네.
            eventManager.Notify(ChannelInfo.UsingBattleCard, battleInfo);

            // Destory 하면 문제가 생기나? 여기때문에 문제가 생기는 듯?
            cardOrigin.SetActive(false);
        }

        dummyObject.SetActive(false);
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
        }
        else
        {
            var battleManager = Locator<BattleManager>.Get();
            var handParent = battleManager.GetHandParent();
            CardOriginRect.SetParent(handParent.transform);
            CardOriginRect.SetSiblingIndex(cardIndex);
        }
    }


    private void IsUsingCard()
    {
        // Cost를 확인한다.

        // 여기서 하수인이면 내 Field를 확인해야 한다.
    }
}


// 어떤 문제를 가지고 있지?
// 카드 드래그시 버벅이는 문제가 잇다.
// 드래그하면서 최상위 canvas로 올렸는데 거기서 생긴 문제같다.
// 그렇다고 canvas만 따로 빼두니 Button이 비활성화되기 때문에 인식이 되지 않는다.