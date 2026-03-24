using UnityEngine;

public class ManaCost : MonoBehaviour
{
    public GameObject selectObject;
    public GameObject noneSelectObject;
    public int mana;

    private void Start()
    {
        noneSelectObject.SetActive(true);
        selectObject.SetActive(false);
    }

    public void Select()
    {
        selectObject.SetActive(true);
        noneSelectObject.SetActive(false);
    }

    public void NoneSelect()
    {
        selectObject.SetActive(false);
        noneSelectObject.SetActive(true);
    }
}


// Mana Cost를 눌렀을 때 다른 것들이 none으로 바꿔야 한다.
// 하지만 그러지 않고 있어.
// 머리아파. 힘도없고