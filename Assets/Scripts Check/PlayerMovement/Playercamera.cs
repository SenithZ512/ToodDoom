using UnityEngine;

/// <summary>
/// First Person Camera Controller
/// Features: Mouse Look, Camera Tilt (slide/grapple), Head Bob
/// วิธี Setup:
///   - Player GameObject มี PlayerController + Grappling
///   - Child: "CameraHolder" (transform ที่ script นี้อยู่)
///   - Child of CameraHolder: Camera
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public float verticalClamp = 85f;

    [Header("Smoothing")]
    public float lookSmoothing = 20f;
    public bool useSmoothing = true;

    [Header("Camera Tilt")]
    public float slideTilt = 8f;
    public float grappleTilt = 5f;
    public float tiltSpeed = 8f;

    [Header("Head Bob (optional)")]
    public bool enableHeadBob = true;
    public float bobFrequency = 8f;
    public float bobAmplitude = 0.04f;
    public float bobSmoothing = 10f;

    [Header("References")]
    public Transform playerBody;

    // ── Components ──────────────────────────────────────────────
    private PlayerController _playerController;
    private Grappling _grappling;  // ← แก้จาก GrapplingHook เป็น Grappling

    // ── State ───────────────────────────────────────────────────
    private float _targetPitch;
    private float _currentPitch;
    private float _currentTilt;

    // Head bob
    private float _bobTimer;
    private Vector3 _defaultLocalPos;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _defaultLocalPos = transform.localPosition;

        if (playerBody != null)
        {
            _playerController = playerBody.GetComponent<PlayerController>();
            _grappling = playerBody.GetComponent<Grappling>();  // ← แก้ชื่อ
        }
    }

    void Update()
    {
        HandleMouseLook();
        HandleCameraTilt();
        HandleHeadBob();
    }

    #region Mouse Look
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);

        _targetPitch -= mouseY;
        _targetPitch = Mathf.Clamp(_targetPitch, -verticalClamp, verticalClamp);

        if (useSmoothing)
            _currentPitch = Mathf.Lerp(_currentPitch, _targetPitch, lookSmoothing * Time.deltaTime);
        else
            _currentPitch = _targetPitch;
    }
    #endregion

    #region Camera Tilt
    void HandleCameraTilt()
    {
        float targetTilt = 0f;

        if (_playerController != null)
        {
            if (_playerController.IsSliding)
                targetTilt = slideTilt;
            else if (_grappling != null && _grappling.IsGrappling())  // ← เรียกเป็น method ()
                targetTilt = grappleTilt;
        }

        _currentTilt = Mathf.Lerp(_currentTilt, targetTilt, tiltSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(_currentPitch, 0f, -_currentTilt);
    }
    #endregion

    #region Head Bob
    void HandleHeadBob()
    {
        if (!enableHeadBob || _playerController == null) return;

        bool isMovingOnGround = _playerController.IsGrounded &&
                                _playerController.CurrentVelocity.magnitude > 0.5f;

        if (isMovingOnGround)
        {
            _bobTimer += Time.deltaTime * bobFrequency;

            float bobX = Mathf.Sin(_bobTimer) * bobAmplitude;
            float bobY = Mathf.Sin(_bobTimer * 2f) * bobAmplitude * 0.5f;

            Vector3 targetBobPos = _defaultLocalPos + new Vector3(bobX, bobY, 0f);
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetBobPos,
                bobSmoothing * Time.deltaTime
            );
        }
        else
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                _defaultLocalPos,
                bobSmoothing * Time.deltaTime
            );
        }
    }
    #endregion
}