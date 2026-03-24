using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Filter : MonoBehaviour
{
    [Header("Filter Outer")]
    public GameObject origin;

    [Header("Fliter Inner")]
    public GameObject filterWindow;
    public GameObject manaCostList;
    public GameObject deleteButton;

    public ManaCost[] manaList;

    public RectTransform filterSearchRectTransform;

    public Ease ease;
    
    public GameObject nameField;
    public TextMeshProUGUI searchText;
    public float duration = 0.5f;

    // Magic Number -> Search Image Width다.
    private float manaInitPos;

    private void Start()
    {
        deleteButton.SetActive(false);
    }

    private void OnEnable()
    {
        if (filterWindow == null || manaCostList == null)
            return;

        Opening();
    }

    public void StartFilter()
    {
        origin.SetActive(true);
    }
    
    /// <summary>
    /// Search Button을 눌렀을 때 작동
    /// </summary>
    public void Searching()
    {
        nameField.SetActive(true);
        searchText.gameObject.SetActive(false);
        var inputComponent = nameField.GetComponent<TMP_InputField>();
        inputComponent.interactable = true;

        inputComponent.Select();
        inputComponent.ActivateInputField();
    }

    /// <summary>
    /// Input Field 마지막에 적용될 내용
    /// </summary>
    public void EnterSearchText()
    {
        var inputComponent = nameField.GetComponent<TMP_InputField>();

        if(string.IsNullOrWhiteSpace(inputComponent.text))
        {
            inputComponent.text = "";
            searchText.text = "검색";
            deleteButton.SetActive(false);
        }
        else
        {
            searchText.text = inputComponent.text.Trim();

            var eventManager = Locator<EventManager>.Get();
            FilterParameter parameter = new FilterParameter(FilterType.Search, _search: searchText.text);
            eventManager.Notify(ChannelInfo.Filter, parameter);

            inputComponent.text = "";

            // Filter 작동과 Delete Object 활성화
            deleteButton.SetActive(true);
        }

        nameField.SetActive(false);
        searchText.gameObject.SetActive(true);
    }

    // Mana를 Click 했을 때 작동되는 것
    public void SelectMana(int mana)
    {
        int current = 0;
        foreach(var manaArray in manaList)
        {
            if (current == mana)
                manaArray.Select();
            else
                manaArray.NoneSelect();

            current++;
        }

        var eventManager = Locator<EventManager>.Get();
        FilterParameter parameter = new FilterParameter(FilterType.Search, _cost: mana);
        eventManager.Notify(ChannelInfo.Filter, parameter);
    }


    public void NoneSelectMana()
    {
        // parameter로 int를 넣을까? 
        foreach (var manaArray in manaList)
        {
            manaArray.NoneSelect();
        }

        var eventManager = Locator<EventManager>.Get();
        FilterParameter parameter = new FilterParameter(FilterType.Search, _cost: -1);
        eventManager.Notify(ChannelInfo.Filter, parameter);
    }



    // Filter Complete를 클릭하면 된다.
    public void FilterComplete()
    {
        Ending();
    }

    /// <summary>
    /// Delete Button 눌렀을 때 실행되는 methord
    /// </summary>
    public void DeleteSearch()
    {
        var inputComponent = nameField.GetComponent<TMP_InputField>();
        inputComponent.text = "";
        searchText.text = "검색";
        deleteButton.SetActive(false);

        // Filter 작동
        var eventManager = Locator<EventManager>.Get();
        FilterParameter parameter = new FilterParameter(FilterType.Search, _search: "");
        eventManager.Notify(ChannelInfo.Filter, parameter);
    }

    private void Opening()
    {
        var filterRectTransform = filterWindow.GetComponent<RectTransform>();
        var manaRectTransform = manaCostList.GetComponent<RectTransform>();
        manaInitPos = filterSearchRectTransform.rect.width;

        float start = manaRectTransform.rect.height / 2;
        float end = -manaRectTransform.rect.height / 2;

        // init 위치
        filterRectTransform.anchoredPosition = new Vector2(manaInitPos, 0);
        manaRectTransform.anchoredPosition = new Vector2(manaRectTransform.anchoredPosition.x, start);

        filterRectTransform.DOKill();
        manaRectTransform.DOKill();

        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(filterRectTransform.DOAnchorPosX(0, duration).SetEase(ease))
                .Join(manaRectTransform.DOAnchorPosY(end, duration).SetEase(ease))
                .OnComplete(() => {
                    manaRectTransform.anchoredPosition = new Vector2(manaRectTransform.anchoredPosition.x, end);
                    filterRectTransform.anchoredPosition = new Vector2(0, 0);
                });
    }

    private void Ending()
    {
        var filterRectTransform = filterWindow.GetComponent<RectTransform>();
        var manaRectTransform = manaCostList.GetComponent<RectTransform>();
        manaInitPos = filterSearchRectTransform.rect.width;

        float start = -manaRectTransform.rect.height / 2;
        float end = manaRectTransform.rect.height / 2;

        // init 위치
        filterRectTransform.anchoredPosition = new Vector2(0, 0);
        manaRectTransform.anchoredPosition = new Vector2(manaRectTransform.anchoredPosition.x, start);

        filterRectTransform.DOKill();
        manaRectTransform.DOKill();

        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(filterRectTransform.DOAnchorPosX(manaInitPos, duration).SetEase(ease))
                .Join(manaRectTransform.DOAnchorPosY(end, duration).SetEase(ease))
                .OnComplete(() => {
                    manaRectTransform.anchoredPosition = new Vector2(manaRectTransform.anchoredPosition.x, end);
                    filterRectTransform.anchoredPosition = new Vector2(manaInitPos, 0);
                    this.gameObject.SetActive(false);
                });
    }
}


// 생각좀 해볼까
// Button에 작동할거다. on off 기능을 제작할건데
// 그러면 배열을 두개로 만들어야 한다고? 차라리 Script 배열이 낫지 않을까?
