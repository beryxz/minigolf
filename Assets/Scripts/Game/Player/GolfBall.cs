using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(AudioSource))]
public class GolfBall : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float ballDirectionChangeSpeed = 60f;
    public Vector3 PointingDirection { private set; get; }
    private Vector3 previousPointingDirection;
    private Vector3 previousPosition;
    private Quaternion previousRotation;

    private Rigidbody rbody;
    private TrailRenderer trailRenderer;
    private AudioSource audioSource;

    private bool isActive;
    /// <summary>
    /// if ball has some velocity
    /// </summary>
    private bool isRolling;
    /// <summary>
    /// countdown phase after ball is launched. This helps give the ball some time to start rolling before checking if it is still.
    /// </summary>
    private bool isRollingCountdown;

    private GameController gameController;

    [Header("Debug Variables")]
    [SerializeField] private float linearVelocity;
    [SerializeField] private float angularVelocity;


    [Header("Ground collision")]
    [SerializeField] private float groundCheckRadius = 0.06f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Sound effects")]
    [SerializeField] private ImpactSoundEvent wallHitSoundEvent;
    [SerializeField] private ImpactSoundEvent groundHitSoundEvent;


    void Awake()
    {
        gameController = FindFirstObjectByType<GameController>();

        rbody = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        audioSource = GetComponent<AudioSource>();

        isRolling = false;
        isActive = false;
    }

    void Update()
    {
        // debug utils
        linearVelocity = rbody.linearVelocity.magnitude;
        angularVelocity = rbody.angularVelocity.magnitude;

        if (!isActive) return;

        if (isRolling)
        {
            if (!isRollingCountdown && rbody.linearVelocity.magnitude <= 0.01 && rbody.angularVelocity.magnitude <= 0.01)
            {
                trailRenderer.emitting = false;
                rbody.Sleep();
                rbody.linearVelocity = Vector3.zero;
                rbody.angularVelocity = Vector3.zero;
                isRolling = false;
            }
            return;
        }

        // change ball direction
        if (!gameController.IsPaused)
        {
            float angle = Input.GetAxis("Mouse X") * ballDirectionChangeSpeed * Time.deltaTime;
            PointingDirection = Quaternion.AngleAxis(angle, Vector3.up) * PointingDirection;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("WallSound"))
        {
            // Debug.Log($"CollisionEnter [Wall] Relative velocity: {collision.relativeVelocity.magnitude}");
            wallHitSoundEvent.PlayOneShot(audioSource, collision.relativeVelocity.magnitude);
        }
        else if (collision.gameObject.CompareTag("GroundSound"))
        {
            // Debug.Log($"CollisionEnter [Ground] Relative velocity: {collision.relativeVelocity.magnitude}");
            groundHitSoundEvent.PlayOneShot(audioSource, collision.relativeVelocity.magnitude);
        }
    }

    /// <summary>
    /// Set the direction in which the ball is pointing at. The direction is then normalized
    /// </summary>
    /// <param name="directionPoint">A point respective to the ball position to represent the direction</param>
    public void SetPointingDirection(Vector3 directionPoint)
    {
        PointingDirection = (directionPoint - transform.position).normalized;
    }

    /// <summary>
    /// Apply a force to the ball in the current this.pointingDirection
    /// </summary>
    /// <param name="throwingForce">the force of the throw</param>
    public void ThrowBall(float throwingForce)
    {
        if (IsRolling()) return;

        previousPointingDirection = PointingDirection;
        transform.GetPositionAndRotation(out previousPosition, out previousRotation);

        trailRenderer.emitting = true;
        isRolling = true;
        isRollingCountdown = true;
        rbody.AddForce(PointingDirection * throwingForce, ForceMode.Impulse);
        Invoke(nameof(StopRollingCountdown), 1f);
    }

    private void StopRollingCountdown()
    {
        isRollingCountdown = false;
    }

    public bool IsRolling()
    {
        return isRolling;
    }

    /// <summary>
    /// Check if the ball is currently in the vicinity of any object in the "Ground" layer.
    /// </summary>
    /// <returns>true if in the vicinity, false otherwise</returns>
    public bool IsTouchingGround()
    {
        return Physics.OverlapSphere(transform.position, groundCheckRadius, groundLayer).Any();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }

    /// <summary>
    /// Stops the balls and resets its position to the previous one (if present)
    /// </summary>
    public void ResetBall()
    {
        // Stop ball and reset its velocities
        rbody.Sleep();
        rbody.linearVelocity = Vector3.zero;
        rbody.angularVelocity = Vector3.zero;

        // Stop possible invoke of StopRollingCountdown
        CancelInvoke();

        // Reset position and rotation
        if (previousPointingDirection != null)
        {
            PointingDirection = previousPointingDirection;
        }
        if (previousPosition != null && previousRotation != null)
        {
            transform.SetPositionAndRotation(previousPosition, previousRotation);
        }

        // Set rolling status
        isRolling = false;
        isRollingCountdown = false;
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }
}
