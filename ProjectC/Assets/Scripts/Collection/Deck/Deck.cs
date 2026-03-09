using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Deck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject origin;
    public TextMeshProUGUI deckName;
    public Image deckImage;
    public GameObject destoryButton;
    public float rotationValue = 10f;
    public int vibrato = 20;

    public void OnPointerEnter(PointerEventData eventData)
    {
        destoryButton.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
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
}
