using UnityEngine;
using UnityEngine.EventSystems;

public class DeckListHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject destoryButton;

    public void OnPointerEnter(PointerEventData eventData)
    {
        destoryButton.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        destoryButton.SetActive(false);
    }
}
