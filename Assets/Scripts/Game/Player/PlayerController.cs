using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    private GameController gameController;
    private PlayerUIController playerUI;

    [Header("Ball components")]
    public GolfBall ball;
    private LineRenderer throwingLine;
    [SerializeField] private Transform ballCircle;

    [Header("Throwing settings")]
    [SerializeField] private float throwingForce = 0.5f;
    [SerializeField] private float throwingForceMin = 0.5f;
    [SerializeField] private float throwingForceMax = 15.0f;
    [SerializeField] private float throwingForceChangeSpeed = 10.0f;

    [Header("Sound effects")]
    [SerializeField] private SoundEvent ballThrownLowSoundEvent;
    [SerializeField] private SoundEvent ballThrownMediumSoundEvent;
    [SerializeField] private SoundEvent ballThrownHighSoundEvent;
    private AudioSource audioSource;

    private bool isCurrentTurn;
    private bool isWaitingBallToStop;

    void Awake()
    {
        gameController = FindFirstObjectByType<GameController>();
        playerUI = gameController.GetComponent<PlayerUIController>();
        audioSource = GetComponent<AudioSource>();

        throwingLine = ball.GetComponent<LineRenderer>();
        if (throwingLine == null) Debug.Log("Expected a player ball with a LineRenderer", this);

        isCurrentTurn = false;
    }

    void Start()
    {
        throwingLine.positionCount = 2;
        throwingLine.SetPosition(0, new Vector3(0, 0, 0));
    }

    void Update()
    {
        // wait current turn
        if (!isCurrentTurn) return;
        // after throw, wait for ball to stop
        if (ball.IsRolling())
        {
            return;
        }
        // if ball stopped after it was thrown
        if (isWaitingBallToStop && !ball.IsRolling())
        {
            isWaitingBallToStop = false;
            if (!ball.IsTouchingGround()) ball.ResetBall();
            gameController.HandlePlayerBallStop();
        }


        // === BEGIN CURRENT TURN ===

        // show direction line
        throwingLine.SetPosition(0, ball.transform.position);
        throwingLine.SetPosition(1, ball.transform.position + ball.PointingDirection * (Mathf.InverseLerp(throwingForceMin, throwingForceMax, throwingForce)+0.4f));

        // show ball circle
        ballCircle.position = ball.transform.position + new Vector3(0f, -0.03f, 0f);

        // update throwing force & powermeter
        if (Input.GetMouseButton((int)MouseButton.Left))
        {
            throwingForce = Mathf.Clamp(throwingForce + throwingForceChangeSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime, throwingForceMin, throwingForceMax);
        }
        playerUI.SetPowerMeterLevel((throwingForce - throwingForceMin) / (throwingForceMax - throwingForceMin));

        // handle actions keys
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown((int)MouseButton.Right))
        {
            isWaitingBallToStop = true;
            throwingLine.enabled = false;
            gameController.HandlePlayerBallThrow();
        }
    }

    private void PlayThrowBallSound(float throwingForce)
    {
        if (throwingForce <= (throwingForceMin + (throwingForceMax - throwingForceMin) * 0.25f))
        {
            ballThrownLowSoundEvent.PlayOneShot(audioSource);
        }
        else if (throwingForce >= (throwingForceMin + (throwingForceMax - throwingForceMin) * 0.75f))
        {
            ballThrownHighSoundEvent.PlayOneShot(audioSource);
        }
        else
        {
            ballThrownMediumSoundEvent.PlayOneShot(audioSource);
        }
    }

    public void ThrowBall()
    {
        ballCircle.gameObject.SetActive(false);
        PlayThrowBallSound(throwingForce);
        ball.ThrowBall(throwingForce);
    }

    public void SetCurrentTurn(bool value)
    {
        isCurrentTurn = value;
        throwingLine.enabled = value;
        ballCircle.gameObject.SetActive(value);
        ball.SetActive(value);
    }
}
