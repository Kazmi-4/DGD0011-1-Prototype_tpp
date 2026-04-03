using UnityEngine;

public class ThirdPersonEyeFollow : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How smoothly the Eye turns to look at where you are aiming.")]
    public float smoothness = 5f;

    [Tooltip("Which layers the Eye should track (Set to Everything, or just your Ground/Walls).")]
    public LayerMask trackingLayers = ~0; // ~0 means "Everything"

    private Transform mainCamera;

    void Start()
    {
        // Find the main camera automatically
        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Eye of Sauron: I cannot find the Main Camera!");
        }
    }

    void Update()
    {
        if (mainCamera == null) return;

        // 1. Shoot a ray straight out from the exact center of the camera (your "cursor")
        Ray ray = new Ray(mainCamera.position, mainCamera.forward);
        Vector3 lookTarget;

        // 2. See where the camera is looking in the 3D world
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, trackingLayers))
        {
            // If the ray hits the terrain, the Eye looks at that exact spot
            lookTarget = hitInfo.point;
        }
        else
        {
            // If you are looking up at the sky, the Eye just looks far away into the distance
            lookTarget = ray.GetPoint(1000f);
        }

        // 3. Calculate the direction from the Eye to the target
        Vector3 directionToTarget = lookTarget - transform.position;

        // 4. Smoothly rotate the Eye to face that direction
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothness);
        }
    }
}