using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;

public class HeroSelect : MonoBehaviour
{
    public Sprite[] heros;
    public Sprite[] heroIcons;
    public Sprite[] heroPowers;

    public Sprite select;
    public Sprite noneSelect;

    // Choice 할 때 생성되는 영웅
    public GameObject choiceHero;
    public GameObject heroExplantion;
    public GameObject selectObject;

    public TextMeshProUGUI selectText;

    private Image selectImage;
    private Button selectButton;
    
    public Image heroImage;
    public Image heroPower;
    public TextMeshProUGUI heroName;

    public Image heroExplantionImage;
    public TextMeshProUGUI heroPowerName;
    public TextMeshProUGUI heroPowerExplantion;

    public Vector3 selectScale;
    public Vector3 noneSelectScale;

    private Color selectTextColor;

    private void Start()
    {
        choiceHero.SetActive(false);
        heroExplantion.SetActive(false);

        selectImage = selectObject.GetComponent<Image>();
        selectButton = selectObject.GetComponent<Button>();

        selectImage.sprite = noneSelect;
        selectImage.rectTransform.localScale = noneSelectScale;
        selectTextColor = selectText.color;
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        selectImage.sprite = noneSelect;
        choiceHero.SetActive(false);
        heroExplantion.SetActive(false);
        selectButton.enabled = false;
        selectImage.rectTransform.localScale = noneSelectScale;
        selectText.color = selectTextColor;
    }

    public async void ChoiceHeroButton(int index)
    {
        await LoadingHero(index);

        choiceHero.SetActive(true);
        selectButton.enabled = true;
    }

    public void IsActvieHeroExplantion(bool isActive)
    {
        heroExplantion.SetActive(isActive);
    }

    public void SelectingHero()
    {
        Debug.Log("Selecting Hero");
    }

    private async UniTask LoadingHero(int index)
    {
        var dataManager = Locator<DataManager>.Get();
        var heroDataDict = dataManager.GetHeroData();

        if (index >= heroDataDict.Count || index < 0)
            index = 0;

        var resourceManager = Locator<ResourceManager>.Get();

        HeroData heroData = heroDataDict[(uint)index + 100];

        dataManager.SetHeroIndex((uint)index + 100);

        var heroSpriteTask = resourceManager.Get<Sprite>(heroData.heroSprite);
        var heroPowerSpriteTask = resourceManager.Get<Sprite>(heroData.heroPowerSprite);

        var (heroSprite, heroPowerSprite) = await UniTask.WhenAll(heroSpriteTask, heroPowerSpriteTask);

        heroImage.sprite = heroSprite;
        heroPower.sprite = heroPowerSprite;
        heroExplantionImage.sprite = heroPowerSprite;

        heroName.text = heroData.heroName;
        heroPowerName.text = heroData.heroPowerName;
        heroPowerExplantion.text = heroData.heroPowerExplanation;

        selectImage.rectTransform.localScale = selectScale;
        selectText.color = Color.white;
    }

}
