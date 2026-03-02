using UnityEngine;
using UnityEngine.EventSystems;

public class NameButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject cardCostObject;
    public void OnPointerEnter(PointerEventData eventData)
    {
        cardCostObject.SetActive(true);
        this.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardCostObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

}
