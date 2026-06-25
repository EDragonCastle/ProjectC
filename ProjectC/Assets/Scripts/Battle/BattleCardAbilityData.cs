using System.Collections.Generic;


// 지금 현재 이 Abitity Data는 원석이나 다름없다. 가공해서 사용해야 하는데?
// 위에서 전투의 함성 처리 
// Effect 처리를 위한 추가 작업을 해줘야 하나?
/// 이건 Visual Helper 사용해서 Resource 만들 때 Ability Setting도 같이 했다.

public class BattleCardAbilityData
{
    private List<CardAbilityData> abilityDatas;
    private string[] cardType;
    public List<CardAbilityData> GetCardAbilityDatas() => abilityDatas;
    public string[] GetCardTypes() => cardType;

    public void Setup(CardData _cardData)
    {
        // cardData를 받아와서 UID를 사용해야 한다.
        uint cardId = _cardData.cardId;

        // Helper를 거치면 위에 있는 Data들이 채워져야 한다.
        CardAbilityHelper helper = new CardAbilityHelper();
        abilityDatas = helper.CardAbilitySetting(cardId);
        cardType = helper.CardTypes(cardId);
    }
}


