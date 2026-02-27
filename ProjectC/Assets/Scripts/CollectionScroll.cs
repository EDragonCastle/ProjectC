using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class CollectionScroll : MonoBehaviour
{
    public GameObject contentPivot;
    public GameObject dummyDeck;
    public GameObject newDeckButton;

    public GameObject newDeckListObject;

    public int maxDeckCount;

    // dummy object
    public List<GameObject> contents = new List<GameObject>();

    public float openPosition = 800f;

    public GameObject createDeck;
    public RectTransform leftDeckList;
    public RectTransform rightDeckList;

    private int newDeckIndex;
    private Vector2 leftOriginPos;
    private Vector2 rightOriginPos;
    private float duration = 1.0f;

    private Stack<GameObject> emptyStack = new Stack<GameObject>();

    private void Start()
    {
        leftOriginPos = leftDeckList.anchoredPosition;
        rightOriginPos = rightDeckList.anchoredPosition;
        newDeckListObject.SetActive(false);

        foreach (var obj in contents)
        {
            emptyStack.Push(obj);
        }

        newDeckIndex = 0;
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

        sequence.OnComplete(() => {
            createDeck.SetActive(false);
        });
    }

    public void SelectDeck()
    {
        CloseDeck();
        // NewDeck List가 활성화되어야 할 것 같고, Scroll Deck은 비활성화 되어야 해.
        newDeckListObject.SetActive(true);
        this.gameObject.SetActive(false);
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
            Destroy(empty);
        }

        // 생성
        Instantiate(dummyDeck, contentPivot.transform);
        newDeckIndex++;

        if (maxDeckCount <= newDeckIndex)
        {
            newDeckButton.SetActive(false);
            return;
        }
    }

}
