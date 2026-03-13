using System.Collections.Generic;
using System.Linq;

public class DataManager
{
    private readonly Dictionary<uint, CardData> cardTable;
    private List<CardData> sortCardTable;
    
    private Dictionary<uint, HeroData> heroTable;
    private uint heroIndex = 0;

    public Dictionary<uint, CardData> GetCardData() => cardTable;
    public Dictionary<uint, HeroData> GetHeroData() => heroTable;
    public List<CardData> GetSortCardData() => sortCardTable;

    public void SetHeroIndex(uint input) => heroIndex = input;
    public uint GetHeroIndex() => heroIndex;

    // Parser 한 Data를 DataManager가 가지고 있다는 건데
    // Parser Data가 N개 늘어난다면?
    public DataManager(Dictionary<uint, CardData> cardDataTable, Dictionary<uint, HeroData> heroDataTable)
    {
        cardTable = cardDataTable;
        heroTable = heroDataTable;
        SortByCost();
    }

    private void SortByCost()
    {
        sortCardTable = cardTable.Values.OrderBy(card => card.cost).ThenBy(card => card.cardName).ToList();
    }
}
