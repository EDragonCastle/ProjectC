using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public class PageController : MonoBehaviour, IChannel
{
    // Deck Controller 
    public GameObject leftButton;
    public GameObject rightButton;
    public GameObject leftPageDummy;

    public GameObject prePage;
    public GameObject curPage;
    public GameObject nextPage;

    // SlotObject 이곳에 있으면 안 되는 애다.
    public Slot slotScript;

    [SerializeField]
    private float angle = 40f;

    [SerializeField]
    private float bookSpeed = 0.2f;

    private bool isProcessing = false;

    private async void Start()
    {
        await UniTask.WaitUntil(() => GameManager.isReadyGameManager);

        leftPageDummy.SetActive(false);
        prePage.SetActive(false);
        curPage.SetActive(true);
        nextPage.SetActive(true);
        leftButton.SetActive(false);

    }

    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.Filter, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.Filter, HandleEvent);
    }


    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch (channel)
        {
            case ChannelInfo.Filter:
                if (information is FilterParameter parameter)
                {
                    if (isProcessing) 
                        return;

                    var token = this.GetCancellationTokenOnDestroy();
                    ProcessFilter(parameter, token).Forget();
                }

                break;
        }
    }

    public async void RightButtonClick()
    {
        Debug.Log("Right Button Click");
        await UniTask.WhenAll(RightLoadingResource(), TurnRightPage());
    }

    private async UniTask TurnRightPage()
    {
        SetButtonInteraction(false);

        bool toggle = false;
        prePage.SetActive(true);
        prePage.transform.rotation = Quaternion.identity;

        await prePage.transform.DORotate(new Vector3(0, 180, 0), bookSpeed, RotateMode.LocalAxisAdd)
                         .SetEase(Ease.OutQuad)
                         .OnUpdate(() => {
                             float currentY = prePage.transform.localEulerAngles.y;
                             if (!toggle && currentY >= angle)
                             {
                                 toggle = true;

                                 leftPageDummy.SetActive(false);
                             }
                         }).ToUniTask();

        RightFinalizeVisuality();
    }

    public async void LeftButtonClick()
    {
        Debug.Log("Left Button Click");
        await UniTask.WhenAll(LeftComplete(), TurnLeftPage());
    }

    private async UniTask TurnLeftPage()
    {
        SetButtonInteraction(false);
        prePage.SetActive(false);

        curPage.transform.localRotation = Quaternion.Euler(0, 180, 0);

        await curPage.transform.DORotate(new Vector3(0, -180, 0), bookSpeed, RotateMode.LocalAxisAdd)
                         .SetEase(Ease.OutQuad)
                         .ToUniTask();

        LeftFinalVisuality();
    }


    private async UniTask RightLoadingResource()
    {
        PageInformation preInfo = prePage.GetComponentInChildren<PageInformation>(true);
        PageInformation curInfo = curPage.GetComponentInChildren<PageInformation>(true);
        PageInformation nextInfo = nextPage.GetComponentInChildren<PageInformation>(true);

        preInfo.ReleaseCard();

        preInfo.page++;
        curInfo.page++;
        nextInfo.page++;

        // slotName
        var dataManager = Locator<DataManager>.Get();
        string hero = dataManager.GetPageToHeroName(curInfo.page);
        slotScript.SetSlotName(hero);

        await UniTask.WhenAll(preInfo.ResettingCard(), curInfo.ResettingCard(), nextInfo.ResettingCard());

        int pageIndex = dataManager.GetPageCount();

        rightButton.SetActive(curInfo.page < pageIndex - 1);
    }

    private void RightFinalizeVisuality()
    {
        SetButtonInteraction(true);
        prePage.SetActive(true);
        leftPageDummy.SetActive(true);
        leftButton.SetActive(true);
    }

    private async UniTask LeftComplete()
    {
        PageInformation preInfo = prePage.GetComponentInChildren<PageInformation>(true);
        PageInformation curInfo = curPage.GetComponentInChildren<PageInformation>(true);
        PageInformation nextInfo = nextPage.GetComponentInChildren<PageInformation>(true);

        nextInfo.ReleaseCard();

        preInfo.page--;
        curInfo.page--;
        nextInfo.page--;

        // slotName
        var dataManger = Locator<DataManager>.Get();
        string hero = dataManger.GetPageToHeroName(curInfo.page);
        slotScript.SetSlotName(hero);

        await UniTask.WhenAll(preInfo.ResettingCard(), curInfo.ResettingCard(), nextInfo.ResettingCard());

        if (preInfo.page < 1)
        {
            if (preInfo.page != 0)
            {
                leftButton.SetActive(false);
                leftPageDummy.SetActive(false);
                prePage.SetActive(false);
            }
            else
            {
                leftPageDummy.SetActive(false);
                prePage.SetActive(true);
            }
        }
        else
            leftButton.SetActive(true);
    }

    private void LeftFinalVisuality()
    {
        SetButtonInteraction(true);
        rightButton.SetActive(true);
    }

    private void SetButtonInteraction(bool isInteract)
    {
        leftButton.GetComponent<UnityEngine.UI.Button>().interactable = isInteract;
        rightButton.GetComponent<UnityEngine.UI.Button>().interactable = isInteract;
    }

    private async UniTask ProcessFilter(FilterParameter parameter, System.Threading.CancellationToken token)
    {
        isProcessing = true;

        try {
            var dataManager = Locator<DataManager>.Get();
            switch (parameter.filterType)
            {
                case FilterType.Jump:
                    int pageIndex = dataManager.GetHeroStartPage(parameter.job[0]);
                    await JumpPage(pageIndex, token);
                    break;
                case FilterType.Search:
                    dataManager.UpdateFilter(job: parameter.job, cost: parameter.cost, keyword: parameter.searchName);
                    await JumpPage(0, token);
                    break;

            }
        }
        catch(OperationCanceledException) {
            Debug.Log("해당 작업이 유저나 시스템에 의해 취소 되었습니다.");
        }
        catch(Exception e) {
            Debug.Log($"Error 발생 : {e.Message}");
        }
        finally { isProcessing = false; }
    }

    private async UniTask JumpPage(int jumpPage, System.Threading.CancellationToken token)
    {
        var dataManager = Locator<DataManager>.Get();
        int maxPageIndex = dataManager.GetPageCount();

        PageInformation preInfo = prePage.GetComponentInChildren<PageInformation>(true);
        PageInformation curInfo = curPage.GetComponentInChildren<PageInformation>(true);
        PageInformation nextInfo = nextPage.GetComponentInChildren<PageInformation>(true);

        preInfo.ReleaseCard();
        curInfo.ReleaseCard();
        nextInfo.ReleaseCard();

        UniTask turnTask = UniTask.CompletedTask;
        turnTask = ActiveJumpButton(jumpPage, curInfo.page, maxPageIndex);
  
        preInfo.page = jumpPage - 1;
        curInfo.page = jumpPage;
        nextInfo.page = jumpPage + 1;

        // Slot Name
        string hero = dataManager.GetPageToHeroName(curInfo.page);
        slotScript.SetSlotName(hero);

        await UniTask.WhenAll(turnTask, preInfo.ResettingCard(), curInfo.ResettingCard(), nextInfo.ResettingCard()).AttachExternalCancellation(token);
    }

    private UniTask ActiveJumpButton(int jumpingPage, int currentPage, int maxPageIndex)
    {
        UniTask turnTask = UniTask.CompletedTask;
        if (currentPage > jumpingPage)
        {
            if (jumpingPage-1 < 1)
            {
                if (jumpingPage-1 != 0)
                {
                    leftButton.SetActive(false);
                    leftPageDummy.SetActive(false);
                    prePage.SetActive(false);
                }
                else
                {
                    leftPageDummy.SetActive(false);
                    prePage.SetActive(true);
                }
            }
            else
                leftButton.SetActive(true);

            turnTask = TurnLeftPage();
        }
        else if (currentPage < jumpingPage)
        {
            rightButton.SetActive(jumpingPage + 1 < maxPageIndex);
            turnTask = TurnRightPage();
        }
        else
        {
            // 0번째일 때는 너무 부자연스러운데 어떻게 해결해야하는 편이 좋을까?
            // 연출로 가리는 게 베스트긴한데..

            if (jumpingPage-1 < 1)
            {
                if (jumpingPage-1 != 0)
                {
                    leftButton.SetActive(false);
                    leftPageDummy.SetActive(false);
                    prePage.SetActive(false);
                }
                else
                {
                    leftPageDummy.SetActive(false);
                    prePage.SetActive(true);
                }
            }
            else
                leftButton.SetActive(true);

            rightButton.SetActive(jumpingPage + 1 < maxPageIndex);
        }

        return turnTask;
    }
}



public enum FilterType
{
    Jump, Search, Reset
}

public struct FilterParameter
{
    public FilterType filterType;
    public string[] job;
    public string searchName;
    public int? cost;

    public FilterParameter(FilterType type, string[] _job = null, string _search = null, int? _cost = null)
    {
        filterType = type;
        job = _job;
        searchName = _search;
        cost = _cost;
    }
}

