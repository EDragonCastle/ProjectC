using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Title : MonoBehaviour
{
    public Image shineJewel;
    public Image logo;
    public GameObject testOpening;
    public float startWaitSecond = 2f;
    public float logoWaitSecond = 2f;
    public float duration = 1.5f;

    private void Awake()
    {
        logo.gameObject.SetActive(false);
        testOpening.SetActive(false);
    }

    private void Start()
    {
        shineJewel.DOFade(0f, 1f).SetLoops(-1, LoopType.Yoyo);
        Color color = logo.color;
        color.a = 0;
        logo.color = color;
        StartCoroutine(Logo());
    }

    private IEnumerator Logo()
    {
        yield return new WaitForSeconds(startWaitSecond);

        logo.gameObject.SetActive(true);
        yield return logo.DOFade(1f, duration).WaitForCompletion();

        yield return new WaitForSeconds(logoWaitSecond);

        yield return logo.DOFade(0f, duration).WaitForCompletion();

        Debug.Log("ø¨√‚ ≥°");
        testOpening.SetActive(true);
    }
}
