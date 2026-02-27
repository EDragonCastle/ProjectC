using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHeroPower : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public HeroSelect select;
    public Collection collection;

    public void OnPointerEnter(PointerEventData eventData)
    {
        collection.IsActvieHeroExplantion(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        collection.IsActvieHeroExplantion(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        collection.IsActvieHeroExplantion(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        collection.IsActvieHeroExplantion(false);
    }

}
