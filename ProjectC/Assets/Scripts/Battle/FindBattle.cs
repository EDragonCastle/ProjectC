using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FindBattle : MonoBehaviour
{
    // Find Battle
    public GameObject origin;
    public Image cancelButton;
    private Animator animator;
    private bool isCancel = false;

    private void Awake()
    {
        animator = this.gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        animator.Play("Battle Animation", 0, 0f);
        cancelButton.color = Color.white;
    }

    // 뒤로가는 Button을 눌렀을 때 실행되는 함수
    public void CancelFindBattleEnmey()
    {
        if (isCancel)
            return;

        origin.SetActive(false);
    }

    public void DisEnableCancelButton()
    {
        isCancel = true;
        cancelButton.color = Color.gray;
    }

    public void FindBattleEnemy()
    {
        // Next Scene으로 간다.
        Debug.Log("Find Battle Enmey. Welcome Battle Scene");
    }
}
