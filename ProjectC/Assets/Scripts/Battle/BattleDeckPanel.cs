using UnityEngine;
using UnityEngine.EventSystems;

public class BattleDeckPanel : MonoBehaviour, IPointerDownHandler
{
    public SelectBattleHero heroPanel;

    public void OnPointerDown(PointerEventData eventData)
    {
        heroPanel.CloseBattleDeck();
    }
}
