using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHeroPower : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public HeroSelect select;

    public void OnPointerEnter(PointerEventData eventData)
    {
        select.IsActvieHeroExplantion(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        select.IsActvieHeroExplantion(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        select.IsActvieHeroExplantion(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        select.IsActvieHeroExplantion(false);
    }

}
