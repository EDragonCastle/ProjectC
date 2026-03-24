using System.Collections.Generic;
using System.Linq;

public class DataManager
{
    // 실제 card Data들이 들어오는 공간.
    private readonly Dictionary<uint, CardData> cardTable;
    private List<List<CardData>> pages;
    private FilterInformation filterInfo;

    private Dictionary<string, int> heroStartPages;

    // hero 정보들이 담겨져 있는 곳
    private Dictionary<uint, HeroData> heroTable;
    private uint heroIndex = 0;

    private int collectionMaxCard = 8;

    public Dictionary<uint, CardData> GetCardData() => cardTable;
    public Dictionary<uint, HeroData> GetHeroData() => heroTable;

    public int GetPageCount() => pages.Count;

    public void SetHeroIndex(uint input) => heroIndex = input;
    public uint GetHeroIndex() => heroIndex;
    public List<CardData> GetPageData(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < pages.Count)
            return pages[pageIndex];
        else
            return null;
    }

    // Parser 한 Data를 DataManager가 가지고 있다는 건데
    // Parser Data가 N개 늘어난다면?
    public DataManager(Dictionary<uint, CardData> cardDataTable, Dictionary<uint, HeroData> heroDataTable)
    {
        pages = new List<List<CardData>>();
        heroStartPages = new Dictionary<string, int>();
        cardTable = cardDataTable;
        heroTable = heroDataTable;
        filterInfo = new FilterInformation();
        filterInfo.Clear();
        RefreshPage();
    }

    public void RefreshPage()
    {
        pages.Clear();
        heroStartPages.Clear();

        var filterQuery = cardTable.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filterInfo.job))
            filterQuery = filterQuery.Where(card => card.jobType == filterInfo.job);

        if (filterInfo.mana != null)
        {
            if (filterInfo.mana != -1)
                filterQuery = filterQuery.Where(card => filterInfo.mana.Value >= 7 ? card.cost >= 7 : card.cost == filterInfo.mana.Value);
        }

        if (!string.IsNullOrWhiteSpace(filterInfo.keyword))
            filterQuery = filterQuery.Where(card => card.cardName.Contains(filterInfo.keyword));

        var sortedCard = filterQuery.OrderBy(card => card.jobType).ThenBy(card => card.cost).ThenBy(card => card.cardName);

        var groupCards = sortedCard.GroupBy(card => card.jobType);

        foreach(var jobGroup in groupCards)
        {
            heroStartPages[jobGroup.Key] = pages.Count;
            var jobCardList = jobGroup.ToList();
            for (int i = 0; i < jobCardList.Count; i += collectionMaxCard)
            {
                pages.Add(jobCardList.Skip(i).Take(collectionMaxCard).ToList());
            }
        }
    }

    public void UpdateFilter(string job = null, int? cost = null, string keyword = null)
    {
        if (job != null) 
            filterInfo.job = job;

        if (cost.HasValue)
            filterInfo.mana = cost.Value;

        if (keyword != null)
            filterInfo.keyword = keyword;

        RefreshPage();
    }

    public void ClearFilter()
    {
        filterInfo.Clear();
        RefreshPage();
    }

    public int GetHeroStartPage(string hero)
    {
        if (heroStartPages.TryGetValue(hero, out int pageIndex))
            return pageIndex;
        else
            return 0;
    }
}


// Update Filter Methord는 버튼을 눌렀을 때 작동되어야 하는 함수야.

// 지금 있는 곳에서 이동하는 로직은 따로 없어.