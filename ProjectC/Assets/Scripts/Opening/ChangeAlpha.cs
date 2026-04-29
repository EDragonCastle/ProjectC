using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChangeAlpha : MonoBehaviour
{
    private Image thisImage;
    private void Awake()
    {
        thisImage = this.GetComponent<Image>();

        if (thisImage == null)
            Debug.LogError($"{this.gameObject.name} is Not Image Component");
    }

    private void Start()
    {
        thisImage.DOFade(0f, 1f).SetLoops(-1, LoopType.Yoyo);
    }
}
