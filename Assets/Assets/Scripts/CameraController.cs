using UnityEngine;
using UnityEngine.InputSystem; // Added for the New Input System

/*
    This file has a commented version with details about how each line works. 
    The commented version contains code that is easier and simpler to read. This file is minified.
*/

/// <summary>
/// Camera movement script for third person games.
/// This Script should not be applied to the camera! It is attached to an empty object and inside
/// it (as a child object) should be your game's MainCamera.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Tooltip("Enable to move the camera by holding the right mouse button. Does not work with joysticks.")]
    public bool clickToMoveCamera = false;
    [Tooltip("Enable zoom in/out when scrolling the mouse wheel. Does not work with joysticks.")]
    public bool canZoom = true;
    [Space]
    [Tooltip("The higher it is, the faster the camera moves. It is recommended to increase this value for games that uses joystick.")]
    public float sensitivity = 5f;

    [Tooltip("Camera Y rotation limits. The X axis is the maximum it can go up and the Y axis is the maximum it can go down.")]
    public Vector2 cameraLimit = new Vector2(-45, 40);

    float mouseX;
    float mouseY;
    float offsetDistanceY;

    Transform player;

    // --- NEW INPUT SYSTEM ACTIONS ---
    InputAction lookAction;
    InputAction zoomAction;
    InputAction clickAction;

    void Awake()
    {
        // Setup internal input actions
        lookAction = new InputAction("Look");
        lookAction.AddBinding("<Pointer>/delta");
        lookAction.AddBinding("<Gamepad>/rightStick");

        zoomAction = new InputAction("Zoom");
        zoomAction.AddBinding("<Mouse>/scroll");

        clickAction = new InputAction("Click", binding: "<Mouse>/rightButton");
    }

    void OnEnable()
    {
        lookAction.Enable();
        zoomAction.Enable();
        clickAction.Enable();
    }

    void OnDisable()
    {
        lookAction.Disable();
        zoomAction.Disable();
        clickAction.Disable();
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        offsetDistanceY = transform.position.y;

        // Lock and hide cursor if option isn't checked
        if (!clickToMoveCamera)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }

    // Changed to LateUpdate to prevent camera jittering when following the player
    void LateUpdate()
    {
        if (player == null) return;

        // Follow player - camera offset
        transform.position = player.position + new Vector3(0, offsetDistanceY, 0);

        // Set camera zoom when mouse wheel is scrolled
        if (canZoom)
        {
            float scrollAmount = zoomAction.ReadValue<Vector2>().y;
            if (scrollAmount != 0)
            {
                // We use Mathf.Sign to just get a +1 or -1, keeping zoom speed consistent
                Camera.main.fieldOfView -= Mathf.Sign(scrollAmount) * sensitivity * 2;

                // Keep the Field of View within normal visual limits
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 20f, 90f);
            }
        }

        // Checker for right click to move camera
        if (clickToMoveCamera && !clickAction.IsPressed())
            return;

        // Read input
        Vector2 lookDelta = lookAction.ReadValue<Vector2>();

        // Calculate new position. 
        // We multiply by 0.1f because the New Input System's raw mouse delta is much larger than the old system.
        mouseX += lookDelta.x * sensitivity * 0.1f;
        mouseY += lookDelta.y * sensitivity * 0.1f;

        // Apply camera limits
        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);

        transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0);
    }
}