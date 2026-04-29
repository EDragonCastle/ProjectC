using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BattleHeroPowerHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject heroPowerExplanation;
    public float duration = 0.2f;
    public Ease ease;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ActiveObject(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ActiveObject(false);
    }

    private void ActiveObject(bool isActive)
    {
        var heroRect = heroPowerExplanation.GetComponent<RectTransform>();
        heroRect.DOKill();

        if(isActive)
        {
            heroPowerExplanation.SetActive(true);
            heroRect.DOScale(1f, duration).SetEase(ease);
        }
        else
        {
            heroRect.DOScale(0.7f, duration/2).SetEase(ease).OnComplete(() => {
            heroPowerExplanation.SetActive(false);
            });
        }
    }
}
