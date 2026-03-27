using UnityEngine;

public class UIManager
{
    private GameObject uiNewDeckList;
    private GameObject uiDeckListScroll;
    private GameObject collectionCanvas;
    private GameObject lobbyObject;
    private bool isInitCreateDeck = false;

    public void SetDeckScroll(GameObject _input) => uiDeckListScroll = _input;
    public GameObject GetDeckScroll() => uiDeckListScroll;
    public void SetNewDeckList(GameObject _input) => uiNewDeckList = _input;
    public GameObject GetNewDeckList() => uiNewDeckList;
    public void SetInitCreateDeck(bool isInit) => isInitCreateDeck = isInit;
    public void SetCollectionCanvas(GameObject _input) => collectionCanvas = _input;
    public GameObject GetCollectionCanvas() => collectionCanvas;
    public bool GetInitCreateDeck() => isInitCreateDeck;
    public void SetLobby(GameObject _input) => lobbyObject = _input;
    public GameObject GetLobby() => lobbyObject;
}

