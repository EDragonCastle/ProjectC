using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class PageController : MonoBehaviour
{
    // Deck Controller 
    public GameObject leftButton;
    public GameObject rightButton;
    public GameObject leftPageDummy;

    // Pivot Object를 참조할 것 같다.
    public GameObject prePage;
    public GameObject curPage;
    public GameObject nextPage;

    [SerializeField]
    private float angle = 40f;

    [SerializeField]
    private float bookSpeed = 0.2f;

    private void Start()
    {
        leftPageDummy.SetActive(false);
        prePage.SetActive(false);
        curPage.SetActive(true);
        nextPage.SetActive(true);
        leftButton.SetActive(false);
    }

    public async void RightButtonClick()
    {
        Debug.Log("Right Button Click");
        bool toggle = false;
        SetButtonInteraction(false);

        var loadTask = RightLoadingResource();
        prePage.SetActive(true);
        prePage.transform.rotation = Quaternion.identity;

        await prePage.transform.DORotate(new Vector3(0, 180, 0), bookSpeed, RotateMode.LocalAxisAdd)
                         .SetEase(Ease.OutQuad)
                         .OnUpdate(() => {
                             float currentY = prePage.transform.localEulerAngles.y;
                             if (!toggle && currentY >= angle) {
                                 toggle = true;
                                 
                                 leftPageDummy.SetActive(false);
                             }
                         }).ToUniTask();

        await loadTask;

        RightFinalizeVisuality();
    }


    public async void LeftButtonClick()
    {
        Debug.Log("Left Button Click");
        SetButtonInteraction(false);

        prePage.SetActive(false);

        var loadTask = LeftComplete();

        curPage.transform.localRotation = Quaternion.Euler(0, 180, 0);

        await curPage.transform.DORotate(new Vector3(0, -180, 0), bookSpeed, RotateMode.LocalAxisAdd)
                         .SetEase(Ease.OutQuad)
                         .ToUniTask();

        await loadTask;

        LeftFinalVisuality();
    }


    private async UniTask RightLoadingResource()
    {
        // prePage, curPage, nextPage의 pageInformation을 받고 page++를 하고 pageInfo에 있는 async 함수 ResttingCard를 실행하고 싶어.
        PageInformation preInfo = prePage.GetComponentInChildren<PageInformation>(true); 
        PageInformation curInfo = curPage.GetComponentInChildren<PageInformation>(true);
        PageInformation nextInfo = nextPage.GetComponentInChildren<PageInformation>(true);
        
        preInfo.ReleaseCard();

        preInfo.page++;
        curInfo.page++;
        nextInfo.page++;

        await UniTask.WhenAll(preInfo.ResettingCard(), curInfo.ResettingCard(), nextInfo.ResettingCard());

        var dataManager = Locator<DataManager>.Get();
        var sortList = dataManager.GetSortCardData();

        if (nextInfo.cards.Length * nextInfo.page > sortList.Count)
            rightButton.SetActive(false);
        else
            rightButton.SetActive(true);
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
        // prePage, curPage, nextPage의 pageInformation을 받고 page++를 하고 pageInfo에 있는 async 함수 ResttingCard를 실행하고 싶어.
        PageInformation preInfo = prePage.GetComponentInChildren<PageInformation>(true);
        PageInformation curInfo = curPage.GetComponentInChildren<PageInformation>(true);
        PageInformation nextInfo = nextPage.GetComponentInChildren<PageInformation>(true);

        nextInfo.ReleaseCard();

        preInfo.page--;
        curInfo.page--;
        nextInfo.page--;

        await UniTask.WhenAll(preInfo.ResettingCard(), curInfo.ResettingCard(), nextInfo.ResettingCard());

        // active false는 page 정보에 따라 true로 할 지 false로 할 지 정해야 한다.
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
        PageInformation preInfo = prePage.GetComponentInChildren<PageInformation>(true);

        SetButtonInteraction(true);
        rightButton.SetActive(true);
    }

    private void SetButtonInteraction(bool isInteract)
    {
        leftButton.GetComponent<UnityEngine.UI.Button>().interactable = isInteract;
        rightButton.GetComponent<UnityEngine.UI.Button>().interactable = isInteract;
    }
}



// 현재 문제
// 장을 넘길때 깜빡이는 현상으로 문제가 생겼다.

// Resource 불러으는 데 생긴 문제인 듯 하다.
// 음..

// pre cur next
// right
// cur이 넘어가는 중이다. next 정보 변경은 아직, pre는 반납
// cur이 다 넘어갔다. cur 위치로 돌아오면서 cur 정보 변경 next 정보 변경
// 이래도 깜빡이는 현상이 나올 것 같긴한데?
// cur이 원래자리로 돌아오면서 생긴 문제가 아닐까?

// 그러면 dummy page가 움직여야 할 것 같아보이네.