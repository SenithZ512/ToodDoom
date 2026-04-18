using UnityEngine;

<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs
=======
/// <summary>
/// First Person Player Controller
/// Features: Double Jump, Slide, Sprint, FOV Transition, Velocity Preservation
/// ต้องการ: CharacterController component บน GameObject เดียวกัน
/// </summary>
>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 7f;
    public float airControlMultiplier = 0.3f;
    public float groundFriction = 12f;
    public float airFriction = 0.5f;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float doubleJumpForce = 7f;
    public float gravity = -25f;
    public float fallMultiplier = 2.2f;

    [Header("Slide Settings")]
    public float slideForce = 14f;
    public float slideDuration = 0.6f;
    public float slideHeightReduction = 0.5f;
    public float slideFriction = 1.5f;
    public float slideSpeedThreshold = 3f;
<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs



=======
>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs

    [Header("References")]
    public Transform cameraHolder;

<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs

    [Header("Sprint Settings")]
    public float sprintSpeed = 40f;          // ความเร็วตอนวิ่ง
    public float sprintFOV = 90f;            // FOV ตอนวิ่ง
    public float normalFOV = 60f;            // FOV ปกติ
    public float fovTransitionSpeed = 8f;    // ความเร็วเปลี่ยน FOV
=======
    [Header("Sprint Settings")]
    public float sprintSpeed = 40f;
    public float sprintFOV = 90f;
    public float normalFOV = 60f;
    public float fovTransitionSpeed = 8f;
>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs

    private CharacterController _cc;

<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs
=======
    // ── Velocity ─────────────────────────────────────────────────
>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs
    [HideInInspector] public Vector3 horizontalVelocity;
    [HideInInspector] public float verticalVelocity;
    [HideInInspector] public Vector3 externalVelocity;

    private bool _isGrounded;
    private bool _wasGrounded;
    private bool _hasDoubleJump;
    private bool _isSliding;
    private float _slideTimer;
    private float _defaultHeight;
    private float _defaultCameraY;
    private Vector3 _defaultCenter;
    private Vector3 _slideDirection;

    private float _coyoteTime = 0.12f;
    private float _coyoteTimer;
    private float _jumpBufferTime = 0.15f;
    private float _jumpBufferTimer;

    private Vector2 _moveInput;
    private bool _slidePressed;

<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs
=======
    // ── Properties ──────────────────────────────────────────────
>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs
    public bool IsGrounded => _isGrounded;
    public bool IsSliding => _isSliding;
    public Vector3 CurrentVelocity => horizontalVelocity + Vector3.up * verticalVelocity + externalVelocity;

<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs

=======
>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs
    private bool _isSprinting;
    private Camera _cam;

    void Awake()
    {
<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs
<<<<<<< Updated upstream
=======


>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs
      
        
            _cc = GetComponent<CharacterController>();
            _defaultHeight = _cc.height;
            _defaultCenter = _cc.center;
            _cam = Camera.main;                  // ← เพิ่มบรรทัดนี้

            if (cameraHolder != null)
                _defaultCameraY = cameraHolder.localPosition.y;
        
<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs
=======
=======
>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs


        _cc = GetComponent<CharacterController>();
        _defaultHeight = _cc.height;
        _defaultCenter = _cc.center;
<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs
        _cam = Camera.main;                  // ← เพิ่มบรรทัดนี้
=======
        _cam = Camera.main;
>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs

        if (cameraHolder != null)
            _defaultCameraY = cameraHolder.localPosition.y;

<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs
>>>>>>> Stashed changes
=======



>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs
    }

    void Update()
    {
        GatherInput();
        CheckGround();
        HandleJump();
        HandleSlide();
        ApplyMovement();
        ApplyGravity();
        DecayExternalVelocity();
        MoveCharacter();
    }

    #region Input
    void GatherInput()
    {
        _moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetButtonDown("Jump"))
            _jumpBufferTimer = _jumpBufferTime;
        else
            _jumpBufferTimer -= Time.deltaTime;

        _slidePressed = Input.GetKeyDown(KeyCode.LeftControl);

<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs
        // ✅ Sprint
