using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem; // Added the New Input System namespace

public class CameraManager : MonoBehaviour
{
    [Header("Assign your 3 Virtual Cameras here")]
    public CinemachineVirtualCamera cam1Follow;
    public CinemachineVirtualCamera cam2Action;
    public CinemachineVirtualCamera cam3Security;

    void Update()
    {
        // Safety check to ensure a keyboard is plugged in/detected
        if (Keyboard.current == null) return;

        // Press 1 for Standard Cam
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            cam1Follow.Priority = 10;
            cam2Action.Priority = 0;
            cam3Security.Priority = 0;
        }

        // Press 2 for Action Cam
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            cam1Follow.Priority = 0;
            cam2Action.Priority = 10;
            cam3Security.Priority = 0;
        }

        // Press 3 for Security Cam
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            cam1Follow.Priority = 0;
            cam2Action.Priority = 0;
            cam3Security.Priority = 10;
        }
    }
}