using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void RotateToPointer(Vector2 lookDirection)
    {
        Vector3 scale = transform.localScale;

        scale.x = Mathf.Sign(lookDirection.x) * Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    public void RunningAnimation(Vector2 movementInput)
    {
        animator.SetBool(Constants.RUN_PARAMETER, movementInput.magnitude > 0);
    }

    public void HurtAnimation()
    {
        animator.SetBool(Constants.PLAYER_TAKE_DAMAGE_PARAMETER, true);
    }

    public void DeadAnimation()
    {
        animator.SetTrigger(Constants.PLAYER_DEAD_PARAMETER);
    }
}