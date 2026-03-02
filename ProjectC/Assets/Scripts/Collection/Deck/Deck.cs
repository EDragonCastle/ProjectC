using UnityEngine;
using TMPro;

public class Deck : MonoBehaviour
{
    public GameObject origin;
    public TextMeshProUGUI deckName;

    public void DestoryButton()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.OutputDeckList);
        Destroy(origin);
    }
}

// 구조체 정보가 필요할 수도 있다. deck name, deck texture, deck information 등등..