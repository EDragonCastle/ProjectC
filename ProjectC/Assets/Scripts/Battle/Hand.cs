using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.EventSystems;

public class Hand : MonoBehaviour, IChannel
{
    // 이 클래스는 카드 관리하는 공간이다.
    public GameObject testPrefab;

    [Header("Layout Settings")]
    public float cardSpacing = 100f;
    public float arcIntensity = 10f;
    public float angleIntensity = 5f;

    private List<BattleCard> cards;
    private RectTransform handRect;

    private BattleCardInformation startDrawCardInformation;
    private BattleCardInformation endDrawCardInformation;

    private int maxHandCard = 10;

    private Vector3 midDrawPosition = new Vector3(363, 344, 0);
    private Vector3 midDrawScale = new Vector3(3, 3, 3);

    private float midDuration = 0.5f;
    private float endDuration = 0.2f;
    private Ease ease = Ease.Unset;

    private bool isDrawing = false;

    private async void Start()
    {
        await UniTask.WaitUntil(() => GameManager.isReadyGameManager);

        var battleManager = Locator<BattleManager>.Get();
        battleManager.SetHandParent(this.gameObject);

        handRect = this.gameObject.GetComponent<RectTransform>();
        endDrawCardInformation = new BattleCardInformation();
        cards = new List<BattleCard>();
    }

    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.BattleDeckListPosition, HandleEvent);
        eventManager.Subscription(ChannelInfo.UsingBattleCard, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.BattleDeckListPosition, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.UsingBattleCard, HandleEvent);
    }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            await DrawCard(this.GetCancellationTokenOnDestroy());
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.BattleDeckListPosition:
                if(information is BattleCardInformation cardInfo)
                {
                    startDrawCardInformation = cardInfo;
                }
                break;
            case ChannelInfo.UsingBattleCard:
                if(information is int cardIndex)
                {
                    // 다른 카드들을 잠시 잠궈야 하는데?
                    cards.RemoveAt(cardIndex);
                    CardLayout(this.GetCancellationTokenOnDestroy()).Forget();
                    CardTouchEnable(false);
                }
                break;
        }
    }


    // 카드들을 해야 하는데? 추천 방식은 async 방식을 추천해줬다.
    private async UniTask DrawCard(CancellationToken token)
    { 
        if (isDrawing)
            return;

        isDrawing = true;

        var battleManager = Locator<BattleManager>.Get();
        var handParent = battleManager.GetHandParent();

        // 여기가 GameObject를 생성이자 카드 드로우다.
        var clone = GameObject.Instantiate(testPrefab, handParent.transform);

        List<UniTask> drawTasks = new List<UniTask>();

        // 먼저 카드가 이동한다.
        drawTasks.Add(CardLayout(token));

        var cloneBattleComponent = clone.GetComponent<BattleCard>();
        cards.Add(cloneBattleComponent);

        // 해당 clone 카드가 Draw Logic을 거쳐야 한다.
        var cloneRect = clone.GetComponent<RectTransform>();
        cloneRect.position = startDrawCardInformation.position;
        cloneRect.rotation = startDrawCardInformation.rotation;

        cloneBattleComponent.SetCardTouchEnable(true);

        // Draw 연출
        DG.Tweening.Sequence midSequence = DOTween.Sequence();

        await midSequence.Append(cloneRect.DOLocalMove(midDrawPosition, midDuration)).SetEase(ease)
                     .Join(cloneRect.DORotateQuaternion(Quaternion.identity, midDuration)).SetEase(ease)
                     .Join(cloneRect.DOScale(midDrawScale, midDuration)).SetEase(ease)
                     .AppendInterval(0.1f);

        int count = cards.Count;

        // 손에 있는 카드 개수에 따라 어떻게 동작할지 달라진다.
        var endSequence = DOTween.Sequence();
        if(count < maxHandCard)
        {
            var task = endSequence.Append(cloneRect.DOLocalMove(endDrawCardInformation.position, endDuration)).SetEase(ease)
                                   .Join(cloneRect.DORotateQuaternion(endDrawCardInformation.rotation, endDuration)).SetEase(ease)
                                   .Join(cloneRect.DOScale(cloneBattleComponent.GetCardOriginScale(), endDuration).SetEase(ease));

            drawTasks.Add(task.ToUniTask());
            drawTasks.Add(cloneBattleComponent.CardSetUpAsync(endDrawCardInformation.position, endDrawCardInformation.rotation, count - 1));
        }
        else
        {
            // 삭제 로직
            // 필요시 Append를 추가해서 뭔가를 한다.
            var task = endSequence.OnComplete(() => {
                cards.Remove(cloneBattleComponent);
                Destroy(clone);
            });
            drawTasks.Add(task.ToUniTask());
        }

        await UniTask.WhenAll(drawTasks);

        CardTouchEnable(false);
        isDrawing = false;
        AfterDrawCheckMousePointer();
    }

    // card 이동까지 마치면 그 때 Enable을 하면 될 것 같은데
    private async UniTask CardLayout(CancellationToken token)
    {
        int count = cards.Count;

        count++;

        float midIndex = (count - 1) / 2f;
        float maxWidth = handRect.sizeDelta.x;
        float currentSpacing = cardSpacing;

        if(count * cardSpacing > maxWidth)
            currentSpacing = maxWidth / count;    

        List<UniTask> moveTasks = new List<UniTask>();

        for (int i = 0; i < count; i++)
        {
            float index = i - midIndex;

            float xPosition = index * currentSpacing;
            float yPosition = -Mathf.Pow(index, 2) * arcIntensity;
            float zRotation = index * -angleIntensity;

            Vector3 targetPosition = new Vector3(xPosition, yPosition, 0);
            Quaternion targetRotation = Quaternion.Euler(0, 0, zRotation);

            // 매번 하는게 그렇긴해.
            if(i == (count - 1)) {
                endDrawCardInformation.position = targetPosition;
                endDrawCardInformation.rotation = targetRotation;
            }

            if (i != count - 1)
            {
                cards[i].SetCardTouchEnable(true);
                moveTasks.Add(cards[i].CardSetUpAsync(targetPosition, targetRotation, i, token));
            }
        }

        if (moveTasks.Count > 0)
            await UniTask.WhenAll(moveTasks).AttachExternalCancellation(token);
    }


    private void CardTouchEnable(bool enable)
    {
        foreach(var card in cards)
        {
            card.SetCardTouchEnable(enable);
        }
    }

    private void AfterDrawCheckMousePointer()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerData, results);

        foreach(var result in results)
        {
            var card = result.gameObject.GetComponent<BattleCardController>();

            if(card != null)
            {
                card.OnPointerEnter(pointerData);
                return;
            }
        }
    }

    // 카드 드래그 하고 있던 것들을 전부 원래대로 되돌린다?
}

// 지금 있을 버그 혀낭은 Draw를 연속으로 하면 문제가 생긴다.
// Draw를 연속으로 하지 못하게 막아야 한다.
