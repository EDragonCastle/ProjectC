using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.EventSystems;

public class BattleCard : MonoBehaviour
{
    [SerializeField]
    private BattleCardController controller;
    [SerializeField]
    private BattleCardResourceData cardResource;

    private BattleCardAbilityData cardAbility;

    private IBattleCard cardType;

    public BattleCardResourceData GetResourceData() => cardResource;
    // Battle Ability Data
    public BattleCardAbilityData GetAbilityData() => cardAbility;
    public void SetAbilityData(BattleCardAbilityData input) => cardAbility = input;

    // Battle Card Type
    public void SetBattleCardType(IBattleCard type) => cardType = type;
    public IBattleCard GetBattleCardType() => cardType;
    // Battle Card Setup
    public void CardPositionSetup(Vector3 _defaultPosition, Quaternion _defaultRotation, int index) => controller.CardSetUp(_defaultPosition, _defaultRotation, index);
    // Battel Card Async Setup 
    public async UniTask CardPositionSetupAsync(Vector3 _defaultPosition, Quaternion _defaultRotation, int index) => await controller.CardPositionSetupAsync(_defaultPosition, _defaultRotation, index);
    public async UniTask CardPositionSetupAsync(Vector3 _defaultPosition, Quaternion _defaultRotation, int index, CancellationToken token) => await controller.CardPositionSetupAsync(_defaultPosition, _defaultRotation, index);
    // Pointer Enter
    public void OnPointerEnter(PointerEventData eventData) => controller.OnPointerEnter(eventData);
    // Card Scale
    public Vector3 GetCardOriginScale() => controller.GetCardOriginScale();
    // Card Touch Enable
    public void SetCardTouchEnable(bool input) => controller.SetCardTouchEnable(input);

    // Card Initalize Setting
    public async UniTask InitalizeCardSetup(CardData cardData)
    {
        cardType = await cardResource.SetUp(cardData);
        cardAbility = new BattleCardAbilityData();
        cardAbility.Setup(cardData);
    }

    public async UniTask InitalizeCardSetup(CardData cardData, CancellationToken token)
    {
        cardType = await cardResource.SetUp(cardData, token);
        cardAbility = new BattleCardAbilityData();
        cardAbility.Setup(cardData);
    }

}


// Initalize 부분에서 Ability도 같이 하자.
// 그럼 class 하나 만들어야 겠네 Ability class를 만드는게 낫겠다.