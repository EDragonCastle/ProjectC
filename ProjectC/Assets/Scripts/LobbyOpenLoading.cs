using UnityEngine;
using DG.Tweening;

// 최상단으로 빼는게 더 나을 수 있겠다. 어차피 하나의 Class로 치기 때문에
public class LobbyOpenLoading : MonoBehaviour
{
    public GameObject lobbyParent;

    public GameObject centerDoor;
    public GameObject centerLogo;
    public GameObject loading;

    public GameObject leftPivot;
    public GameObject rightPivot;

    public GameObject questUI;
    public GameObject shopUI;
    public GameObject cardUI;
    public GameObject collectionUI;

    // Testing ui
    public GameObject battle;
    public GameObject collection;
    public GameObject shop;
    public GameObject mode;

    public float duration = 1.0f;
    public float zoomScale = 2.5f;

    private RectTransform centerDoorRectTransform;
    private RectTransform centerLogoTransform;
    private RectTransform loadingRectTransform;

    private bool trigger = false;
    private bool isOpen = false;

    private float distance = 1000f;

    private int openIndex = 0;

    private void Awake()
    {
        centerDoorRectTransform = centerDoor.GetComponent<RectTransform>();
        centerLogoTransform = centerLogo.GetComponent<RectTransform>();
        loadingRectTransform = loading.GetComponent<RectTransform>();
    }

    private async void Start()
    {
        LobbyOpening();
        await Cysharp.Threading.Tasks.UniTask.WaitUntil(() => GameManager.isReadyGameManager);

        var uiManager = Locator<UIManager>.Get();
        uiManager.SetLobby(this.gameObject);

    }

    private void OnEnable()
    {
        if (!isOpen) return;

        CloseDoor(openIndex);
        isOpen = false;
    }

    private void OnDisable()
    {
        centerDoor.SetActive(true);
        centerLogo.SetActive(false);
    }

    private void LobbyOpening()
    {
        var lobbyRectTransform = lobbyParent.GetComponent<RectTransform>();

        DG.Tweening.Sequence sequence = DOTween.Sequence();

        centerLogo.SetActive(true);
        centerDoor.SetActive(false);
        centerLogoTransform.localRotation = Quaternion.Euler(0, 0, 0);

        var questRectTransfrom = questUI.GetComponent<RectTransform>();
        var shopRectTransfrom = shopUI.GetComponent<RectTransform>();
        var cardRectTransfrom = cardUI.GetComponent<RectTransform>();
        var collectionRectTransfrom = collectionUI.GetComponent<RectTransform>();

        questRectTransfrom.anchoredPosition = new Vector2(questRectTransfrom.anchoredPosition.x - distance, questRectTransfrom.anchoredPosition.y);
        shopRectTransfrom.anchoredPosition = new Vector2(shopRectTransfrom.anchoredPosition.x - distance, shopRectTransfrom.anchoredPosition.y);
        cardRectTransfrom.anchoredPosition = new Vector2(cardRectTransfrom.anchoredPosition.x + distance, cardRectTransfrom.anchoredPosition.y);
        collectionRectTransfrom.anchoredPosition = new Vector2(collectionRectTransfrom.anchoredPosition.x + distance, collectionRectTransfrom.anchoredPosition.y);

        float openingDuration = 0.5f;

        sequence.Append(lobbyRectTransform.DOScale(1f, openingDuration));
        sequence.Append(centerLogoTransform.DORotate(new Vector3(0, -90, 0), openingDuration/2).SetEase(Ease.Linear));
        sequence.AppendCallback(() => {
            centerLogo.SetActive(false); 
            centerDoor.SetActive(true);
            centerDoorRectTransform.localRotation = Quaternion.Euler(0, 90, 0);
        });
        sequence.Append(centerDoorRectTransform.DORotate(new Vector3(0, 0, 0), openingDuration/2).SetEase(Ease.Linear));

        sequence.OnComplete(() => { 
            centerLogo.SetActive(false);
            CloseOther();
        });
    }

    public void OnButtonClick(int index)
    {
        openIndex = index;

        OpenOther();

        var sequence = CenterLogo();
        sequence.OnComplete(() => {
            Open(index);
            OpenDoor();
            isOpen = true;
        });
    }

    private DG.Tweening.Sequence CenterLogo()
    {
        DOTween.Kill(centerDoorRectTransform);
        DOTween.Kill(centerLogoTransform);
        DOTween.Kill(loadingRectTransform);

        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(centerDoorRectTransform.DORotate(new Vector3(0, -90, 0), 0.01f).SetEase(Ease.Linear));

        sequence.AppendCallback(() => {
            centerDoor.SetActive(false);
            centerLogo.SetActive(true);

            centerDoorRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            centerLogoTransform.localRotation = Quaternion.Euler(0, -90, 0);
        });

        sequence.Append(centerLogoTransform.DORotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.OutQuad));

        if (trigger)
            sequence.Join(loadingRectTransform.DORotate(new Vector3(0, 0, 120), 0.5f).From(new Vector3(0, -90, 30)));
        else
            sequence.Join(loadingRectTransform.DORotate(new Vector3(0, 0, 30), 0.5f).From(new Vector3(0, -90, -60)));

