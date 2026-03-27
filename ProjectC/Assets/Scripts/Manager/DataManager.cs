using System.Collections.Generic;
using System.Linq;

public class DataManager
{
    // 실제 card Data들이 들어오는 공간.
    private readonly Dictionary<uint, CardData> cardTable;
    private List<List<CardData>> pages;
    private FilterInformation filterInfo;

    private Dictionary<string, int> heroStartPages;
    private List<KeyValuePair<string, int>> sortedHeroPages;

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

    public bool HasJob(string hero)
    {
        return heroStartPages.ContainsKey(hero) ? true : false;
    }

    public int GetHeroStartPage(string hero)
    {
        if (heroStartPages.TryGetValue(hero, out int pageIndex))
            return pageIndex;
        else
            return 0;
    }

    public string GetPageToHeroName(int page)
    {
        string hero = "";
        int heroPageIndex = sortedHeroPages.Count;

        for (int i = heroPageIndex-1; i >= 0; i--)
        {
            if(sortedHeroPages[i].Value <= page)
            {
                hero = sortedHeroPages[i].Key;
                break;
            }
        }
        return hero;
    }

    public DataManager(Dictionary<uint, CardData> cardDataTable, Dictionary<uint, HeroData> heroDataTable)
    {
        pages = new List<List<CardData>>();
        heroStartPages = new Dictionary<string, int>();
        sortedHeroPages = new List<KeyValuePair<string, int>>();
        cardTable = cardDataTable;
        heroTable = heroDataTable;
        filterInfo = new FilterInformation();
        filterInfo.Clear();
        RefreshPage();
    }

    private void RefreshPage()
    {
        pages.Clear();
        heroStartPages.Clear();

        var filterQuery = cardTable.Values.AsEnumerable();

        if(filterInfo.job != null && filterInfo.job.Length > 0)
        {
            if(!filterInfo.job.Contains("All"))
                filterQuery = filterQuery.Where(card => filterInfo.job.Contains(card.jobType));
        }

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

        sortedHeroPages = heroStartPages.OrderBy(card => card.Value).ToList();
    }

    public void UpdateFilter(string[] job = null, int? cost = null, string keyword = null)
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
}

