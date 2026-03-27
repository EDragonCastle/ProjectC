using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

public class Deck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject origin;
    // Deck Title Name, Image
    public Image deckImage;
    public TextMeshProUGUI deckName;

    public GameObject requireObject;
    public TextMeshProUGUI cardText;

    // Button Hover Destory Button
    public GameObject destoryButton;
    public int currentCard;
    public int maxCard;

    // Destory Vibirato
    public float rotationValue = 10f;
    public int vibrato = 20;

    private List<DeckData> deckList = new List<DeckData>();
    private bool isEmpty = false;

    private DeckInformation deckInformation;

    private readonly string lackName = "\n<color=yellow>모자란 카드</color>";

    public void SettingDeck(DeckInformation deckInfo)
    {
        deckInformation = deckInfo;
        deckList = new List<DeckData>(deckInfo.deckData);
        currentCard = deckInfo.currentCard;
        maxCard = deckInfo.maxCard;
        string pureName = deckName.text.Replace(lackName, "").Trim();

        if (deckInfo.currentCard < deckInfo.maxCard)
        {
            requireObject.SetActive(true);
            cardText.text = $"{deckInfo.currentCard}/{deckInfo.maxCard}";
            deckName.text = pureName + lackName;
            isEmpty = true;
        }
        else
        {
            requireObject.SetActive(false);
            deckName.text = pureName;
            isEmpty = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isEmpty)
            requireObject.SetActive(false);
        
        destoryButton.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(isEmpty)
            requireObject.SetActive(true);

        destoryButton.SetActive(false);
    }

    public void DestoryButton()
    {
        if (DOTween.IsTweening(origin.transform)) return;

        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.OutputDeckList);

        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(origin.transform.DOShakeRotation(0.5f, strength: new Vector3(0, 0, rotationValue), vibrato));
        sequence.OnComplete(() => { Destroy(origin); });
    }

    // Deck을 다시 Click 했을 때 볼 수 있도록 해야 해.
    public async void SelectDeck()
    {
        Debug.Log("Selecting Deck Button");
        var uiManager = Locator<UIManager>.Get();
        var uiNewDeckList = uiManager.GetNewDeckList();
        var uiDeckScroll = uiManager.GetDeckScroll();
        uiNewDeckList.SetActive(true);
        uiDeckScroll.SetActive(false);

        var newDeckComponent = uiNewDeckList.GetComponent<ProvideNewDeckList>();
        var rect = this.gameObject.GetComponent<RectTransform>();
        // Deck View Port에서 몇번째 Deck인지 알 수 있나?
        newDeckComponent.deckComplete.ResetSetup(this.gameObject, rect.position);
        
        if(newDeckComponent != null)
        {
            await newDeckComponent.DeckSetup(deckInformation);
        }
    }
}


