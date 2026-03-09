using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ExplanationCard : MonoBehaviour, IPointerDownHandler
{
    public GameObject origin;
    public GameObject card;
    private Vector3 startPosition;
    private float duration = 0.5f;
    private bool isTweening = false;

    public void SetUp(Vector3 startPoint)
    {
        startPosition = startPoint;
        Openning();
    }

    // Panel에 설치할 예정이다.
    public void OnPointerDown(PointerEventData eventData)
    {
        if(!isTweening)
            Endding();
    }

    private void Openning()
    {
        var rectTransform = card.GetComponent<RectTransform>();
        rectTransform.position = startPosition;
        rectTransform.localScale = Vector3.one;
        isTweening = true;

        rectTransform.DOKill();

        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutBack));
        sequence.Join(rectTransform.DOScale(new Vector3(2f, 2f, 2f), duration).SetEase(Ease.OutBack));

        sequence.OnComplete(() => { isTweening = false; });
    }

    private void Endding()
    {
        var rectTransform = card.GetComponent<RectTransform>();

        rectTransform.DOKill();
        isTweening = true;
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOMove(startPosition, duration).SetEase(Ease.InBack));
        sequence.Join(rectTransform.DOScale(Vector3.one, duration).SetEase(Ease.InBack));

        sequence.OnComplete(() => {
            origin.SetActive(false); isTweening = false; });
    }
}