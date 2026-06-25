using UnityEngine;
using System;

public class SelectingArrow : MonoBehaviour, IChannel
{
    private LineRenderer lineRenderer;
    private Camera mainCamera;

    private void Awake()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        mainCamera = Camera.main;
        var battleManager = Locator<BattleManager>.Get();
        battleManager.SetTargetPanel(this.gameObject);
        this.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.TargetSelected, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.TargetSelected, HandleEvent);
    }

    private void Update()
    {
        // Update에서 LineRenderer를 설정한다.
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCamera.transform.position.z);

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        lineRenderer.SetPosition(1, mouseWorldPos);

        // Cancel도 지원한다.
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            CancelSelection();
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.TargetSelected:
                SelectObject();
                break;
        }
    }

    // 지금은 LineRenderer로 하자
    public void LineSetting(Vector3 startPos)
    {
        // start point 설정
        gameObject.SetActive(true);

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, startPos);
    }

    // Object를 선택한다.
    private void SelectObject()
    {
        this.gameObject.SetActive(false);
    }

    private void CancelSelection()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.TargetCanceled);
        this.gameObject.SetActive(false);
    }
}