=======
>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs
        _isSprinting = Input.GetKey(KeyCode.LeftShift) && _isGrounded && _moveInput.y > 0;
    }
    #endregion

    #region Ground Check
    void CheckGround()
    {
        _wasGrounded = _isGrounded;
        _isGrounded = _cc.isGrounded;

        if (_isGrounded)
        {
            _coyoteTimer = _coyoteTime;
            _hasDoubleJump = true;
<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs
=======

>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs
            if (!_wasGrounded)
                OnLanded();
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }
    }

    void OnLanded()
    {
        externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, 0.5f);
    }
    #endregion

    #region Jump & Double Jump
    void HandleJump()
    {
        bool canJumpFromGround = _coyoteTimer > 0f;
        bool jumpRequested = _jumpBufferTimer > 0f;

        if (jumpRequested)
        {
            if (canJumpFromGround)
            {
                PerformJump(jumpForce);
                _coyoteTimer = 0f;
                _jumpBufferTimer = 0f;
            }
            else if (_hasDoubleJump)
            {
                PerformDoubleJump();
                _jumpBufferTimer = 0f;
            }
        }
    }

    void PerformJump(float force)
    {
        verticalVelocity = force;
        if (_isSliding) StopSlide();
    }

    void PerformDoubleJump()
    {
        verticalVelocity = doubleJumpForce;
        _hasDoubleJump = false;

        Vector3 inputDir = GetInputDirection();
        if (inputDir.magnitude > 0.1f)
            horizontalVelocity += inputDir * 2f;
    }
    #endregion

    #region Slide
    void HandleSlide()
    {
        if (_slidePressed && _isGrounded && !_isSliding)
        {
            // ✅ ใช้ speed จาก frame ก่อนหน้า ไม่ใช่ frame ที่กด Shift
            float speed = horizontalVelocity.magnitude + externalVelocity.magnitude;
            Debug.Log($"Slide attempt | speed: {speed} | grounded: {_isGrounded}");

            if (speed >= slideSpeedThreshold)
                StartSlide();
            else
<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs
                // slide แม้ความเร็วน้อย (optional)
                StartSlide(); // ← ลบ threshold ออกชั่วคราวเพื่อทดสอบ
=======
                StartSlide();
>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs
        }

        if (_isSliding)
        {
            _slideTimer -= Time.deltaTime;

            bool cancelByKey = Input.GetKeyDown(KeyCode.LeftShift);
            bool cancelByJump = _jumpBufferTimer > 0f;

            if (_slideTimer <= 0f || !_isGrounded || cancelByKey || cancelByJump)
                StopSlide();
        }
    }

    void StartSlide()
    {
        _isSliding = true;
        _slideTimer = slideDuration;

        _slideDirection = horizontalVelocity.normalized;
        if (_slideDirection == Vector3.zero)
            _slideDirection = transform.forward;

<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs
        // ✅ เพิ่มจาก 0.3f เป็น 1.5f = พุ่งแรงขึ้นมาก
=======
>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs
        horizontalVelocity += _slideDirection * slideForce * 1.5f;

        float newHeight = _defaultHeight * slideHeightReduction;
        float heightDiff = _defaultHeight - newHeight;

        _cc.height = newHeight;
        _cc.center = new Vector3(0, _defaultCenter.y - heightDiff / 2f, 0);

        if (cameraHolder != null)
        {
            float targetY = _defaultCameraY - heightDiff;
            cameraHolder.localPosition = new Vector3(
                cameraHolder.localPosition.x,
                targetY,
                cameraHolder.localPosition.z
            );
        }
    }

    void StopSlide()
    {
        _isSliding = false;

        _cc.height = _defaultHeight;
        _cc.center = _defaultCenter;

        if (cameraHolder != null)
        {
            cameraHolder.localPosition = new Vector3(
                cameraHolder.localPosition.x,
                _defaultCameraY,
                cameraHolder.localPosition.z
            );
        }
    }
    #endregion

    #region Movement & Velocity
    void ApplyMovement()
    {
        Vector3 inputDir = GetInputDirection();
        float currentSpeed = _isSprinting ? sprintSpeed : walkSpeed;

        // ✅ เลือก speed ตาม sprint
        float currentSpeed = _isSprinting ? sprintSpeed : walkSpeed;

        if (_isSliding)
        {
            horizontalVelocity = Vector3.Lerp(
                horizontalVelocity,
                Vector3.zero,
                slideFriction * Time.deltaTime
            );
        }
        else if (_isGrounded)
        {
            Vector3 targetVelocity = inputDir * currentSpeed;
            horizontalVelocity = Vector3.Lerp(
                horizontalVelocity,
                targetVelocity,
                groundFriction * Time.deltaTime
            );
        }
        else
        {
            Vector3 targetVelocity = inputDir * currentSpeed;
            horizontalVelocity = Vector3.Lerp(
                horizontalVelocity,
                targetVelocity,
                airControlMultiplier * groundFriction * Time.deltaTime
            );

            horizontalVelocity = Vector3.Lerp(
                horizontalVelocity,
                Vector3.zero,
                airFriction * Time.deltaTime
            );
        }

<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs
        // ✅ FOV transition
=======
>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs
        if (_cam != null)
        {
            float targetFOV = _isSprinting ? sprintFOV : normalFOV;
            _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, targetFOV, fovTransitionSpeed * Time.deltaTime);
        }
    }

    void ApplyGravity()
    {
        if (_isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
            return;
        }

        float gravityScale = verticalVelocity < 0f ? fallMultiplier : 1f;
        verticalVelocity += gravity * gravityScale * Time.deltaTime;
        verticalVelocity = Mathf.Max(verticalVelocity, -50f);
    }

    void DecayExternalVelocity()
    {
        float decayRate = _isGrounded ? 6f : 2f;
        externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, decayRate * Time.deltaTime);
    }

    void MoveCharacter()
    {
        Vector3 totalVelocity = horizontalVelocity + externalVelocity + Vector3.up * verticalVelocity;
        _cc.Move(totalVelocity * Time.deltaTime);
    }

    Vector3 GetInputDirection()
    {
        Vector3 dir = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        return dir.magnitude > 1f ? dir.normalized : dir;
    }
    #endregion

    #region Public API
<<<<<<< HEAD:Assets/Scripts Check/PlayerController.cs
=======
    /// <summary>เพิ่ม velocity จากภายนอก เช่น Grappling Hook</summary>
>>>>>>> Oatlol:Assets/Scripts Check/PlayerMovement/PlayerController.cs
    public void AddExternalVelocity(Vector3 velocity)
    {
        externalVelocity += velocity;
    }

    public void SetExternalVelocity(Vector3 velocity)
    {
        externalVelocity = velocity;
    }

    public void ForceStopSlide()
    {
        if (_isSliding) StopSlide();
    }
    #endregion
}