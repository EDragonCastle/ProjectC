using UnityEngine;
using UnityEngine.UI;
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

    // 영웅 선택을 눌렀을 때 나오는 결과
    public void ChoiceHero(int index)
    {
        Debug.Log($"Choice Hero {index}");

        // heroPortrait에서 heroImage와 heroIcon과 Text와 Hero Power를 교체한다.
        Hero(index);

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

    private void Hero(int index)
    {
        if (index >= heros.Length || index < 0)
            index = 0;

        selectImage.sprite = select;
        selectImage.rectTransform.localScale = selectScale;
        heroImage.sprite = heros[index];
        heroPower.sprite = heroPowers[index];
        heroExplantionImage.sprite = heroPowers[index];
        selectText.color = Color.white;

        switch (index)
        {
            case 0:
                heroName.text = "가로쉬";
                heroPowerName.text = "방어도 증가";
                heroPowerExplantion.text = "방어도 2를 얻습니다.";
                break;
            case 1:
                heroName.text = "안두인";
                heroPowerName.text = "생명력 회복";
                heroPowerExplantion.text = "체력 2를 회복합니다.";
                break;
            case 2:
                heroName.text = "렉사르";
                heroPowerName.text = "영웅 사격";
                heroPowerExplantion.text = "적 영웅에게 2의 피해를 줍니다.";
                break;
            case 3:
                heroName.text = "말퓨리온";
                heroPowerName.text = "방어도 증가";
                heroPowerExplantion.text = "공격력 1과 방어도 1을 얻습니다.";
                break;
            case 4:
                heroName.text = "제이나";
                heroPowerName.text = "화염작렬";
                heroPowerExplantion.text = "피해 1를 줍니다.";
                break;
            case 5:
                heroName.text = "우서";
                heroPowerName.text = "신병 모집";
                heroPowerExplantion.text = "신병을 소환합니다.";
                break;
            case 6:
                heroName.text = "발리라";
                heroPowerName.text = "단검 포착";
                heroPowerExplantion.text = "1/2 무기를 착용합니다.";
                break;
            case 7:
                heroName.text = "스랄";
                heroPowerName.text = "토템 소환";
                heroPowerExplantion.text = "무작위 토템을 소환합니다.";
                break;
            case 8:
                heroName.text = "굴단";
                heroPowerName.text = "생명력 착취";
                heroPowerExplantion.text = "내 영웅이 피해 2를 입습니다.\n 카드 한 장을 뽑습니다.";
                break;
            default:
                break;
        }
    }
}
