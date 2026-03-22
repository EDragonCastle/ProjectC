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

    public RectTransform filterSearchRectTransform;

    public Ease ease;
    
    public GameObject nameField;
    public TextMeshProUGUI searchText;
    public float duration = 0.5f;

    // Magic Number -> Search Image Width다.
    private float manaInitPos;

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

        Debug.Log("Search Input Mode 시작");
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
        }
        else
        {
            inputComponent.text = "";
            searchText.text = inputComponent.text.Trim();
        }

        nameField.SetActive(false);
        searchText.gameObject.SetActive(true);
    }

    // Filter Complete를 클릭하면 된다.
    public void FilterComplete()
    {
        Ending();
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
