using UnityEngine;
using UnityEngine.EventSystems;

public class ArrowTest : MonoBehaviour
{
    [Header("References")]
    public LineRenderer arrowBody;       // LineRenderer
    public RectTransform arrowEnd;       // Image (화살촉)
    public RectTransform arrowTarget;    // Image (타겟 마커)
    public RectTransform canvasRect;     // 최상위 Canvas RectTransform
    public Canvas canvas;                // Canvas (카메라 참조용)

    [Header("Arrow Body Settings")]
    public int lineSegments = 20;      // 선 부드러움 (베지어 쓸 경우)
    public float lineWidth = 10f;     // 선 두께

    private bool _isDragging;
    private Vector2 _startPos;           // Canvas 로컬 좌표
    private Camera _cam;

    void Start()
    {
        _cam = canvas.worldCamera ?? Camera.main;

        // LineRenderer 기본 설정
        arrowBody.positionCount = 2;
        arrowBody.useWorldSpace = true;
        arrowBody.startWidth    = lineWidth;
        arrowBody.endWidth      = lineWidth;

        SetVisible(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // UI 클릭 감지 (버튼 등 UI 위 클릭 제외하려면 아래 주석 해제)
            // if (EventSystem.current.IsPointerOverGameObject()) return;

            _startPos   = ScreenToCanvasPos(Input.mousePosition);
            _isDragging = true;
            SetVisible(true);
        }

        if (_isDragging && Input.GetMouseButton(0))
        {
            UpdateArrow(ScreenToCanvasPos(Input.mousePosition));
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            SetVisible(false);
        }
    }

    void UpdateArrow(Vector2 endCanvasPos)
    {
        // Canvas 로컬 좌표 → World 좌표 변환
        Vector3 startWorld = CanvasPosToWorld(_startPos);
        Vector3 endWorld = CanvasPosToWorld(endCanvasPos);

        // ── LineRenderer 업데이트 ──────────────────────────
        arrowBody.SetPosition(0, startWorld);
        arrowBody.SetPosition(1, endWorld);

        // ── 화살촉 방향 회전 ───────────────────────────────
        Vector2 dir = endCanvasPos - _startPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        arrowEnd.anchoredPosition  = endCanvasPos;
        arrowEnd.localEulerAngles  = new Vector3(0, 0, angle - 90f);

        // ── 타겟 마커 ──────────────────────────────────────
        arrowTarget.anchoredPosition = endCanvasPos;
    }

    // Canvas 로컬 좌표 → World 좌표
    Vector3 CanvasPosToWorld(Vector2 canvasPos)
    {
        // canvasRect 기준 로컬 → World
        return canvasRect.TransformPoint(new Vector3(canvasPos.x, canvasPos.y, 0));
    }

    // 스크린 좌표 → Canvas 로컬 좌표
    Vector2 ScreenToCanvasPos(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            _cam,
            out Vector2 localPos
        );
        return localPos;
    }

    void SetVisible(bool v)
    {
        arrowBody.gameObject.SetActive(v);
        arrowEnd.gameObject.SetActive(v);
        arrowTarget.gameObject.SetActive(v);
    }
}