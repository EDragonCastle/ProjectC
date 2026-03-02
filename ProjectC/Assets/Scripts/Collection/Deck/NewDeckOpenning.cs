using UnityEngine;
using DG.Tweening;

public class NewDeckOpenning : MonoBehaviour
{
    public GameObject deck;
    public GameObject textObject;
    private float duration = 0.2f;

    private Vector3 initPosition;
    private float offset = 20.0f;

    // Deck Title에 달까?
    private void OnEnable()
    {
        deck.SetActive(false);
        textObject.SetActive(true);
        var rectTransform = this.GetComponent<RectTransform>();
        initPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, rectTransform.localPosition.z);
        Openning();
    }

    private void Openning()
    {
        // 이 object를 한 바퀴 돌릴 때 뭔가 해야 한다.

        var rectTransform = this.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(initPosition.x, initPosition.y - offset, initPosition.z);
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.DOKill();

        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.Join(rectTransform.DOLocalMove(initPosition, duration / 2).SetEase(Ease.InQuad));
        sequence.Append(rectTransform.DORotate(new Vector3(0, 180, 0), duration / 2).SetEase(Ease.Linear));
        sequence.Append(rectTransform.DORotate(new Vector3(0, 360, 0), duration / 2).SetEase(Ease.Linear));
        sequence.AppendCallback(() => {
            textObject.SetActive(false);
            deck.SetActive(true);
        });


        sequence.OnComplete(() => { rectTransform.localPosition = initPosition; });
    }
}