        trigger = !trigger;

        return sequence;
    }

    private void OpenDoor()
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        // RectTransform을 받아온다.
        var leftRectTransform = leftPivot.GetComponent<RectTransform>();
        var rightRectTransform = rightPivot.GetComponent<RectTransform>();

        // 좌우 회전
        sequence.Append(leftRectTransform.DORotate(new Vector3(0, 90, 0), 0.8f).SetEase(Ease.OutCubic));
        sequence.Join(rightRectTransform.DORotate(new Vector3(0, -90, 0), 0.8f).SetEase(Ease.OutCubic));

        // 크기 확대
        sequence.Join(leftRectTransform.DOScale(zoomScale, duration).SetEase(Ease.InCubic));
        sequence.Join(rightRectTransform.DOScale(zoomScale, duration).SetEase(Ease.InCubic));

        // 바깥에서 밀어내기
        sequence.Join(leftRectTransform.DOAnchorPosX(-distance, duration).SetRelative().SetEase(Ease.InCubic));
        sequence.Join(rightRectTransform.DOAnchorPosX(distance, duration).SetRelative().SetEase(Ease.InCubic));

        sequence.OnComplete(() => {
            Debug.Log("Complete Open Door");
            this.gameObject.SetActive(false);
        });
    }

    private void OpenOther()
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        var questRectTransfrom = questUI.GetComponent<RectTransform>();
        var shopRectTransfrom = shopUI.GetComponent<RectTransform>();
        var cardRectTransfrom = cardUI.GetComponent<RectTransform>();
        var collectionRectTransfrom = collectionUI.GetComponent<RectTransform>();
        
        sequence.Append(questRectTransfrom.DOAnchorPosX(-distance, duration).SetRelative().SetEase(Ease.InCubic));
        sequence.Join(shopRectTransfrom.DOAnchorPosX(-distance, duration).SetRelative().SetEase(Ease.InCubic));
        sequence.Join(cardRectTransfrom.DOAnchorPosX(distance, duration).SetRelative().SetEase(Ease.InCubic));
        sequence.Join(collectionRectTransfrom.DOAnchorPosX(distance, duration).SetRelative().SetEase(Ease.InCubic));
    }

    private void CloseOther()
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        var questRectTransfrom = questUI.GetComponent<RectTransform>();
        var shopRectTransfrom = shopUI.GetComponent<RectTransform>();
        var cardRectTransfrom = cardUI.GetComponent<RectTransform>();
        var collectionRectTransfrom = collectionUI.GetComponent<RectTransform>();

        sequence.Append(questRectTransfrom.DOAnchorPosX(distance, duration).SetRelative().SetEase(Ease.OutCubic));
        sequence.Join(shopRectTransfrom.DOAnchorPosX(distance, duration).SetRelative().SetEase(Ease.OutCubic));
        sequence.Join(cardRectTransfrom.DOAnchorPosX(-distance, duration).SetRelative().SetEase(Ease.OutCubic));
        sequence.Join(collectionRectTransfrom.DOAnchorPosX(-distance, duration).SetRelative().SetEase(Ease.OutCubic));
    }


    private void CloseDoor(int index)
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        // RectTransform을 받아온다.
        var leftRectTransform = leftPivot.GetComponent<RectTransform>();
        var rightRectTransform = rightPivot.GetComponent<RectTransform>();


        // 좌우 회전
        sequence.Append(leftRectTransform.DORotate(new Vector3(0, 0, 0), 0.8f).SetEase(Ease.InCubic));
        sequence.Join(rightRectTransform.DORotate(new Vector3(0, 0, 0), 0.8f).SetEase(Ease.InCubic));

        // 크기 확대
        sequence.Join(leftRectTransform.DOScale(1, duration).SetEase(Ease.OutCubic));
        sequence.Join(rightRectTransform.DOScale(1, duration).SetEase(Ease.OutCubic));

        // 원래대로 돌아오기
        sequence.Join(leftRectTransform.DOAnchorPosX(distance, duration).SetRelative().SetEase(Ease.OutCubic));
        sequence.Join(rightRectTransform.DOAnchorPosX(-distance, duration).SetRelative().SetEase(Ease.OutCubic));

        sequence.OnComplete(() => {
            Debug.Log("Complete Close Door");
            Close(index);
            CloseOther();
        });
    }

    private void Open(int index)
    {
        switch(index)
        {
            case 0:
                battle.SetActive(true);
                break;
            case 1:
                collection.SetActive(true);
                break;
            case 2:
                shop.SetActive(true);
                break;
            default:
                mode.SetActive(true);
                break;
        }
    }

    private void Close(int index)
    {
        switch (index)
        {
            case 0:
                battle.SetActive(false);
                break;
            case 1:
                collection.SetActive(false);
                break;
            case 2:
                shop.SetActive(false);
                break;
            default:
                mode.SetActive(false);
                break;
        }
    }
}
