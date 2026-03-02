using UnityEngine;
using DG.Tweening;
using TMPro;

public class DeckList : MonoBehaviour
{
    // Deck Scroll, Create Character Deck
    public GameObject collectionPivot;

    private RectTransform collectionPivotTransform;
    private float distance = 100f;
    private float duration = 1.0f;

    public TextMeshProUGUI deckListText;
    public GameObject content;
    private int maxCard = 9;
    private int index = 0;

    private void Awake()
    {
        collectionPivotTransform = collectionPivot.GetComponent<RectTransform>();
    }

    void Start()
    {
        OpeningCollectionPivot();
        deckListText.text = "0/0\nµ¦";
    }


    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.InputDeckList, HandleEvent);
        eventManager.Subscription(ChannelInfo.OutputDeckList, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.InputDeckList, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.OutputDeckList, HandleEvent);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch (channel)
        {
            case ChannelInfo.InputDeckList:
                index++;
                if (index > maxCard)
                    index = maxCard;
                deckListText.text = $"{index}/{maxCard}\nµ¦";
                break;
            case ChannelInfo.OutputDeckList:
                index--;
                if (index < 0)
                    index = 0;
                deckListText.text = $"{index}/{maxCard}\nµ¦";
                break;
        }
    }

    private void OpeningCollectionPivot()
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(collectionPivotTransform.DORotate(new Vector3(0, 100, 0), duration).SetEase(Ease.Linear));
        sequence.Join(collectionPivotTransform.DOAnchorPosX(-distance, duration).SetRelative().SetEase(Ease.InCubic));

        sequence.AppendCallback(() => {
        collectionPivotTransform.DOAnchorPosX(distance, duration).SetRelative().SetEase(Ease.InCubic);
        collectionPivotTransform.localRotation = Quaternion.Euler(Vector3.zero);
            collectionPivot.SetActive(false);
        });
    }
}
