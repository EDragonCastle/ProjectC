using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class NewDeckHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject cardCostObject;
    public GameObject nameField;
    public TextMeshProUGUI deckName;
    public DeckViewPort viewPort;

    public uint heroIndex;
    public List<DeckData> deckList;



    public async void OnPointerEnter(PointerEventData eventData)
    {
        await CostObejct(true);
    }

    public async void OnPointerExit(PointerEventData eventData)
    {
        await CostObejct(false);
    }

    public void ChangeNameButton()
    {
        deckName.gameObject.SetActive(false);
        nameField.SetActive(true);
        var inputComponent = nameField.GetComponent<TMP_InputField>();
        inputComponent.interactable = true;

        inputComponent.Select();
        inputComponent.ActivateInputField();

        Debug.Log("입력 모드 시작");
    }

    public void SelectName()
    {
        var inputComponent = nameField.GetComponent<TMP_InputField>();
        deckName.text = inputComponent.text;
        inputComponent.text = "";
        nameField.SetActive(false);
        deckName.gameObject.SetActive(true);
    }

    private async UniTask CostObejct(bool isActive)
    {
        var cardCostComponent = cardCostObject.GetComponent<CostCard>();
        cardCostComponent.heroIndex = heroIndex;

        if (deckList == null || deckList != viewPort.GetDeckData())
            deckList = viewPort.GetDeckData();

        await cardCostComponent.CardSetup(deckList);
        cardCostObject.SetActive(isActive);
    }


}
