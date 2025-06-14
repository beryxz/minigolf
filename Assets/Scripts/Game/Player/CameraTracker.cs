using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    public Transform trackedTarget;
    private GolfBall trackedBall;

    [Header("Camera Movement Settings")]
    public float cameraDistanceOffset = 3.0f;
    public float cameraDistanceOffsetMin = 1.0f;
    public float cameraDistanceOffsetMax = 5.0f;
    public float cameraHeightOffset = 2.0f;
    public float cameraHeightOffsetMin = 0.4f;
    public float cameraHeightOffsetMax = 6.0f;
    public float cameraZoomSpeed = 50.0f;

    public bool IsActive { get; private set; }

    private GameController gameController;


    void Awake()
    {
        gameController = FindFirstObjectByType<GameController>();

        IsActive = false;
    }

    void Start()
    {
        SetTrackedTarget(trackedTarget);
    }

    void Update()
    {
        if (!IsActive || trackedBall == null || trackedTarget == null) return;

        // change camera distance
        if (!gameController.IsPaused)
        {
            cameraDistanceOffset += -1 * Input.GetAxis("Mouse ScrollWheel") * cameraZoomSpeed * Time.deltaTime;
        }
        cameraDistanceOffset = Mathf.Clamp(cameraDistanceOffset, cameraDistanceOffsetMin, cameraDistanceOffsetMax);
        // change camera height
        cameraHeightOffset = Mathf.Lerp(
            cameraHeightOffsetMin,
            cameraHeightOffsetMax,
            Mathf.InverseLerp(cameraDistanceOffsetMin, cameraDistanceOffsetMax, cameraDistanceOffset));

        UpdateCameraPosition();
    }

    public void SetActive(bool value)
    {
        IsActive = value;
    }

    public void UpdateCameraPosition()
    {
        // change position based on
        Vector3 newPos = -1 * cameraDistanceOffset * trackedBall.PointingDirection;
        newPos.y = cameraHeightOffset;
        transform.position = trackedBall.transform.position + newPos;
        // Change rotation to look at ball
        transform.LookAt(trackedTarget);
    }

    public void SetTrackedTarget(Transform newTarget)
    {
        trackedTarget = newTarget;
        trackedBall = trackedTarget.gameObject.GetComponent<GolfBall>();
        if (trackedBall == null) Debug.LogWarning("The tracked target is not a GolfBall", this);
    }
}
