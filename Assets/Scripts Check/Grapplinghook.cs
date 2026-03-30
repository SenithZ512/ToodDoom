using UnityEngine;

/// <summary>
/// GrapplingHook v2 — ยิง Hook ไปติด Surface แล้วโหน
///
/// Controls:
///   E (กด)    = ยิง Hook ไปยังจุดที่ Crosshair ชี้
///   E (ถือ)   = โหนอยู่ (แกว่งได้ด้วย WASD)
///   E (ปล่อย) = ปล่อย Hook + เก็บ Momentum
///
/// Physics:
///   - Hook บินออกจากผู้เล่นไป Surface (Raycast)
///   - เมื่อติดแล้ว: ดึงผู้เล่นพร้อมโหน (Swing)
///   - ปล่อยแล้วได้ Momentum ต่อ
///
/// Setup:
///   1. ใส่ Script นี้บน Player (เดียวกับ PlayerController)
///   2. สร้าง LineRenderer บน Child Object → drag มาใส่ hookLine
///   3. hookOrigin = จุดยิง (ปลาย Camera หรือจุดกึ่งกลาง)
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class GrapplingHook : MonoBehaviour
{
    // ══════════════════════════════════════════════════════
    //  Inspector
    // ══════════════════════════════════════════════════════

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.E;

    [Header("Hook")]
    public float maxRange = 35f;     // ระยะยิงสูงสุด
    public float hookSpeed = 55f;     // ความเร็ว hook บิน

    [Tooltip("ติ๊กเฉพาะ Layer 'Wall' เท่านั้น — hook จะไม่ติด Floor / Enemy / อื่นๆ")]
    public LayerMask wallLayer;         // เฉพาะ Wall Layer เท่านั้น

    [Header("Swing Physics")]
    public float swingGravity = 20f;   // gravity ขณะโหน (ยิ่งมาก แกว่งเร็ว)
    public float reelingSpeed = 5f;    // กด W = ดึงเชือกสั้นลง (เพิ่มความเร็วแกว่ง)
    public float unreelSpeed = 4f;    // กด S = ปล่อยเชือกยาวขึ้น
    public float swingSideForce = 10f;   // กด A/D = แรงเสริมด้านข้าง
    public float minRopeLen = 3f;    // ความยาวเชือกสั้นสุด
    public float jumpOffForce = 6f;    // กด Space ขณะโหน = กระโดดออก

    [Header("Momentum")]
    public float releaseBoost = 1.1f;    // คูณ velocity เมื่อปล่อย

    [Header("Rope Visual")]
    public LineRenderer hookLine;
    public Transform hookOrigin;     // จุดเริ่มเชือก
    [Range(6, 32)]
    public int ropeSegments = 14;
    public float ropeSag = 0.12f;  // ความห้อยของเชือก

    [Header("Camera FOV")]
    public Camera playerCamera;
    public float grappleFOV = 88f;
    public float fovLerpSpeed = 9f;

    [Header("Crosshair Feedback (optional)")]
    public UnityEngine.UI.Image crosshairImage;
    public Color colorDefault = Color.white;
    public Color colorHooked = new Color(0.2f, 1f, 0.4f);

    // ══════════════════════════════════════════════════════
    //  State
    // ══════════════════════════════════════════════════════

    private enum GrappleState { Idle, Traveling, Hooked, Releasing }
    private GrappleState _state = GrappleState.Idle;

    private PlayerController _player;
    private Vector3 _hookPos;           // ตำแหน่ง hook ปัจจุบัน (visual)
    private Vector3 _anchorPoint;       // จุดที่ hook ติด wall
    private float _ropeLen;           // ความยาวเชือกปัจจุบัน
    private Vector3 _swingVelocity;     // velocity ของ player ขณะโหน (Pendulum)
    private float _wobbleTimer;
    private float _defaultFOV;

    // ══════════════════════════════════════════════════════
    //  Properties
    // ══════════════════════════════════════════════════════

    public bool IsGrappling => _state == GrappleState.Hooked;
    public bool IsActive => _state != GrappleState.Idle;

    // ══════════════════════════════════════════════════════
    //  Unity
    // ══════════════════════════════════════════════════════

    void Awake()
    {
        _player = GetComponent<PlayerController>();
        if (playerCamera != null) _defaultFOV = playerCamera.fieldOfView;
        SetLine(false);
        SetCrosshair(false);
    }

    void Update()
    {
        HandleInput();
        TickState();
        DrawRope();
        TickFOV();
    }

    // ══════════════════════════════════════════════════════
    //  Input
    // ══════════════════════════════════════════════════════

    void HandleInput()
    {
        if (Input.GetKeyDown(grappleKey) && _state == GrappleState.Idle)
            Shoot();

        if (Input.GetKeyUp(grappleKey) && _state != GrappleState.Idle)
            Release(boosted: true);
    }

    // ══════════════════════════════════════════════════════
    //  Shoot
    // ══════════════════════════════════════════════════════

    void Shoot()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        // ยิงได้เฉพาะเมื่อโดน Wall Layer เท่านั้น
        // ถ้าไม่โดน Wall → ยกเลิกทันที hook ไม่ออก
        if (!Physics.Raycast(ray, out RaycastHit hit, maxRange, wallLayer))
            return;

        _anchorPoint = hit.point;
        _hookPos = GetOrigin();
        _state = GrappleState.Traveling;
        SetLine(true);
    }

    // ══════════════════════════════════════════════════════
    //  State Ticks
    // ══════════════════════════════════════════════════════

    void TickState()
    {
        switch (_state)
        {
            case GrappleState.Traveling: TickTraveling(); break;
            case GrappleState.Hooked: TickHooked(); break;
            case GrappleState.Releasing: TickReleasing(); break;
        }
    }

    // ── Traveling: hook บินออกไป ─────────────────────────

    void TickTraveling()
    {
        _hookPos = Vector3.MoveTowards(_hookPos, _anchorPoint, hookSpeed * Time.deltaTime);

        if (Vector3.Distance(_hookPos, _anchorPoint) < 0.15f)
        {
            _hookPos = _anchorPoint;
            _ropeLen = Mathf.Min(Vector3.Distance(GetOrigin(), _anchorPoint), maxRange);
            _ropeLen = Mathf.Max(_ropeLen, minRopeLen);

            // รับ velocity ปัจจุบันของ player มาเป็นจุดเริ่มต้น swing
            _swingVelocity = _player.horizontalVelocity + _player.externalVelocity
                           + Vector3.up * Mathf.Min(_player.CurrentVelocity.y, 0f);

            // หยุด player controller velocity ชั่วคราว — swing จัดการเอง
            _player.SetExternalVelocity(Vector3.zero);
            _player.horizontalVelocity = Vector3.zero;

            _state = GrappleState.Hooked;
            SetCrosshair(true);
        }
    }

    // ── Hooked: โหนแบบ Pendulum ──────────────────────────
    //
    // หลักการ:
    //   1. Apply gravity ลง _swingVelocity
    //   2. เลื่อน player ตาม _swingVelocity
    //   3. Constraint: ถ้าระยะ > _ropeLen → ดัน player กลับบน "วงกลม"
    //      (ตัด component velocity ที่ชี้ออกจาก anchor ออก)
    //   4. กด W/S = reel in/out ปรับความยาวเชือก
    //   5. กด A/D = แรงด้านข้างเพิ่ม
    //   6. กด Space = กระโดดออกพร้อม momentum

    void TickHooked()
    {
        Vector3 origin = GetOrigin();
        Vector3 toAnchor = _anchorPoint - origin;
        float dist = toAnchor.magnitude;
        Vector3 ropeDir = toAnchor.normalized;     // ทิศจาก player → anchor

        // ── 1. Gravity ───────────────────────────────────
        _swingVelocity += Vector3.down * swingGravity * Time.deltaTime;

        // ── 2. Reel In / Out (W / S) ─────────────────────
        float vertInput = Input.GetAxisRaw("Vertical");
        if (vertInput > 0f)   // W = ดึงเชือกสั้น = แกว่งเร็วขึ้น
        {
            _ropeLen -= reelingSpeed * Time.deltaTime;
            _ropeLen = Mathf.Max(_ropeLen, minRopeLen);
        }
        else if (vertInput < 0f) // S = ปล่อยเชือกยาว
        {
            _ropeLen += unreelSpeed * Time.deltaTime;
            _ropeLen = Mathf.Min(_ropeLen, maxRange);
        }

        // ── 3. Side Swing (A / D) ─────────────────────────
        float sideInput = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(sideInput) > 0.1f)
        {
            // แรงด้านข้างตั้งฉากกับ ropeDir และ Up
            Vector3 sideAxis = Vector3.Cross(ropeDir, Vector3.up).normalized;
            _swingVelocity += sideAxis * sideInput * swingSideForce * Time.deltaTime;
        }

        // ── 4. Rope Constraint ────────────────────────────
        // เลื่อน player ก่อน แล้วค่อย constraint กลับ
        Vector3 newPos = origin + _swingVelocity * Time.deltaTime;

        Vector3 newToAnchor = _anchorPoint - newPos;
        float newDist = newToAnchor.magnitude;

        if (newDist > _ropeLen)
        {
            // ดัน player กลับมาบนวงกลม radius = _ropeLen
            newPos = _anchorPoint - newToAnchor.normalized * _ropeLen;

            // ตัด velocity component ที่ชี้ออกจาก anchor (radial component)
            // เหลือแต่ tangential velocity (ทิศแกว่ง) → นี่คือหัวใจของ pendulum
            Vector3 radial = Vector3.Project(_swingVelocity, newToAnchor.normalized);
            _swingVelocity -= radial;     // ตัด radial ออก เหลือแต่ swing
        }

        // ── 5. ส่ง velocity ไป PlayerController ──────────
        // bypass gravity ของ PlayerController ขณะโหน
        Vector3 delta = newPos - origin;
        _player.SetExternalVelocity(delta / Time.deltaTime);
        _player.horizontalVelocity = Vector3.zero;

        // override vertical ให้ PlayerController ไม่ดึง gravity ซ้ำ
        // (เราจัดการ gravity เองใน _swingVelocity แล้ว)
        // → เซ็ต verticalVelocity = 0 ป้องกัน double gravity
        _player.verticalVelocity = 0f;

        // ── 6. Jump Off ───────────────────────────────────
        if (Input.GetButtonDown("Jump"))
        {
            // กระโดดออกในทิศที่กำลังแกว่ง + บวก upward kick
            Vector3 jumpDir = (_swingVelocity.normalized + Vector3.up).normalized;
            _player.SetExternalVelocity(_swingVelocity + jumpDir * jumpOffForce);
            Release(boosted: false);
            return;
        }

        _hookPos = _anchorPoint;
    }

    // ── Releasing: เชือกหดกลับ ───────────────────────────

    void TickReleasing()
    {
        Vector3 origin = GetOrigin();
        _hookPos = Vector3.MoveTowards(_hookPos, origin, hookSpeed * 2f * Time.deltaTime);

        if (Vector3.Distance(_hookPos, origin) < 0.2f)
        {
            _state = GrappleState.Idle;
            SetLine(false);
            SetCrosshair(false);
        }
    }

    // ══════════════════════════════════════════════════════
    //  Release
    // ══════════════════════════════════════════════════════

    public void Release(bool boosted)
    {
        if (_state == GrappleState.Idle) return;

        if (boosted)
        {
            // ส่ง swing momentum ต่อ พร้อม boost เล็กน้อย
            _player.SetExternalVelocity(_swingVelocity * releaseBoost);
        }

        _swingVelocity = Vector3.zero;
        SetCrosshair(false);
        _state = GrappleState.Releasing;
    }

    // ══════════════════════════════════════════════════════
    //  Rope Visual (Catenary approximation)
    // ══════════════════════════════════════════════════════

    void DrawRope()
    {
        if (hookLine == null || !hookLine.enabled) return;

        Vector3 start = GetOrigin();
        Vector3 end = _hookPos;
        float len = Vector3.Distance(start, end);

        hookLine.positionCount = ropeSegments;
        _wobbleTimer += Time.deltaTime * 4f;

        for (int i = 0; i < ropeSegments; i++)
        {
            float t = i / (float)(ropeSegments - 1);
            Vector3 p = Vector3.Lerp(start, end, t);

            // ห้อยตาม gravity
            float sag = Mathf.Sin(t * Mathf.PI) * ropeSag * len * 0.1f;

            // สั่นเล็กน้อย
            float wobble = Mathf.Sin(_wobbleTimer + t * Mathf.PI * 2f)
                         * 0.008f * (1f - Mathf.Abs(t - 0.5f) * 2f);

            p += Vector3.down * sag + Vector3.right * wobble;
            hookLine.SetPosition(i, p);
        }
    }

    // ══════════════════════════════════════════════════════
    //  FOV
    // ══════════════════════════════════════════════════════

    void TickFOV()
    {
        if (playerCamera == null) return;
        float target = IsGrappling ? grappleFOV : _defaultFOV;
        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView, target, fovLerpSpeed * Time.deltaTime);
    }

    // ══════════════════════════════════════════════════════
    //  Helpers
    // ══════════════════════════════════════════════════════

    Vector3 GetOrigin()
        => hookOrigin != null ? hookOrigin.position : transform.position + Vector3.up * 0.7f;

    Vector3 GetMoveDir()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 d = transform.right * h + transform.forward * v;
        return d.magnitude > 1f ? d.normalized : d;
    }

    void SetLine(bool on)
    {
        if (hookLine != null) hookLine.enabled = on;
    }

    void SetCrosshair(bool hooked)
    {
        if (crosshairImage != null)
            crosshairImage.color = hooked ? colorHooked : colorDefault;
    }

    // ══════════════════════════════════════════════════════
    //  Gizmos
    // ══════════════════════════════════════════════════════

    void OnDrawGizmosSelected()
    {
        if (playerCamera == null) return;

        // แสดงทิศยิงและระยะ
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(playerCamera.transform.position,
                       playerCamera.transform.forward * maxRange);

        // แสดง anchor และเชือก
        if (_state == GrappleState.Hooked)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_anchorPoint, 0.2f);
            Gizmos.DrawLine(GetOrigin(), _anchorPoint);

            // แสดง rope length constraint
            Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
            Gizmos.DrawWireSphere(_anchorPoint, _ropeLen);
        }
    }
}