using UnityEngine;
using System.Collections.Generic;

public class BattleManager
{
    private GameObject handPanel;
    private GameObject handParent;
    private GameObject battleField;
    private GameObject targetPanel;

    private List<DeckData> deckData;
    public int usingCardIndex;

    public GameObject GetTargetPanel() => targetPanel;
    public void SetTargetPanel(GameObject input) => targetPanel = input;

    public GameObject GetHandPanel() => handPanel;
    public void SetHandPanel(GameObject input) => handPanel = input;

    public GameObject GetHandParent() => handParent;
    public void SetHandParent(GameObject input) => handParent = input;

    public GameObject GetBattleField() => battleField;
    public void SetBattleField(GameObject input) => battleField = input; 

    public void SetDeckData(List<DeckData> input) => deckData = input;
    public List<DeckData> GetDeckData() => deckData;
}
