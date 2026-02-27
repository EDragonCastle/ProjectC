using UnityEngine;
using DG.Tweening;

public class Deck : MonoBehaviour
{
    // Deck Scroll, Create Character Deck
    public GameObject collectionPivot;

    private RectTransform collectionPivotTransform;
    private float distance = 100f;
    private float duration = 1.0f;

    private void Awake()
    {
        collectionPivotTransform = collectionPivot.GetComponent<RectTransform>();
    }

    void Start()
    {
        OpeningCollectionPivot();
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
