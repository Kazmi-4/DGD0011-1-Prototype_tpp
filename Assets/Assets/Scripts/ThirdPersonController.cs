using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement Feel")]
    public float rotationSpeed = 10f;
    public float animationDampTime = 0.15f;
    public float strafeSpeedBoost = 1f;

    [Header("Parkour & Vault System")]
    public LayerMask obstacleLayer;
    public float vaultCheckDistance = 1.5f;
    public float vaultMaxHeight = 3.0f;

    [Tooltip("Start slightly LATER than the CrossFade time (e.g., 0.2)")]
    [Range(0, 1)] public float matchStartTime = 0.2f;
    [Tooltip("End should be around 0.5 to 0.6 for a standard vault")]
    [Range(0, 1)] public float matchEndTime = 0.5f;

    [Header("Jump Physics")]
    public float jumpForce = 15f;
    public float jumpTime = 0.5f;
    public float gravity = 25f;
    public float standingJumpDelay = 0.8f;
    public float runningJumpDelay = 0.1f;

    private float verticalVelocity = 0f;
    private float jumpElapsedTime = 0;
    private bool isJumping = false;
    private bool isJumpWindup = false;
    private float windupTimer = 0f;
    private bool isVaulting = false;
    private Vector3 vaultTargetPosition;

    public Animator animator;
    CharacterController cc;

    InputAction moveAction; InputAction sprintAction; InputAction jumpAction;

    void Awake()
    {
        Application.targetFrameRate = 60;
        cc = GetComponent<CharacterController>();

        moveAction = new InputAction("Move");
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");

        sprintAction = new InputAction("Sprint", binding: "<Keyboard>/leftShift");
        jumpAction = new InputAction("Jump", binding: "<Keyboard>/space");
    }

    void OnEnable() { moveAction.Enable(); sprintAction.Enable(); jumpAction.Enable(); }
    void OnDisable() { moveAction.Disable(); sprintAction.Disable(); jumpAction.Disable(); }

    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        bool isSprinting = sprintAction.IsPressed();
        bool inputJump = jumpAction.WasPressedThisFrame();
        bool isMoving = moveInput.magnitude > 0.1f;

        if (animator != null)
        {
            animator.SetBool("air", !cc.isGrounded);
            animator.SetBool("isMoving", isMoving);
        }

        float speedMultiplier = isSprinting ? 1f : 0.5f;
        float targetAnimX = (isMoving && !isVaulting) ? moveInput.x * speedMultiplier : 0f;
        float targetAnimY = (isMoving && !isVaulting) ? moveInput.y * speedMultiplier : 0f;

        if (animator != null)
        {
            animator.SetFloat("InputX", targetAnimX, animationDampTime, Time.deltaTime);
            animator.SetFloat("InputY", targetAnimY, animationDampTime, Time.deltaTime);
        }

        // Vault Trigger
        if (inputJump && cc.isGrounded && !isJumping && !isVaulting)
        {
            if (isMoving && DetectVaultObstacle(out vaultTargetPosition))
            {
                isVaulting = true;
                // Use a slightly faster CrossFade for responsiveness
                if (animator != null) animator.CrossFadeInFixedTime("Vault", 0.1f);
            }
            else
            {
                isJumpWindup = true;
                windupTimer = isMoving ? runningJumpDelay : standingJumpDelay;
                if (animator != null) animator.SetTrigger("jumpTrigger");
            }
        }

        if (isJumpWindup)
        {
            windupTimer -= Time.deltaTime;
            if (windupTimer <= 0f) { isJumpWindup = false; isJumping = true; jumpElapsedTime = 0; }
        }

        // Vault Execution
        if (isVaulting && animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("Vault"))
            {
                // FIX: Ensure the snap only happens once the transition is totally done
                if (!animator.IsInTransition(0))
                {
                    // WEIGHT MASK FIX: Vector3.one ensures we match X, Y, and Z positions perfectly
                    animator.MatchTarget(vaultTargetPosition, transform.rotation, AvatarTarget.RightHand,
                        new MatchTargetWeightMask(Vector3.one, 0f), matchStartTime, matchEndTime);
                }

                // If we are past the end time, release control
                if (stateInfo.normalizedTime >= 0.85f) isVaulting = false;
            }
            // FAILSAFE: If the animator transitions to something else, force isVaulting to false
            else if (!animator.IsInTransition(0))
            {
                isVaulting = false;
            }
        }

        if (isMoving && !isVaulting)
        {
            float cameraYaw = Camera.main.transform.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, cameraYaw, 0), rotationSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (isVaulting) { verticalVelocity = 0f; return; }
        if (isJumping)
        {
            verticalVelocity = Mathf.SmoothStep(jumpForce, 0, jumpElapsedTime / jumpTime);
            jumpElapsedTime += Time.fixedDeltaTime;
            if (jumpElapsedTime >= jumpTime) isJumping = false;
        }
        else if (!cc.isGrounded) verticalVelocity -= gravity * Time.fixedDeltaTime;
        else verticalVelocity = -5f;
    }

    public void ReceiveRootMotion(Vector3 animationDeltaPosition)
    {
        if (cc == null) return;
        // Apply smooth movement during vaulting
        Vector3 finalMove = isVaulting ? animationDeltaPosition : animationDeltaPosition * strafeSpeedBoost;
        finalMove.y = verticalVelocity * Time.deltaTime;
        cc.Move(finalMove);
    }

    bool DetectVaultObstacle(out Vector3 edgePosition)
    {
        edgePosition = Vector3.zero;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(origin, transform.forward, out RaycastHit forwardHit, vaultCheckDistance, obstacleLayer))
        {
            // Position the "downward" ray slightly inside the obstacle
            Vector3 topOrigin = forwardHit.point + (transform.forward * 0.2f) + (Vector3.up * vaultMaxHeight);

            if (Physics.Raycast(topOrigin, Vector3.down, out RaycastHit topHit, vaultMaxHeight, obstacleLayer))
            {
                // MATH FIX: Set the target to the EXACT edge where the hand should be.
                // We take the Hit Point and pull it slightly back toward the player so the hand grips the front edge.
                edgePosition = topHit.point - (transform.forward * 0.05f);

                // Visual feedback
                Debug.DrawLine(origin, forwardHit.point, Color.red, 1f);
                Debug.DrawLine(topOrigin, topHit.point, Color.green, 1f);

                return true;
            }
        }
        return false;
    }
}