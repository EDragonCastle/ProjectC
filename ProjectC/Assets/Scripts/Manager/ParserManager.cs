using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// Excel CSV 파일을 Parser해서 Data만 추출한다.
/// </summary>
public class ParserManager
{
    private Dictionary<uint, CardData> cardTable;
    private Dictionary<uint, CardData> spawnCardTable;
    private Dictionary<uint, HeroData> heroTable;
    
    public ParserManager()
    {
        cardTable = new Dictionary<uint, CardData>();
        spawnCardTable = new Dictionary<uint, CardData>();
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

        string[] lines = Regex.Split(data, @"\r\n(?=(?:[^""]*""[^""]*"")*[^""]*$)");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) 
                continue;

            string[] row = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            if (string.IsNullOrEmpty(row[0])) 
                continue;

            var card = ScriptableObject.CreateInstance<CardData>();

            card.cardId = SafetyParser<uint>(row[0].Trim());
            card.cardName = row[1].Trim();
            card.cost = SafetyParser<int>(row[2].Trim()); 
            card.attack = SafetyParser<int>(row[3].Trim());
            card.health = SafetyParser<int>(row[4].Trim());
            card.description = row[5].Trim().Replace("\\r\\n", "\n").Replace("\\n", "\n").Replace("\r\n", "\n");
            card.spriteName = row[6].Trim();
            card.gem = row[7].Trim();
            card.isMinion = row[8].Trim() == "Minion" ? true : false;
            card.jobType = row[9].Trim();
            card.packgeType = row[10].Trim();

            if (row[8].Trim() == "Minion")
                card.cardType = row[11].Trim();
            else
                card.cardType = row[12].Trim();

            if (!string.IsNullOrWhiteSpace(row[13]))
            {
                string spawnNumberWord = row[13].Replace("\"", "").Trim();
                string[] spawnIds = spawnNumberWord.Split(',');
                card.spawn = new uint[spawnIds.Length];

                for (int j = 0; j < spawnIds.Length; j++)
                {
                    card.spawn[j] = SafetyParser<uint>(spawnIds[j]);
                }
            }
            else
                card.spawn = new uint[0];

            card.posX = SafetyParser<float>(row[14]);
            card.posY = SafetyParser<float>(row[15]);
            card.rotation = SafetyParser<float>(row[16]);

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
            hero.heroDeckName = row[7].Trim();

            heroTable.Add(hero.heroId, hero);
        }
    }

    private T SafetyParser<T>(string number) where T : struct
    {
        // 1. 빈 값 처리
        if (string.IsNullOrWhiteSpace(number)) return default(T);

        // 2. 따옴표 및 공백 제거
        string cleanValue = number.Replace("\"", "").Trim();

        try
        {
            // 3. TypeDescriptor를 이용한 변환 (int, float, uint, bool 등 대부분 지원)
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null) {
                return (T)converter.ConvertFromString(cleanValue);
            }
        }
        catch
        {
            // 변환 실패 시 해당 타입의 기본값(0, 0f 등) 반환
            return default(T);
        }

        return default(T);
    }
}


