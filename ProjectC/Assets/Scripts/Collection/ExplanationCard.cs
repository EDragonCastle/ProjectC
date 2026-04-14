using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class ExplanationCard : MonoBehaviour, IPointerDownHandler
{
    public GameObject origin;
    public GameObject card;

    [Header("Main Card Data")]
    public Image cardMask;
    public Image cardImage;
    public Image cardBackGround;

    public GameObject legandPortrait;

    public TextMeshProUGUI specialCardExplanation;
    public Image cardExplanationImage;
    public TextMeshProUGUI cardExplanation;

    public Image gem;
    public TextMeshProUGUI cost;

    public Image cardNameImage;
    public TextMeshProUGUI cardName;

    public Image type;
    public TextMeshProUGUI typeName;

    public Image attack;
    public TextMeshProUGUI attackName;

    public Image health;
    public TextMeshProUGUI healthName;

    private CollectionCardData cardData;

    private Vector3 startPosition;
    private float duration = 0.5f;
    private bool isTweening = false;

    public void SetUp(Vector3 startPoint, CollectionCardData _cardData)
    {
        startPosition = startPoint;
        cardData = _cardData;
        CollectionCardDataSetting();
        CardPositionSetting(cardData.cardTransformSetting, cardData.isSpecial);
        Opening();
    }

    // Panel에 설치할 예정이다.
    public void OnPointerDown(PointerEventData eventData)
    {
        if(!isTweening)
            Endding();
    }

    private void Opening()
    {
        var rectTransform = card.GetComponent<RectTransform>();
        rectTransform.position = startPosition;
        rectTransform.localScale = Vector3.one;
        isTweening = true;

        rectTransform.DOKill();

        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutBack));
        sequence.Join(rectTransform.DOScale(new Vector3(2f, 2f, 2f), duration).SetEase(Ease.OutBack));

        sequence.OnComplete(() => { isTweening = false; });
    }

    private void CollectionCardDataSetting()
    {
        // mask Image
        cardMask.sprite = cardData.maskImage.sprite;
        cardImage.sprite = cardData.cardImage.sprite;
        cardBackGround.sprite = cardData.cardBackGround.sprite;
        
        // Name
        cardNameImage.sprite = cardData.cardNameImage.sprite;
        cardName.text = cardData.cardName.text;

        // explanation Text
        if (cardData.isSpecial) {
            specialCardExplanation.text = cardData.cardExplanation.text;
        }
        else {
            cardExplanationImage.sprite = cardData.cardExplanationImage.sprite;
            cardExplanation.text = cardData.cardExplanation.text;
        }

        // Mana
        cost.text = cardData.cost.text;

        // Gem
        if(cardData.isActiveGem) {
            gem.gameObject.SetActive(true);
            gem.sprite = cardData.gem.sprite;
        }
        else {
            gem.gameObject.SetActive(false);
        }

        // Types
        if(cardData.isActiveType){
            type.gameObject.SetActive(true);
            type.sprite = cardData.typeImage.sprite;
            typeName.text = cardData.cardTypeText.text;
        }
        else {
            type.gameObject.SetActive(false);
        }

        // Attack
        if(cardData.isAttack) {
            attack.gameObject.SetActive(true);
            attack.sprite = cardData.attackImage.sprite;
            attackName.text = cardData.attack.text;
        }
        else {
            attack.gameObject.SetActive(false);
        }

        // health
        if(cardData.isHealth) {
            health.gameObject.SetActive(true);
            health.sprite = cardData.healthImage.sprite;
            healthName.text = cardData.health.text;
        }
        else {
            health.gameObject.SetActive(false);
        }
    }

    private void Endding()
    {
        var rectTransform = card.GetComponent<RectTransform>();

        rectTransform.DOKill();
        isTweening = true;
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOMove(startPosition, duration).SetEase(Ease.InBack));
        sequence.Join(rectTransform.DOScale(Vector3.one, duration).SetEase(Ease.InBack));

        sequence.OnComplete(() => {
            origin.SetActive(false); isTweening = false; });
    }


    private void CardPositionSetting(CardTransformSetting setting, bool isSpecial)
    {
        if (!setting.isVoidValue(setting.mask))
        {
            var maskRect = cardMask.GetComponent<RectTransform>();
            maskRect.anchoredPosition = setting.mask.position;
            maskRect.sizeDelta = setting.mask.ratio;
            maskRect.localScale = setting.mask.scale;
        }

        if (!setting.isVoidValue(setting.cardMainImage))
        {
            var mainImageRect = cardImage.GetComponent<RectTransform>();
            mainImageRect.anchoredPosition = setting.cardMainImage.position;
            mainImageRect.sizeDelta = setting.cardMainImage.ratio;
            mainImageRect.localScale = setting.cardMainImage.scale;
        }

        if (!setting.isVoidValue(setting.legandPortrait))
        {
            var legandRect = legandPortrait.GetComponent<RectTransform>();
            legandRect.anchoredPosition = setting.legandPortrait.position;
            legandRect.sizeDelta = setting.legandPortrait.ratio;
            legandRect.localScale = setting.legandPortrait.scale;
        }

        if (!setting.isVoidValue(setting.cardExplanation))
        {
            if (isSpecial)
            {
                var explanationRect = specialCardExplanation.GetComponent<RectTransform>();
                explanationRect.anchoredPosition = setting.cardExplanation.position;
                explanationRect.sizeDelta = setting.cardExplanation.ratio;
                explanationRect.localScale = setting.cardExplanation.scale;
            }
            else
            {
                var explanationRect = cardExplanationImage.GetComponent<RectTransform>();
                explanationRect.anchoredPosition = setting.cardExplanation.position;
                explanationRect.sizeDelta = setting.cardExplanation.ratio;
                explanationRect.localScale = setting.cardExplanation.scale;
            }
        }

        if (!setting.isVoidValue(setting.gem))
        {
            var gemRect = gem.GetComponent<RectTransform>();
            gemRect.anchoredPosition = setting.gem.position;
            gemRect.sizeDelta = setting.gem.ratio;
            gemRect.localScale = setting.gem.scale;
        }

        if (!setting.isVoidValue(setting.cardName))
        {
            var cardNameRect = cardNameImage.GetComponent<RectTransform>();
            cardNameRect.anchoredPosition = setting.cardName.position;
            cardNameRect.sizeDelta = setting.cardName.ratio;
            cardNameRect.localScale = setting.cardName.scale;
        }

        if (!setting.isVoidValue(setting.cardNameText))
        {
            var cardNameTextRect = cardName.GetComponent<RectTransform>();
            cardNameTextRect.anchoredPosition = setting.cardNameText.position;
            cardNameTextRect.sizeDelta = setting.cardNameText.ratio;
            cardNameTextRect.localScale = setting.cardNameText.scale;
        }

        if (!setting.isVoidValue(setting.cardType))
        {
            var cardTypeRect = type.GetComponent<RectTransform>();
            cardTypeRect.anchoredPosition = setting.cardType.position;
            cardTypeRect.sizeDelta = setting.cardType.ratio;
            cardTypeRect.localScale = setting.cardType.scale;

            var cardTypeTextRect = typeName.GetComponent<RectTransform>();
            cardTypeTextRect.anchoredPosition = setting.cardTypeText.position;
            cardTypeTextRect.sizeDelta = setting.cardTypeText.ratio;
            cardTypeTextRect.localScale = setting.cardTypeText.scale;

            //string typeName = cardData.cardTypeText.text;
        }

        if (!setting.isVoidValue(setting.attack))
        {
            var attackRect = attack.GetComponent<RectTransform>();
            attackRect.anchoredPosition = setting.attack.position;
            attackRect.sizeDelta = setting.attack.ratio;
            attackRect.localScale = setting.attack.scale;
        }

        if(!setting.isVoidValue(setting.attackText))
        {
            var attackRect = attackName.GetComponent<RectTransform>();
            attackRect.anchoredPosition = setting.attackText.position;
            attackRect.sizeDelta = setting.attackText.ratio;
            attackRect.localScale = setting.attackText.scale;
        }

        if (!setting.isVoidValue(setting.health))
        {
            var healthRect = health.GetComponent<RectTransform>();
            healthRect.anchoredPosition = setting.health.position;
            healthRect.sizeDelta = setting.health.ratio;
            healthRect.localScale = setting.health.scale;
        }

        if (!setting.isVoidValue(setting.healthText))
        {
            var healthRect = healthName.GetComponent<RectTransform>();
            healthRect.anchoredPosition = setting.healthText.position;
            healthRect.sizeDelta = setting.healthText.ratio;
            healthRect.localScale = setting.healthText.scale;
        }
    }
}