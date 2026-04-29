using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.EventSystems;

public class BattleCard : MonoBehaviour
{
    [SerializeField]
    private BattleCardController controller;

    // Battle
    public void CardSetUp(Vector3 _defaultPosition, Quaternion _defaultRotation, int index)
    {
        controller.CardSetUp(_defaultPosition, _defaultRotation, index);
    }

    public async UniTask CardSetUpAsync(Vector3 _defaultPosition, Quaternion _defaultRotation, int index)
    {
        await controller.CardSetUpAsync(_defaultPosition, _defaultRotation, index);
    }

    public async UniTask CardSetUpAsync(Vector3 _defaultPosition, Quaternion _defaultRotation, int index, CancellationToken token)
    {
        await controller.CardSetUpAsync(_defaultPosition, _defaultRotation, index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        controller.OnPointerEnter(eventData);
    }

    public Vector3 GetCardOriginScale() => controller.GetCardOriginScale();

    public void SetCardTouchEnable(bool input) => controller.SetCardTouchEnable(input);
}
