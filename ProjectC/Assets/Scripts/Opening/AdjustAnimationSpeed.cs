using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AdjustAnimationSpeed : MonoBehaviour
{
    private Animator animator;
    
    [SerializeField]
    private float animationSpeed = 1.0f;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    private void Start()
    {
        animator.speed = animationSpeed;
    }
}
