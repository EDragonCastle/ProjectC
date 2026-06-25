using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponEntity : MonoBehaviour
{
    [Header("Card Data")]
    public Image cardMask;
    public Image cardImage;
    public Image cardBackGround;

    public GameObject legandPortrait;

    public Image attack;
    public TextMeshProUGUI attackText;
    public Image health;
    public TextMeshProUGUI healthText;

    public BattleCard card;

    // Weapon Controller에서 할 수 있는건
    // 마우스 올리면 옵션 보기?
    // 해당 무기는 Target도 되지 않는다.

    public void ResourceSetting(MinionResourceData resourceData)
    {
        cardImage.sprite = resourceData.cardImage;
        legandPortrait.SetActive(resourceData.legand);
        attackText.text = resourceData.attack;
        healthText.text = resourceData.health;
    }

    public void DestroyWeapon()
    {
        // 무기 삭제 로직을 실행한다.
        Debug.Log("Weapon Destoy");
        Destroy(this.gameObject);
    }

    public void CreateWeapon()
    {
        // 무기 생성 로직을 실행한다.
        Debug.Log("Weapon Initalize");
    }
}
