using UnityEngine;
using Cysharp.Threading.Tasks;


[DefaultExecutionOrder(-100)]

public class GameManager : MonoBehaviour
{
    public static bool isReadyGameManager { get; private set; } = false;
    private async void Awake()
    {
        EventManager eventManager = new EventManager();
        Locator<EventManager>.Provide(eventManager);

        ResourceManager resourceManager = new ResourceManager();
        resourceManager.Initalize();
        Locator<ResourceManager>.Provide(resourceManager);

        ParserManager parserManager = new ParserManager();
        await parserManager.Initalize();

        DataManager dataManager = new DataManager(parserManager.GetCardTable(), parserManager.GetHeroTable());
        Locator<DataManager>.Provide(dataManager);

        UIManager uiManager = new UIManager();
        Locator<UIManager>.Provide(uiManager);

        Factory factory = new Factory();
        Locator<Factory>.Provide(factory);

        isReadyGameManager = true;
    }
}
