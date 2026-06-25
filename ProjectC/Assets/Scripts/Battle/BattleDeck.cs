using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class BattleDeck : MonoBehaviour
{
    public Image deckImage;
    public TextMeshProUGUI deckName;
    public GameObject cardNumber;
    public TextMeshProUGUI requireCardText;

    private DeckInformation deckInfo;
    private bool isMaxCard = false;
    

    /// <summary>
    /// Deck을 클릭했을 때 실행되는 함수
    /// </summary>
    public async void OnClickDeck()
    {
        // 부족하면 return 한다.
        if (!isMaxCard)
            return;

        // Button을 눌렀을 때 선택창이 나와야 한다.
        var dataManager = Locator<DataManager>.Get();

        var heroDataList = dataManager.GetHeroData();
        var heroData = heroDataList[deckInfo.heroIndex];
        var resourceManager = Locator<ResourceManager>.Get();

        var result = await resourceManager.Get<Sprite>(heroData.heroPowerSprite);

        var battleInfo = new BattleInformation();
        battleInfo.deckName = deckName.text;
        battleInfo.heroImage = deckImage.sprite;
        battleInfo.heroPowerImage = result;
        battleInfo.heroPowerExplanation = heroData.heroPowerExplanation;
        battleInfo.heroPowerName = heroData.heroPowerName;
        battleInfo.deckData = deckInfo.deckData;

        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.SelectBattleHero, battleInfo);
    }

    public void DeckSetting(int index)
    {
        // DeckData를 어떤 식으로 받아와야 할까?
        var dataManager = Locator<DataManager>.Get();
        var deckList = dataManager.GetBattleDeckList();

        if (deckList.Count <= index)
        {
            this.gameObject.SetActive(false);
            return;
        }

        deckInfo = deckList[index];

        this.gameObject.SetActive(true);
        deckImage.sprite = deckInfo.deckImage;
        deckName.text = deckInfo.deckName;

        if(deckInfo.currentCard != deckInfo.maxCard) {
            cardNumber.SetActive(true);
            requireCardText.text = $"{deckInfo.currentCard}/{deckInfo.maxCard}";
            isMaxCard = false;
        }
        else {
            cardNumber.SetActive(false);
            isMaxCard = true;
        }
    }

}

public struct BattleInformation
{
    public string deckName;
    public Sprite heroImage;
    public Sprite heroPowerImage;
    public string heroPowerName;
    public string heroPowerExplanation;
    public List<DeckData> deckData;
}