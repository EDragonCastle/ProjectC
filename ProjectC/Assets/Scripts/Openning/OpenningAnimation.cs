using UnityEngine;

public class OpenningAnimation : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Openning Complete! Welcome Lobby.");
    }
}
