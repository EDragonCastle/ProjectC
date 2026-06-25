using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class SelectBattleHero : MonoBehaviour, IChannel
{
    public GameObject panel;
    public GameObject hero;

    public GameObject findBattle;

    public Image heroImage;
    public Image heroPower;
    public Image heroPowerImage;
    public TextMeshProUGUI powerName;
    public TextMeshProUGUI powerExplanation;
    public TextMeshProUGUI deckName;
    public GameObject explanation;

    private List<DeckData> deckData;

    public float duration = 0.2f;
    private float x = 0;

    private void Start()
    {
        var heroRect = hero.GetComponent<RectTransform>();
        x = heroRect.anchoredPosition.x + heroRect.sizeDelta.x;

        heroRect.anchoredPosition = new Vector2(x, heroRect.anchoredPosition.y);
        panel.SetActive(false);
        hero.SetActive(false);

        explanation.SetActive(false);
    }

    // 터지지 않을까?
    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.SelectBattleHero, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.SelectBattleHero, HandleEvent);
    }

    public void FindBattle()
    {
        Debug.Log("Game Start");
        findBattle.SetActive(true);

        var battleManager = Locator<BattleManager>.Get();
        battleManager.SetDeckData(deckData);
    }

    public void CloseBattleDeck()
    {
        var heroRect = hero.GetComponent<RectTransform>();
        heroRect.DOKill();
        heroRect.DOAnchorPosX(x, duration).OnComplete(() => {
            panel.SetActive(false);
            hero.SetActive(false);
        });
    }

    public void OpenBattleDeck()
    {
        hero.SetActive(true);
        panel.SetActive(true);
        var heroRect = hero.GetComponent<RectTransform>();
        heroRect.DOKill();
        heroRect.DOAnchorPosX(x - heroRect.sizeDelta.x, duration).OnComplete(()=> {
        });
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.SelectBattleHero:
                OpenBattleDeck();

                if(information is BattleInformation battleInfo) {
                    heroImage.sprite = battleInfo.heroImage;
                    heroPower.sprite = battleInfo.heroPowerImage;
                    heroPowerImage.sprite = battleInfo.heroPowerImage;
                    powerName.text = battleInfo.heroPowerName;
                    powerExplanation.text = battleInfo.heroPowerExplanation;
                    deckName.text = battleInfo.deckName;
                    deckData = battleInfo.deckData;
                }

                break;
        }
    }

}
