using UnityEngine;
using DG.Tweening;

// 최상단으로 빼는게 더 나을 수 있겠다. 어차피 하나의 Class로 치기 때문에
public class LobbyOpenLoading : MonoBehaviour
{
    public GameObject centerDoor;
    public GameObject centerLogo;
    public GameObject loading;

    public GameObject leftPivot;
    public GameObject rightPivot;

    // Testing ui
    public GameObject battle;
    public GameObject collection;
    public GameObject shop;
    public GameObject mode;

    public float duration = 1.0f;
    public float zoomScale = 2.5f;

    private RectTransform centerDoorRectTransform;
    private RectTransform centerLogoTransform;
    private RectTransform loadingRectTransform;

    private bool trigger = false;
    private bool isOpen = false;

    private float distance = 1000f;

    private int openIndex = 0;

    private void Awake()
    {
        centerDoorRectTransform = centerDoor.GetComponent<RectTransform>();
        centerLogoTransform = centerLogo.GetComponent<RectTransform>();
        loadingRectTransform = loading.GetComponent<RectTransform>();
    }

    private void Start()
    {
        centerLogo.SetActive(false);
    }

    private void OnEnable()
    {
        if (!isOpen) return;

        CloseDoor(openIndex);
        isOpen = false;
    }

    private void OnDisable()
    {
        centerDoor.SetActive(true);
        centerLogo.SetActive(false);
    }

    public void OnButtonClick(int index)
    {
        DOTween.Kill(centerDoorRectTransform);
        DOTween.Kill(centerLogoTransform);
        DOTween.Kill(loadingRectTransform);

        openIndex = index;

        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(centerDoorRectTransform.DORotate(new Vector3(0, -90, 0), 0.01f).SetEase(Ease.Linear));

        sequence.AppendCallback(() => {
            centerDoor.SetActive(false);
            centerLogo.SetActive(true);

            centerDoorRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            centerLogoTransform.localRotation = Quaternion.Euler(0, -90, 0);
        });

        sequence.Append(centerLogoTransform.DORotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.OutQuad));

        if(trigger)
            sequence.Join(loadingRectTransform.DORotate(new Vector3(0, 0, 120), 0.5f).From(new Vector3(0, -90, 30)));
        else
            sequence.Join(loadingRectTransform.DORotate(new Vector3(0, 0, 30), 0.5f).From(new Vector3(0, -90, -60)));

        trigger = !trigger;

        sequence.OnComplete(() => {
            Open(index);
            OpenDoor();
            isOpen = true;
        });
    }

    private void OpenDoor()
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        // RectTransform을 받아온다.
        var leftRectTransform = leftPivot.GetComponent<RectTransform>();
        var rightRectTransform = rightPivot.GetComponent<RectTransform>();

        // 좌우 회전
        sequence.Append(leftRectTransform.DORotate(new Vector3(0, 90, 0), 0.8f).SetEase(Ease.OutCubic));
        sequence.Join(rightRectTransform.DORotate(new Vector3(0, -90, 0), 0.8f).SetEase(Ease.OutCubic));

        // 크기 확대
        sequence.Join(leftRectTransform.DOScale(zoomScale, duration).SetEase(Ease.InCubic));
        sequence.Join(rightRectTransform.DOScale(zoomScale, duration).SetEase(Ease.InCubic));

        // 바깥에서 밀어내기
        sequence.Join(leftRectTransform.DOAnchorPosX(-distance, duration).SetRelative().SetEase(Ease.InCubic));
        sequence.Join(rightRectTransform.DOAnchorPosX(distance, duration).SetRelative().SetEase(Ease.InCubic));

        sequence.OnComplete(() => {
            Debug.Log("Complete Open Door");
            this.gameObject.SetActive(false);
        });
    }


    private void CloseDoor(int index)
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        // RectTransform을 받아온다.
        var leftRectTransform = leftPivot.GetComponent<RectTransform>();
        var rightRectTransform = rightPivot.GetComponent<RectTransform>();

        // 좌우 회전
        sequence.Append(leftRectTransform.DORotate(new Vector3(0, 0, 0), 0.8f).SetEase(Ease.InCubic));
        sequence.Join(rightRectTransform.DORotate(new Vector3(0, 0, 0), 0.8f).SetEase(Ease.InCubic));

        // 크기 확대
        sequence.Join(leftRectTransform.DOScale(1, duration).SetEase(Ease.OutCubic));
        sequence.Join(rightRectTransform.DOScale(1, duration).SetEase(Ease.OutCubic));

        // 원래대로 돌아오기
        sequence.Join(leftRectTransform.DOAnchorPosX(distance, duration).SetRelative().SetEase(Ease.OutCubic));
        sequence.Join(rightRectTransform.DOAnchorPosX(-distance, duration).SetRelative().SetEase(Ease.OutCubic));

        sequence.OnComplete(() => {
            Debug.Log("Complete Close Door");
            Close(index);
        });
    }


    private void Open(int index)
    {
        switch(index)
        {
            case 0:
                battle.SetActive(true);
                break;
            case 1:
                collection.SetActive(true);
                break;
            case 2:
                shop.SetActive(true);
                break;
            default:
                mode.SetActive(true);
                break;
        }
    }

    private void Close(int index)
    {
        switch (index)
        {
            case 0:
                battle.SetActive(false);
                break;
            case 1:
                collection.SetActive(false);
                break;
            case 2:
                shop.SetActive(false);
                break;
            default:
                mode.SetActive(false);
                break;
        }
    }
}
