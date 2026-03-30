using UnityEngine;

/// <summary>
/// First Person Camera Controller
/// Features: Mouse Look, Camera Tilt (slide/grapple), Head Bob
/// วิธี Setup:
///   - Player GameObject มี PlayerController + GrapplingHook
///   - Child: "CameraHolder" (transform ที่ script นี้อยู่)
///   - Child of CameraHolder: Camera
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public float verticalClamp = 85f;            // จำกัดมองบน/ล่าง

    [Header("Smoothing")]
    public float lookSmoothing = 20f;            // ยิ่งมาก = ยิ่ง responsive (0 = ไม่ smooth)
    public bool useSmoothing = true;

    [Header("Camera Tilt")]
    public float slideTilt = 8f;                 // องศาเอียงขณะ slide
    public float grappleTilt = 5f;               // องศาเอียงขณะ grapple
    public float tiltSpeed = 8f;

    [Header("Head Bob (optional)")]
    public bool enableHeadBob = true;
    public float bobFrequency = 8f;              // ความถี่ bob
    public float bobAmplitude = 0.04f;           // ความแรง bob
    public float bobSmoothing = 10f;

    [Header("References")]
    public Transform playerBody;                 // Transform ของ Player Body (หมุน Y)

    // ── Components ──────────────────────────────────────────────
    private PlayerController _playerController;
    private GrapplingHook _grapplingHook;

    // ── State ───────────────────────────────────────────────────
    private float _targetPitch;      // X rotation (บน/ล่าง)
    private float _currentPitch;
    private float _currentTilt;      // Z rotation (เอียง)

    // Head bob
    private float _bobTimer;
    private Vector3 _defaultLocalPos;

    // ────────────────────────────────────────────────────────────
    void Awake()
    {
        // ล็อค cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _defaultLocalPos = transform.localPosition;

        // หา component จาก parent
        if (playerBody != null)
        {
            _playerController = playerBody.GetComponent<PlayerController>();
            _grapplingHook = playerBody.GetComponent<GrapplingHook>();
        }
    }

    void Update()
    {
        HandleMouseLook();
        HandleCameraTilt();
        HandleHeadBob();
    }

    // ────────────────────────────────────────────────────────────
    #region Mouse Look
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        // หมุน player body แนวนอน (Yaw)
        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);

        // หมุน camera แนวตั้ง (Pitch) — clamp ไม่ให้คอหัก
        _targetPitch -= mouseY;
        _targetPitch = Mathf.Clamp(_targetPitch, -verticalClamp, verticalClamp);

        // Smooth หรือ snap
        if (useSmoothing)
            _currentPitch = Mathf.Lerp(_currentPitch, _targetPitch, lookSmoothing * Time.deltaTime);
        else
            _currentPitch = _targetPitch;
    }
    #endregion

    // ────────────────────────────────────────────────────────────
    #region Camera Tilt
    void HandleCameraTilt()
    {
        float targetTilt = 0f;

        if (_playerController != null)
        {
            if (_playerController.IsSliding)
                targetTilt = slideTilt;
            else if (_grapplingHook != null && _grapplingHook.IsGrappling)
                targetTilt = grappleTilt;
        }

        _currentTilt = Mathf.Lerp(_currentTilt, targetTilt, tiltSpeed * Time.deltaTime);

        // Apply rotation: Pitch (X) + Tilt (Z)
        transform.localRotation = Quaternion.Euler(_currentPitch, 0f, -_currentTilt);
    }
    #endregion

    // ────────────────────────────────────────────────────────────
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
            float bobY = Mathf.Sin(_bobTimer * 2f) * bobAmplitude * 0.5f; // Y bob เร็วกว่า 2x

            Vector3 targetBobPos = _defaultLocalPos + new Vector3(bobX, bobY, 0f);
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetBobPos,
                bobSmoothing * Time.deltaTime
            );
        }
        else
        {
            // ค่อยๆ กลับตำแหน่ง default
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                _defaultLocalPos,
                bobSmoothing * Time.deltaTime
            );
        }
    }
    #endregion
}