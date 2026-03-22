using UnityEngine;

public class OpeningAnimation : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Opening Complete! Welcome Lobby.");
    }
}
