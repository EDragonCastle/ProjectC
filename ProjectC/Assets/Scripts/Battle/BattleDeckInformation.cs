using UnityEngine;

public class BattleDeckInformation : MonoBehaviour
{
    // 총 9개 존재하니까 일단 한 번 제작해볼까? 넘기는거나 이런거는 작성하지 말고 최대치 9개 먼저 하고 그 다음 늘리도록 하자.

    public BattleDeck[] battleDecks;

    private async void OnEnable()
    {
        await Cysharp.Threading.Tasks.UniTask.WaitUntil(() => GameManager.isReadyGameManager);

        ResettingBattleDeckList();
    }

    public void ResettingBattleDeckList()
    {
        for(int i = 0; i < battleDecks.Length; i++)
        {
            battleDecks[i].DeckSetting(i);
        }
    }
}

