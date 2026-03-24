using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Slot : MonoBehaviour
{
    [Header("Slot Outer")]
    public GameObject origin;
    public TextMeshProUGUI slotName;

    [Header("Slot Inner")]
    public GameObject slot;
    public Ease ease;

    public float duration = 0.5f;

    private void OnEnable()
    {
        if (slot == null)
            return;

        Opening();
    }

    public void StartSlot()
    {
        origin.SetActive(true);
    }

    private void Opening()
    {
        var slotRectTransform = slot.GetComponent<RectTransform>();

        float start = slotRectTransform.rect.height / 2;
        float end = -slotRectTransform.rect.height / 2;

        slotRectTransform.anchoredPosition = new Vector2(0, start);

        slotRectTransform.DOKill();

        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(slotRectTransform.DOAnchorPosY(end, duration).SetEase(ease))
                .OnComplete(() => {
                    slotRectTransform.anchoredPosition = new Vector2(0, end);
                });
    }

    public void Ending(bool isComplete)
    {
        var slotRectTransform = slot.GetComponent<RectTransform>();
        isComplete = true;

        float start = -slotRectTransform.rect.height / 2;
        float end = slotRectTransform.rect.height / 2;

        slotRectTransform.anchoredPosition = new Vector2(0, start);

        slotRectTransform.DOKill();

        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(slotRectTransform.DOAnchorPosY(end, duration).SetEase(ease))
                .OnComplete(() => {
                    slotRectTransform.anchoredPosition = new Vector2(0, end);
                    this.gameObject.SetActive(false);
                    isComplete = false;
                });
    }
}
