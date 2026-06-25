using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

public class BattleCardResourceData : MonoBehaviour
{
    public Image cardMask;
    public Image cardImage;
    public Image cardBackGround;

    public GameObject legandPortrait;

    public TextMeshProUGUI specialCardExplanation;
    public Image cardExplanationObject;
    public TextMeshProUGUI cardExplanation;

    public Image gem;
    public TextMeshProUGUI cost;

    public Image cardNameImage;
    public TextMeshProUGUI cardName;

    public Image typeImage;
    public TextMeshProUGUI typeImageName;

    public Image attackImage;
    public TextMeshProUGUI attack;

    public Image healthImage;
    public TextMeshProUGUI health;

    private CardData cardData;

    public uint GetCardUID() => cardData.cardId;

    public async UniTask<IBattleCard> SetUp(CardData _cardData)
    {
        // cardData를 받아와야 한다.
        cardData = _cardData;
        CardVisualHelper helper = new CardVisualHelper();
        var result = await helper.CardCategorySetting(cardData);

        CardvisualSetting(result);

        IBattleCard battleCard = CreateCardType(result.cardType);
        return battleCard;
    }

    // 차라리 token없이 그냥 흘러두는 게 더 나을 수도 있어. 괜히 삭제했다가 ref type이여서 돌다가 전부 사라질 수도 있어서.
    // 게다가 이거 지역변수로 받고 있네 그러면참조 하면 사라지는 거아닌가? 확인하ㅏ.

    public async UniTask<IBattleCard> SetUp(CardData _cardData, CancellationToken token)
    {
        // cardData를 받아와야 한다.
        cardData = _cardData;
        CardVisualHelper helper = new CardVisualHelper();
        var result = await helper.CardCategorySetting(cardData, token);

        CardvisualSetting(result);

        IBattleCard battleCard = CreateCardType(result.cardType);
        return battleCard;
    }

    private IBattleCard CreateCardType(CardType cardType)
    {
        IBattleCard battleCardType = null;
        switch(cardType)
        {
            case CardType.Minion:
                battleCardType = new MinionCard();
                break;
            case CardType.Spell:
                battleCardType = new SpellCard();
                break;
            case CardType.Weapon:
                battleCardType = new WeaponCard();
                break;
            default:
                battleCardType = null;
                break;
        }
        return battleCardType;
    }

    private void CardvisualSetting(CardVisualData visualData)
    {
        ActiveSetting(visualData.activeCard);

        CardPositionSetting(visualData.cardTransformSetting, visualData.activeCard.specialCardExplanation);
        
        // Sprite Setting
        for (int i = 0; i < visualData.sprites.Length; i++)
        {
            Sprite sprite = visualData.sprites[i];
            int index = visualData.indexList[i];
            SpriteCardSetting(sprite, index);
        }
    }

    private void ActiveSetting(ActiveCard activeCard)
    {
        legandPortrait.SetActive(activeCard.legandPortrait);
        specialCardExplanation.gameObject.SetActive(activeCard.specialCardExplanation);
        cardExplanationObject.gameObject.SetActive(activeCard.cardExplanation);
        gem.gameObject.SetActive(activeCard.gem);
        typeImage.gameObject.SetActive(activeCard.type);
        attackImage.gameObject.SetActive(activeCard.attack);
        healthImage.gameObject.SetActive(activeCard.health);
    }

    private void SpriteCardSetting(Sprite sprite, int index)
    {
        switch (index)
        {
            case 0:
                cardMask.sprite = sprite;
                break;
            case 1:
                cardImage.sprite = sprite;
                break;
            case 2:
                cardBackGround.sprite = sprite;
                break;
            case 3:
                cardExplanationObject.sprite = sprite;
                break;
            case 4:
                gem.sprite = sprite;
                break;
            case 5:
                cardNameImage.sprite = sprite;
                break;
            case 6:
                typeImage.sprite = sprite;
                break;
            case 7:
                attackImage.sprite = sprite;
                break;
            case 8:
                healthImage.sprite = sprite;
                break;
        }
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
                specialCardExplanation.text = cardData.description;
            }
            else
            {
                var explanationRect = cardExplanationObject.GetComponent<RectTransform>();
                explanationRect.anchoredPosition = setting.cardExplanation.position;
                explanationRect.sizeDelta = setting.cardExplanation.ratio;
                explanationRect.localScale = setting.cardExplanation.scale;
                cardExplanation.text = cardData.description;
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
            cardName.text = cardData.cardName;
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
            var cardTypeRect = typeImage.GetComponent<RectTransform>();
            cardTypeRect.anchoredPosition = setting.cardType.position;
            cardTypeRect.sizeDelta = setting.cardType.ratio;
            cardTypeRect.localScale = setting.cardType.scale;

            var cardTypeTextRect = typeImageName.GetComponent<RectTransform>();
            cardTypeTextRect.anchoredPosition = setting.cardTypeText.position;
            cardTypeTextRect.sizeDelta = setting.cardTypeText.ratio;
            cardTypeTextRect.localScale = setting.cardTypeText.scale;

            string typeName = cardData.cardTypes[0];
            for (int i = 1; i < cardData.cardTypes.Length; i++)
            {
                typeName += $"\n{cardData.cardTypes[i]}";
            }
            typeImageName.text = typeName;
        }

        if (!setting.isVoidValue(setting.attack))
        {
            var attackRect = attackImage.GetComponent<RectTransform>();
            attackRect.anchoredPosition = setting.attack.position;
            attackRect.sizeDelta = setting.attack.ratio;
            attackRect.localScale = setting.attack.scale;
            attack.text = cardData.attack.ToString();
        }

        if (!setting.isVoidValue(setting.attackText))
        {
            var attackRect = attack.GetComponent<RectTransform>();
            attackRect.anchoredPosition = setting.attackText.position;
            attackRect.sizeDelta = setting.attackText.ratio;
            attackRect.localScale = setting.attackText.scale;
        }

        if (!setting.isVoidValue(setting.health))
        {
            var healthRect = healthImage.GetComponent<RectTransform>();
            healthRect.anchoredPosition = setting.health.position;
            healthRect.sizeDelta = setting.health.ratio;
            healthRect.localScale = setting.health.scale;
            health.text = cardData.health.ToString();
        }

        if (!setting.isVoidValue(setting.healthText))
        {
            var healthRect = health.GetComponent<RectTransform>();
            healthRect.anchoredPosition = setting.healthText.position;
            healthRect.sizeDelta = setting.healthText.ratio;
            healthRect.localScale = setting.healthText.scale;
        }

        cost.text = cardData.cost.ToString();
    }
}
