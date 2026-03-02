using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class NewDeckHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject cardCostObject;
    public GameObject nameField;
    public TextMeshProUGUI deckName;

    // Down 눌렀을 때 이름을 바꾸도록 한다.
    public void OnPointerEnter(PointerEventData eventData)
    {
        CostObejct(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CostObejct(false);
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

    private void CostObejct(bool isActvie)
    {
        cardCostObject.SetActive(isActvie);
    }
}
