using UnityEngine;

public class RootMotionRelay : MonoBehaviour
{
    private ThirdPersonController parentController;
    private Animator animator;

    void Start()
    {
        // Find the main script on the parent object
        parentController = GetComponentInParent<ThirdPersonController>();
        animator = GetComponent<Animator>();
    }

    // This steals the animation movement from Remy and throws it up to the parent!
    void OnAnimatorMove()
    {
        if (parentController != null && animator != null)
        {
            parentController.ReceiveRootMotion(animator.deltaPosition);
        }
    }
}