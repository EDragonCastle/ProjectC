using UnityEngine;
using Cysharp.Threading.Tasks;

public class ProvideNewDeckList : MonoBehaviour
{
    public NewDeckOpening Opening;
    public DeckViewPort deckView;
    public DeckComplete deckComplete;

    private async void Start()
    {
        await UniTask.WaitUntil(() => GameManager.isReadyGameManager);

        this.gameObject.SetActive(false);
        var uiManager = Locator<UIManager>.Get();
        uiManager.SetNewDeckList(this.gameObject);
        uiManager.SetInitCreateDeck(false); 
    }


    public async UniTask DeckSetup(DeckInformation deckInfo)
    {
        // 여기서 이미 OnEnable Start 처리를 했어.
        Opening.isOpening = false;
        // 필요한 거 적어보자. -> 일단 이미지와 text가 바뀌어야 해.
        Opening.deckImage.sprite = deckInfo.deckImage;
        Opening.deckNameText.text = deckInfo.deckName;

        Opening.deckHover.heroIndex = deckInfo.heroIndex;
        Opening.deckHover.deckList = deckInfo.deckData;


        deckView.cardNumber.text = $"{deckInfo.currentCard}/{deckInfo.maxCard}\n장";
        await deckView.RecontructDeck(deckInfo);
    }
}
