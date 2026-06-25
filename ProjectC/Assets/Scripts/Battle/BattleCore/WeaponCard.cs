using UnityEngine;
using Cysharp.Threading.Tasks;

public class WeaponCard : IWeapon, IChannel
{
    private GameObject spawnObject;

    public async UniTask Execute(BattleFieldObjectInformation battleCard)
    {
        // Weapon의 경우 무조건 생성이 된다.
        // 여기도 Entity를 통해 얻어가야 하나?
        var resourceManager = Locator<ResourceManager>.Get();
        var battleManager = Locator<BattleManager>.Get();

        var battleComponent = battleCard.card.GetComponent<BattleCard>();
        var battleCardResource = battleComponent.GetResourceData();

        MinionResourceData weaponResource = new MinionResourceData();
        weaponResource.cardImage = battleCardResource.cardImage.sprite;
        weaponResource.legand = battleCardResource.legandPortrait.activeSelf;
        weaponResource.attack = battleCardResource.attack.text;
        weaponResource.health = battleCardResource.health.text;

        var weapon = await resourceManager.Get<GameObject>("Weapon Entity");
        var battleField = battleManager.GetBattleField();
        var battleFieldComponent = battleField.GetComponent<BattleField>();

        var weaponEntity = weapon.GetComponent<WeaponEntity>();
        weaponEntity.ResourceSetting(weaponResource);
        weaponEntity.card = battleComponent;

        spawnObject = battleFieldComponent.PlaceWeapon(weapon, battleCard.isPlayer);

        var newWeaponEntity = spawnObject.GetComponent<WeaponEntity>();
        newWeaponEntity.CreateWeapon();
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {

    }
}
