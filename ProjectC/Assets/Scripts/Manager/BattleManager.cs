using UnityEngine;

public class BattleManager
{
    private GameObject handPanel;
    private GameObject handParent;

    public GameObject GetHandPanel() => handPanel;
    public void SetHandPanel(GameObject input) => handPanel = input;

    public GameObject GetHandParent() => handParent;
    public void SetHandParent(GameObject input) => handParent = input;
}
