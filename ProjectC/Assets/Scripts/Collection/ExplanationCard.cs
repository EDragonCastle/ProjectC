using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class ExplanationCard : MonoBehaviour, IPointerDownHandler
{
    public GameObject origin;
    public GameObject card;

    public GameObject minion;
    public GameObject magic;

    [Header("Minion Card Data")]
    public Image cardImage;
    public Image cardBackGround;
    public TextMeshProUGUI cardExplanation;
    public Image gem;
    public GameObject legandPortrait;
    public TextMeshProUGUI cost;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI attack;
    public TextMeshProUGUI health;
    public GameObject type;
    public TextMeshProUGUI typeName;


    [Header("Magic Card Data")]
    public Image magicCardImage;
    public Image magicCardBackGround;
    public TextMeshProUGUI magicCardExplanation;
    public Image magicGem;
    public GameObject magicLegandPortrait;
    public TextMeshProUGUI magicCost;
    public TextMeshProUGUI magicCardName;
    public GameObject magicType;
    public TextMeshProUGUI magicTypeName;


    private CollectionCardData cardData;

    private Vector3 startPosition;
    private float duration = 0.5f;
    private bool isTweening = false;

    public void SetUp(Vector3 startPoint, CollectionCardData _cardData)
    {
        startPosition = startPoint;
        cardData = _cardData;
        CollectionCardDataSetting();
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

/* 
    private void CollectionCardDataSetting()
    {
        cardImage.sprite = cardData.cardImage.sprite;
        cardExplanation.text = cardData.cardExplanation.text;

        if(cardData.gem.gameObject.activeSelf) {
            gem.gameObject.SetActive(true);
            gem.sprite = cardData.gem.sprite;
        }
        else {
            gem.gameObject.SetActive(false);
        }

        if (cardData.legandPortrait.activeSelf)
            legandPortrait.SetActive(true);
        else
            legandPortrait.SetActive(false);

        cost.text = cardData.cost.text;
        cardName.text = cardData.cardName.text;
        attack.text = cardData.attack.text;
        health.text = cardData.health.text;
    }
 */

    private void CollectionCardDataSetting()
    {
        // minion or magic
        if (cardData.isMinion)
        {
            minion.gameObject.SetActive(true);
            magic.gameObject.SetActive(false);
            MinionSetting();
        }
        else
        {
            minion.gameObject.SetActive(false);
            magic.gameObject.SetActive(true);
            MagicSetting();
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

    private void MinionSetting()
    {
        cardImage.sprite = cardData.cardImage.sprite;
        cardBackGround.sprite = cardData.cardBackGround.sprite;
        cardExplanation.text = cardData.cardExplanation.text;
        if (cardData.isActiveGem)
        {
            gem.gameObject.SetActive(true);
            gem = cardData.gem;
        }
        else
            gem.gameObject.SetActive(false);

        if (cardData.legandPortrait.activeSelf)
            legandPortrait.SetActive(true);
        else
            legandPortrait.SetActive(false);
        cost.text = cardData.cost.text;
        cardName.text = cardData.cardName.text;
        attack.text = cardData.attack.text;
        health.text = cardData.health.text;
        if (cardData.isActiveType)
        {
            type.SetActive(true);
            typeName.text = cardData.cardTypeText.text;
        }
        else
        {
            type.SetActive(false);
        }
    }

    private void MagicSetting()
    {
        magicCardImage.sprite = cardData.cardImage.sprite;
        magicCardBackGround.sprite = cardData.cardBackGround.sprite;
        magicCardExplanation.text = cardData.cardExplanation.text;

        if (cardData.isActiveGem)
        {
            magicGem.gameObject.SetActive(true);
            magicGem = cardData.gem;
        }
        else
            magicGem.gameObject.SetActive(false);

        if (cardData.legandPortrait.activeSelf)
            magicLegandPortrait.SetActive(true);
        else
            magicLegandPortrait.SetActive(false);

        magicCost.text = cardData.cost.text;
        magicCardName.text = cardData.cardName.text;
        if (cardData.isActiveType)
        {
            magicType.SetActive(true);
            magicTypeName.text = cardData.cardTypeText.text;
        }
        else
        {
            magicType.SetActive(false);
        }
    }
}