using UnityEngine;
using DG.Tweening;

public class Battle : MonoBehaviour
{
    public GameObject backGround;
    public GameObject lobby;

    private float originScale = 0.7f;
    private float duration = 1.0f;

    private void OnEnable()
    {
        var backGroundTransform = backGround.GetComponent<RectTransform>();
        backGroundTransform.DOScale(1.0f, duration);
    }

    private void OnDisable()
    {
        var backGroundTransform = backGround.GetComponent<RectTransform>();
        backGroundTransform.localScale = new Vector3(originScale, originScale, originScale);
    }

    public void ExitButton()
    {
        lobby.SetActive(true);

        var backGroundTransform = backGround.GetComponent<RectTransform>();
        backGroundTransform.DOScale(originScale, duration * 1.5f);
    }

    // Animation 중간에 Touch 할 수 없게 막아둘까? -> 나중에 문제 생길 수도 있나?
}
