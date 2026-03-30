using UnityEngine;

/// <summary>
/// First Person Player Controller
/// Features: Double Jump, Slide, Velocity Preservation
/// ต้องการ: CharacterController component บน GameObject เดียวกัน
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 7f;
    public float airControlMultiplier = 0.3f;   // ควบคุมได้น้อยลงขณะลอยอยู่
    public float groundFriction = 12f;           // ความฝืดพื้น
    public float airFriction = 0.5f;             // ความฝืดอากาศ (น้อย = ลื่น = รู้สึก momentum)

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float doubleJumpForce = 7f;
    public float gravity = -25f;                  // แรงโน้มถ่วงเอง (หนักกว่า default ทำให้กระโดดรู้สึก snappy)
    public float fallMultiplier = 2.2f;           // ตกเร็วกว่าขาขึ้น (เกมรู้สึก responsive)

    [Header("Slide Settings")]
    public float slideForce = 14f;
    public float slideDuration = 0.6f;
    public float slideHeightReduction = 0.5f;    // ลด CharacterController height ขณะ slide
    public float slideFriction = 1.5f;           // ฝืดขณะ slide (น้อยกว่า ground)
    public float slideSpeedThreshold = 3f;       // ความเร็วขั้นต่ำที่จะ slide ได้

    [Header("References")]
    public Transform cameraHolder;               // Transform ที่ Camera อยู่ใน

    // ── Components ──────────────────────────────────────────────
    private CharacterController _cc;

    // ── Velocity (หัวใจหลักของระบบ) ─────────────────────────────
    // แยก horizontal กับ vertical เพื่อควบคุมแต่ละส่วนได้อิสระ
    [HideInInspector] public Vector3 horizontalVelocity;   // XZ velocity
    [HideInInspector] public float verticalVelocity;        // Y velocity

    // External velocity เช่นจาก Grappling Hook หรือ knockback
    [HideInInspector] public Vector3 externalVelocity;

    // ── State ───────────────────────────────────────────────────
    private bool _isGrounded;
    private bool _wasGrounded;
    private bool _hasDoubleJump;
    private bool _isSliding;
    private float _slideTimer;
    private float _defaultHeight;
    private float _defaultCameraY;
    private Vector3 _slideDirection;

    // Coyote time: กระโดดได้แม้เพิ่ง walk off หน้าผา
    private float _coyoteTime = 0.12f;
    private float _coyoteTimer;

    // Jump buffer: กด jump ก่อนถึงพื้นนิดนึงก็กระโดดได้
    private float _jumpBufferTime = 0.15f;
    private float _jumpBufferTimer;

    // ── Input Cache ─────────────────────────────────────────────
    private Vector2 _moveInput;
    private bool _jumpPressed;
    private bool _slidePressed;

    // ── Properties สำหรับ script อื่นอ่านได้ ────────────────────
    public bool IsGrounded => _isGrounded;
    public bool IsSliding => _isSliding;
    public Vector3 CurrentVelocity => horizontalVelocity + Vector3.up * verticalVelocity + externalVelocity;

    // ────────────────────────────────────────────────────────────
    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _defaultHeight = _cc.height;

        if (cameraHolder != null)
            _defaultCameraY = cameraHolder.localPosition.y;
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

    // ────────────────────────────────────────────────────────────
    #region Input
    void GatherInput()
    {
        _moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Jump buffer: เก็บ input กด jump ไว้ชั่วคราว
        if (Input.GetButtonDown("Jump"))
            _jumpBufferTimer = _jumpBufferTime;
        else
            _jumpBufferTimer -= Time.deltaTime;

        _slidePressed = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C);
    }
    #endregion

    // ────────────────────────────────────────────────────────────
    #region Ground Check
    void CheckGround()
    {
        _wasGrounded = _isGrounded;
        _isGrounded = _cc.isGrounded;

        // Coyote time counter
        if (_isGrounded)
        {
            _coyoteTimer = _coyoteTime;
            _hasDoubleJump = true; // รีเซ็ต double jump เมื่อแตะพื้น

            // Landing: ลด horizontal velocity ด้วย external ที่ค้างอยู่
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
        // เมื่อ land ให้ external velocity หายไปเร็วขึ้น (ไม่งั้น grapple velocity ค้างบนพื้น)
        externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, 0.5f);
    }
    #endregion

    // ────────────────────────────────────────────────────────────
    #region Jump & Double Jump
    void HandleJump()
    {
        bool canJumpFromGround = _coyoteTimer > 0f;   // รวม coyote time
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

        // ยกเลิก slide ถ้ากำลัง slide อยู่
        if (_isSliding)
            StopSlide();
    }

    void PerformDoubleJump()
    {
        verticalVelocity = doubleJumpForce;
        _hasDoubleJump = false;

        // Double jump เพิ่ม horizontal speed นิดนึงตามทิศที่กด (รู้สึก snappy)
        Vector3 inputDir = GetInputDirection();
        if (inputDir.magnitude > 0.1f)
            horizontalVelocity += inputDir * 2f;
    }
    #endregion

    // ────────────────────────────────────────────────────────────
    #region Slide
    void HandleSlide()
    {
        // เริ่ม slide: ต้องอยู่บนพื้น + เร็วพอ + กด key
        if (_slidePressed && _isGrounded && !_isSliding)
        {
            float speed = horizontalVelocity.magnitude + externalVelocity.magnitude;
            if (speed >= slideSpeedThreshold)
                StartSlide();
        }

        // Slide timer countdown
        if (_isSliding)
        {
            _slideTimer -= Time.deltaTime;
            if (_slideTimer <= 0f || !_isGrounded)
                StopSlide();
        }
    }

    void StartSlide()
    {
        _isSliding = true;
        _slideTimer = slideDuration;

        // ทิศ slide = ทิศที่กำลังวิ่ง (ล็อคทิศ ไม่หมุนตาม input)
        _slideDirection = horizontalVelocity.normalized;
        if (_slideDirection == Vector3.zero)
            _slideDirection = transform.forward;

        // เพิ่มความเร็วเมื่อเริ่ม slide
        horizontalVelocity += _slideDirection * slideForce * 0.3f;

        // ลดความสูง CharacterController
        _cc.height = _defaultHeight * slideHeightReduction;
        _cc.center = new Vector3(0, _cc.height / 2f, 0);

        // ลด camera (visual feedback)
        if (cameraHolder != null)
        {
            float targetY = _defaultCameraY - (_defaultHeight * (1 - slideHeightReduction));
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

        // คืนความสูง CharacterController
        _cc.height = _defaultHeight;
        _cc.center = new Vector3(0, _defaultHeight / 2f, 0);

        // คืน camera
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

    // ────────────────────────────────────────────────────────────
    #region Movement & Velocity
    void ApplyMovement()
    {
        Vector3 inputDir = GetInputDirection();

        if (_isSliding)
        {
            // ขณะ slide: friction น้อย + ไม่รับ input direction (ลื่นไปตามทิศเดิม)
            horizontalVelocity = Vector3.Lerp(
                horizontalVelocity,
                Vector3.zero,
                slideFriction * Time.deltaTime
            );
        }
        else if (_isGrounded)
        {
            // บนพื้น: เปลี่ยนความเร็วหา target speed พร้อม friction
            Vector3 targetVelocity = inputDir * walkSpeed;
            horizontalVelocity = Vector3.Lerp(
                horizontalVelocity,
                targetVelocity,
                groundFriction * Time.deltaTime
            );
        }
        else
        {
            // ในอากาศ: air control น้อยกว่า + air friction รักษา momentum
            Vector3 targetVelocity = inputDir * walkSpeed;
            horizontalVelocity = Vector3.Lerp(
                horizontalVelocity,
                targetVelocity,
                airControlMultiplier * groundFriction * Time.deltaTime
            );

            // Air friction: ค่อยๆ ลด velocity (ไม่ใช่ brake ทันที)
            horizontalVelocity = Vector3.Lerp(
                horizontalVelocity,
                Vector3.zero,
                airFriction * Time.deltaTime
            );
        }
    }

    void ApplyGravity()
    {
        if (_isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f; // กด character ลงพื้นเสมอ (ป้องกัน floating)
            return;
        }

        // Fall multiplier: ตกเร็วกว่าขาขึ้น
        float gravityScale = verticalVelocity < 0f ? fallMultiplier : 1f;
        verticalVelocity += gravity * gravityScale * Time.deltaTime;

        // Terminal velocity
        verticalVelocity = Mathf.Max(verticalVelocity, -50f);
    }

    void DecayExternalVelocity()
    {
        // External velocity (Grappling Hook ฯลฯ) ค่อยๆ หายไป
        float decayRate = _isGrounded ? 6f : 2f; // บนพื้นสลายเร็วกว่า
        externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, decayRate * Time.deltaTime);
    }

    void MoveCharacter()
    {
        Vector3 totalVelocity = horizontalVelocity + externalVelocity + Vector3.up * verticalVelocity;
        _cc.Move(totalVelocity * Time.deltaTime);
    }

    Vector3 GetInputDirection()
    {
        // แปลง input เป็น world direction ตาม facing ของ player (XZ plane)
        Vector3 dir = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        return dir.magnitude > 1f ? dir.normalized : dir;
    }
    #endregion

    // ────────────────────────────────────────────────────────────
    #region Public API (ให้ script อื่นเรียกได้)

    /// <summary>เพิ่ม velocity จากภายนอก เช่น Grappling Hook</summary>
    public void AddExternalVelocity(Vector3 velocity)
    {
        externalVelocity += velocity;
    }

    /// <summary>ตั้ง external velocity โดยตรง (override)</summary>
    public void SetExternalVelocity(Vector3 velocity)
    {
        externalVelocity = velocity;
    }

    /// <summary>ยกเลิก slide จากภายนอก</summary>
    public void ForceStopSlide()
    {
        if (_isSliding) StopSlide();
    }
    #endregion
}
