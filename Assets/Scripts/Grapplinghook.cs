using UnityEngine;

/// <summary>
/// Physics-based Grappling Hook
/// ดึงผู้เล่นเข้าหาศัตรูทีละน้อย พร้อมเพิ่มความเร็วสะสม
/// ต้องการ: PlayerController บน Player, LineRenderer สำหรับวาดเส้น
/// วิธีใช้: ใส่ script นี้ไว้บน Player GameObject เดียวกับ PlayerController
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class GrapplingHook : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float grappleRange = 30f;             // ระยะยิง hook
    public float pullForce = 25f;                // แรงดึงต่อวินาที
    public float maxPullSpeed = 20f;             // ความเร็วดึงสูงสุด
    public float speedBoostMultiplier = 1.5f;    // ความเร็วที่บวกเพิ่มเมื่อ grapple ใกล้เป้า
    public float arrivalDistance = 2.5f;         // ระยะที่ถือว่า "ถึงแล้ว"
    public float hookTravelSpeed = 40f;          // ความเร็วที่ hook projectile บินไป (visual)

    [Header("Spring Settings (ทำให้การดึงดูนุ่มนวล)")]
    public float springStrength = 20f;           // ความแข็งของ spring
    public float springDamper = 5f;              // ลด oscillation ของ spring

    [Header("Line Renderer")]
    public LineRenderer hookLine;                // ลาก LineRenderer มาใส่
    public Transform hookOrigin;                 // จุดเริ่มต้นของเส้น (เช่น มือ / ปลายกล้อง)

    [Header("Enemy Detection")]
    public LayerMask enemyLayer;                 // Layer ของ Enemy
    public LayerMask obstacleLayer;              // Layer ของ obstacle ที่บัง

    [Header("Camera FOV Effect")]
    public Camera playerCamera;
    public float grappleFOV = 90f;              // FOV ขณะ grapple
    public float fovLerpSpeed = 8f;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;  // Right Click

    // ── References ──────────────────────────────────────────────
    private PlayerController _player;

    // ── State ───────────────────────────────────────────────────
    private enum GrappleState { Idle, Traveling, Pulling, Releasing }
    private GrappleState _state = GrappleState.Idle;

    private Transform _targetEnemy;             // Enemy ที่ hook ไป
    private Vector3 _hookPosition;              // ตำแหน่ง hook ปัจจุบัน (animation)
    private Vector3 _hookTargetPosition;        // ตำแหน่งที่ hook จะไป
    private float _defaultFOV;
    private Vector3 _accumulatedVelocity;       // velocity สะสมขณะดึง

    // ── Properties ──────────────────────────────────────────────
    public bool IsGrappling => _state == GrappleState.Pulling;

    // ────────────────────────────────────────────────────────────
    void Awake()
    {
        _player = GetComponent<PlayerController>();

        if (playerCamera != null)
            _defaultFOV = playerCamera.fieldOfView;

        if (hookLine != null)
            hookLine.enabled = false;
    }

    void Update()
    {
        HandleInput();
        UpdateGrappleState();
        UpdateLineRenderer();
        UpdateFOV();
    }

    // ────────────────────────────────────────────────────────────
    #region Input
    void HandleInput()
    {
        if (Input.GetKeyDown(grappleKey))
        {
            if (_state == GrappleState.Idle)
                TryStartGrapple();
        }

        if (Input.GetKeyUp(grappleKey))
        {
            if (_state != GrappleState.Idle)
                ReleaseGrapple();
        }
    }
    #endregion

    // ────────────────────────────────────────────────────────────
    #region Grapple Logic
    void TryStartGrapple()
    {
        // หา enemy ที่ camera จ้องอยู่
        Transform enemy = FindTargetEnemy();
        if (enemy == null) return;

        // ตรวจว่ามีอะไรบังไหม
        Vector3 dirToEnemy = (enemy.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, dirToEnemy,
            Vector3.Distance(transform.position, enemy.position),
            obstacleLayer))
        {
            Debug.Log("Grapple blocked by obstacle");
            return;
        }

        _targetEnemy = enemy;
        _hookTargetPosition = enemy.position;
        _hookPosition = hookOrigin != null ? hookOrigin.position : transform.position;
        _accumulatedVelocity = Vector3.zero;

        _state = GrappleState.Traveling;

        if (hookLine != null)
            hookLine.enabled = true;
    }

    Transform FindTargetEnemy()
    {
        if (playerCamera == null) return null;

        // Raycast จากกล้องตรงๆ
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, grappleRange, enemyLayer))
            return hit.transform;

        // ถ้าไม่โดน: หา enemy รอบๆ ในระยะและเลือกที่ใกล้ center screen ที่สุด
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, grappleRange, enemyLayer);
        Transform bestTarget = null;
        float bestAngle = 30f; // รับเฉพาะที่อยู่ใน cone 30 องศาจาก crosshair

        foreach (var col in nearbyEnemies)
        {
            Vector3 dirToEnemy = (col.transform.position - playerCamera.transform.position).normalized;
            float angle = Vector3.Angle(playerCamera.transform.forward, dirToEnemy);

            if (angle < bestAngle)
            {
                bestAngle = angle;
                bestTarget = col.transform;
            }
        }

        return bestTarget;
    }

    void UpdateGrappleState()
    {
        switch (_state)
        {
            case GrappleState.Traveling:
                UpdateTraveling();
                break;
            case GrappleState.Pulling:
                UpdatePulling();
                break;
            case GrappleState.Releasing:
                UpdateReleasing();
                break;
        }
    }

    void UpdateTraveling()
    {
        // Hook บินไปหาเป้า
        if (_targetEnemy != null)
            _hookTargetPosition = _targetEnemy.position;

        _hookPosition = Vector3.MoveTowards(
            _hookPosition,
            _hookTargetPosition,
            hookTravelSpeed * Time.deltaTime
        );

        // เมื่อ hook ถึงเป้า เริ่ม pull
        if (Vector3.Distance(_hookPosition, _hookTargetPosition) < 0.5f)
        {
            _state = GrappleState.Pulling;
        }
    }

    void UpdatePulling()
    {
        if (_targetEnemy == null)
        {
            ReleaseGrapple();
            return;
        }

        // อัปเดตตำแหน่ง hook ตาม enemy ที่ขยับ
        _hookPosition = _targetEnemy.position;

        Vector3 toTarget = _targetEnemy.position - transform.position;
        float distance = toTarget.magnitude;

        // ถึงแล้ว: ปล่อย grapple
        if (distance <= arrivalDistance)
        {
            OnArrived();
            return;
        }

        // ── Spring Physics Pull ──────────────────────────────────
        // F = (distance * springStrength) - (velocity * springDamper)
        // คำนวณ velocity ที่ player กำลังเคลื่อนที่เทียบกับ target
        Vector3 currentVelocity = _player.horizontalVelocity + _player.externalVelocity;
        float velocityAlongPull = Vector3.Dot(currentVelocity, toTarget.normalized);

        float springForce = (distance * springStrength) - (velocityAlongPull * springDamper);
        springForce = Mathf.Clamp(springForce, 0f, pullForce * 2f); // ไม่ push กลับ

        // สะสม velocity ทิศทางหา target
        _accumulatedVelocity += toTarget.normalized * springForce * Time.deltaTime;

        // จำกัดความเร็วสูงสุด
        if (_accumulatedVelocity.magnitude > maxPullSpeed)
            _accumulatedVelocity = _accumulatedVelocity.normalized * maxPullSpeed;

        // ส่ง velocity ไป PlayerController
        _player.SetExternalVelocity(_accumulatedVelocity);

        // ยกเลิก gravity บางส่วนขณะดึง (รู้สึกเหมือนถูกดึงลอยไปจริงๆ)
        // PlayerController จัดการ gravity เอง แต่เราเพิ่ม upward component เล็กน้อย
        if (_player.horizontalVelocity.magnitude < 2f)
        {
            // ถ้า player ไม่ได้วิ่ง ให้ grapple ช่วย pull แนวตั้งด้วย
            float verticalHelp = Mathf.Clamp(toTarget.y * 0.5f, -5f, 10f);
            // Note: ส่วนนี้ทำงานผ่าน externalVelocity ที่มี Y component
        }
    }

    void OnArrived()
    {
        // เมื่อถึงเป้า: เพิ่ม speed boost ทิศที่วิ่งมา (momentum transfer)
        Vector3 arrivalDirection = (_targetEnemy.position - transform.position).normalized;
        Vector3 boostVelocity = arrivalDirection * maxPullSpeed * speedBoostMultiplier;

        // Override velocity พร้อม boost
        _player.SetExternalVelocity(boostVelocity);

        // เพิ่ม upward kick เล็กน้อยเพื่อให้กระเด้งขึ้น (รู้สึกดี)
        _player.horizontalVelocity = new Vector3(boostVelocity.x, 0, boostVelocity.z);

        ReleaseGrapple(keepMomentum: true);
    }

    void UpdateReleasing()
    {
        // Hook line หดกลับมาหา player (visual feedback)
        _hookPosition = Vector3.MoveTowards(
            _hookPosition,
            hookOrigin != null ? hookOrigin.position : transform.position,
            hookTravelSpeed * 2f * Time.deltaTime
        );

        Vector3 origin = hookOrigin != null ? hookOrigin.position : transform.position;
        if (Vector3.Distance(_hookPosition, origin) < 0.3f)
        {
            _state = GrappleState.Idle;
            if (hookLine != null)
                hookLine.enabled = false;
        }
    }

    public void ReleaseGrapple(bool keepMomentum = false)
    {
        if (!keepMomentum)
        {
            // ลด external velocity ลงครึ่งนึงเมื่อปล่อย (ไม่หยุดทันที)
            _player.SetExternalVelocity(_player.externalVelocity * 0.6f);
        }

        _state = GrappleState.Releasing;
        _targetEnemy = null;
    }
    #endregion

    // ────────────────────────────────────────────────────────────
    #region Visual Feedback
    void UpdateLineRenderer()
    {
        if (hookLine == null || !hookLine.enabled) return;

        Vector3 origin = hookOrigin != null ? hookOrigin.position : transform.position;

        hookLine.SetPosition(0, origin);
        hookLine.SetPosition(1, _hookPosition);
    }

    void UpdateFOV()
    {
        if (playerCamera == null) return;

        float targetFOV = IsGrappling ? grappleFOV : _defaultFOV;
        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFOV,
            fovLerpSpeed * Time.deltaTime
        );
    }
    #endregion

    // ────────────────────────────────────────────────────────────
    #region Gizmos (Editor debug)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, grappleRange);
    }
    #endregion
}