using UnityEngine;
using TMPro;

public class ChangeName : MonoBehaviour
{
    public GameObject deckButton;
    public TextMeshProUGUI deckName;

    public void ChangeNameButton(string name)
    {
        deckName.text = name;
        deckButton.SetActive(true);
    }
}

