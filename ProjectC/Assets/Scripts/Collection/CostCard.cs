using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class CostCard : MonoBehaviour
{
    public Image heroPowerImage;
    public TextMeshProUGUI heroExplanationText;
    public RectTransform[] costBar;
    public TextMeshProUGUI[] costText;
    public uint heroIndex;
    public List<DeckData> deckList;

    private readonly float empty = -131;
    private readonly float barTexture = 78f;


    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public async UniTask CardSetup(List<DeckData> _deckList)
    {
        deckList = _deckList;
        DeckToCardBar();
        var resourceManager = Locator<ResourceManager>.Get();
        var dataManager = Locator<DataManager>.Get();
        var heroDatas = dataManager.GetHeroData();
        var heroData = heroDatas[heroIndex];
        string explanation = heroData.heroPowerExplanation.Replace("\\n", "\n");
        heroExplanationText.text = explanation;
        var heroExplanationSprite = await resourceManager.Get<Sprite>(heroData.heroPowerSprite);

        heroPowerImage.sprite = heroExplanationSprite;
    }

    private void DeckToCardBar()
    {
        int[] costCounts = new int[8];

        foreach(var deckData in deckList)
        {
            int index = Mathf.Min(deckData.cardData.cost, 7);
            costCounts[index] += deckData.count;
        }

        int maxCount = 0;
        foreach(int count in costCounts)
        {
            if (count > maxCount)
                maxCount = count;
        }

        for(int i = 0; i < costBar.Length; i++)
        {
            float ratio = 0;
            
            if(maxCount > 0)
                ratio = (float)costCounts[i] / maxCount;

            float reSizeY = empty + (Mathf.Abs(empty) * ratio);

            if(reSizeY == 0.0f)
                reSizeY -= 20f;

            costBar[i].anchoredPosition = new Vector2(costBar[i].anchoredPosition.x, reSizeY);
            costText[i].rectTransform.anchoredPosition = new Vector2(costBar[i].anchoredPosition.x, costBar[i].anchoredPosition.y + barTexture);
            costText[i].text = costCounts[i] > 0 ? costCounts[i].ToString() : "0";
        }
        
    }

    
}
