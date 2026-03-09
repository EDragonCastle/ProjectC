using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// Excel CSV 파일을 Parser해서 Data만 추출한다.
/// </summary>
public class ParserManager
{
    private Dictionary<uint, CardData> cardTable;
    private Dictionary<uint, HeroData> heroTable;
    
    public ParserManager()
    {
        cardTable = new Dictionary<uint, CardData>();
        heroTable = new Dictionary<uint, HeroData>();
    }

    public async UniTask Initalize() 
    {
        await LoadAndParserCSVData();
        Debug.Log("Data Parser Complete And Game Setting Start!");
    }

    public Dictionary<uint, CardData> GetCardTable() => cardTable;
    public Dictionary<uint, HeroData> GetHeroTable() => heroTable;

    private async UniTask LoadAndParserCSVData()
    {
        // 여러 개의 일을 하고 싶을 때는 어떻게 해야할까?
        var HeroDataHandle = Addressables.LoadAssetAsync<TextAsset>("HeroData").ToUniTask();
        var CardDataHandle = Addressables.LoadAssetAsync<TextAsset>("CardData").ToUniTask();
    
        try {
            var result = await UniTask.WhenAll(HeroDataHandle, CardDataHandle);

            TextAsset heroCSV = result.Item1;
            TextAsset cardCSV = result.Item2;

            if (cardCSV != null)
                CardParserCSV(cardCSV.text);
            if (heroCSV != null)
                HeroParserCSV(heroCSV.text);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Address Asset Loading Error : {e.Message}");
        }
    }

    private void CardParserCSV(string data)
    {
        Debug.Log("Parsing CSV File");

        string[] lines = data.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        for(int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) 
                continue;

            string[] row = lines[i].Split(',');

            var card = ScriptableObject.CreateInstance<CardData>();

            card.cardId = uint.Parse(row[0].Trim());
            card.cardName = row[1].Trim();
            card.cost = int.Parse(row[2].Trim());
            card.attack = int.Parse(row[3].Trim());
            card.health = int.Parse(row[4].Trim());
            card.description = row[5].Trim();
            card.spriteName = row[6].Trim();
            card.gem = row[7].Trim();

            cardTable.Add(card.cardId, card);
        }
    }

    private void HeroParserCSV(string heroData)
    {
        string[] lines = heroData.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        for(int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] row = lines[i].Split(',');

            var hero = ScriptableObject.CreateInstance<HeroData>();

            hero.heroId = uint.Parse(row[0]);
            hero.heroName = row[1].Trim();
            hero.heroSprite = row[2].Trim();
            hero.heroPowerName = row[3].Trim();
            hero.heroPowerSprite = row[4].Trim();
            hero.heroPowerExplanation = row[5].Trim();
            hero.heroPowerIconSprite = row[6].Trim();

            heroTable.Add(hero.heroId, hero);
        }
    }

}


