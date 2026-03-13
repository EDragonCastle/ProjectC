using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class CollectionScroll : MonoBehaviour, IChannel
{
    public GameObject contentPivot;
    public GameObject dummyDeck;
    public GameObject newDeckButton;

    public GameObject newDeckListObject;

    public int maxDeckCount;

    // dummy object
    public List<GameObject> contents = new List<GameObject>();

    public float openPosition = 500f;

    public GameObject createDeck;
    public RectTransform leftDeckList;
    public RectTransform rightDeckList;

    private int newDeckIndex;
    private Vector2 leftOriginPos;
    private Vector2 rightOriginPos;
    private float duration = 1.0f;

    private bool isSelect = false;

    private Stack<GameObject> emptyStack = new Stack<GameObject>();

    private int contentIndex;

    private async void Start()
    {
        await UniTask.WaitUntil(() => GameManager.isReadyGameManager);

        var uiManager = Locator<UIManager>.Get();
        uiManager.SetDeckScroll(this.gameObject);

        leftOriginPos = leftDeckList.anchoredPosition;
        rightOriginPos = rightDeckList.anchoredPosition;

        foreach (var obj in contents)
        {
            emptyStack.Push(obj);
        }
        contentIndex = contents.Count - 1;

        newDeckIndex = 0;
    }

    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.OutputDeckList, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.OutputDeckList, HandleEvent);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.OutputDeckList:
                PopDeck();
                break;
        }
    }

    // New Deck List 추가하는 버튼
    public void CreateDeck()
    {
        createDeck.SetActive(true);

        // left door와 right door가 서로 세게 닫히면서 통통 튀는 연출을 주고 싶은데
        leftDeckList.DOKill();
        rightDeckList.DOKill();

        leftDeckList.anchoredPosition = new Vector2(leftOriginPos.x - openPosition, leftDeckList.anchoredPosition.y);
        rightDeckList.anchoredPosition = new Vector2(rightOriginPos.x + openPosition, rightDeckList.anchoredPosition.y);

        leftDeckList.DOAnchorPosX(leftOriginPos.x, duration).SetEase(Ease.OutBounce);
        rightDeckList.DOAnchorPosX(rightOriginPos.x, duration).SetEase(Ease.OutBounce);
    }


    public void CloseDeck()
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        leftDeckList.DOKill();
        rightDeckList.DOKill();

        sequence.Append(leftDeckList.DOAnchorPosX(leftOriginPos.x - openPosition, duration).SetEase(Ease.InOutQuart));
        sequence.Join(rightDeckList.DOAnchorPosX(rightOriginPos.x + openPosition, duration).SetEase(Ease.InOutQuart));

        if(isSelect)
        {
            // 음
            sequence.InsertCallback(duration * 0.4f, () => {
                newDeckListObject.SetActive(true);
                this.gameObject.SetActive(false);
            });
        }

        sequence.OnComplete(() => {
            createDeck.SetActive(false);
        });
    }

    public void SelectDeck()
    {
        isSelect = true;
        CloseDeck();
        PushDeck();
        isSelect = false;

        // 여기서 카드 좌클릭 했을 때 덱에 넣을 수 있는 기능을 활성화 하면 된다.
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.SelectingDeck, true);
    }
    
    public void PushDeck()
    {
        if (maxDeckCount <= newDeckIndex)
        {
            newDeckButton.SetActive(false);
            return;
        }

        newDeckButton.SetActive(true);

        if (emptyStack.Count > 0)
        {
            var empty = emptyStack.Peek();
            emptyStack.Pop();
            empty.SetActive(false);
            //Destroy(empty);
        }

        // 생성
        //Instantiate(dummyDeck, contentPivot.transform);
        newDeckIndex++;
        contentIndex--;
        Debug.Log(contentIndex);

        if (maxDeckCount <= newDeckIndex)
        {
            newDeckButton.SetActive(false);
            return;
        }
    }

    public void PopDeck()
    {
        contentIndex++;
        newDeckIndex--;
        newDeckButton.SetActive(true);

        if (contentIndex < 0 || contentIndex > contents.Count)
            return;

        contents[contentIndex].SetActive(true);
        emptyStack.Push(contents[contentIndex]);
    }
}
