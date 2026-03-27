using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningAnimation : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Opening Complete! Welcome Lobby.");
        // NewScene으로 이동해야한다.
        SceneManager.LoadScene("newScene");
    }
}
