using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerController pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance = 50f;
    public float grappleDelayTime = 0f;
    public float overshootYAxis = 3f;

    [Header("Swing")]
    public float swingForce = 20f;        // แรงแกว่ง
    public float airControl = 8f;         // ควบคุมทิศขณะแกว่ง
    public bool enableSwing = true;       // true = เหวี่ยง, false = พุ่งตรง

    [Header("Cooldown")]
    public float grapplingCd = 0.3f;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.E;

    private Vector3 grapplePoint;
    private bool grappling;

    // Swing state
    private SpringJoint _joint;

    private void Start()
    {
        pm = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey))
            StartGrapple();

        if (Input.GetKeyUp(grappleKey))
            StopGrapple();

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;

        // วาด rope
        if (grappling && lr != null && gunTip != null)
        {
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grapplePoint);
        }

        // Air control ขณะแกว่ง
        if (grappling && enableSwing)
            ApplySwingAirControl();
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        RaycastHit hit;
        if (!Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
            return;

        grappling = true;
        grapplePoint = hit.point;

        if (enableSwing)
            StartSwing();
        else
        {
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }

        if (lr != null)
        {
            lr.enabled = true;
            lr.positionCount = 2;
        }
    }

    // ── Swing Mode ───────────────────────────────────────────────
    void StartSwing()
    {
        // ใช้ SpringJoint เพื่อจำลองเชือก physics-based
        Rigidbody rb = pm.GetComponent<Rigidbody>();
        if (rb == null)
            rb = pm.gameObject.AddComponent<Rigidbody>();

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // ถ่าย velocity จาก CharacterController ไป Rigidbody
        rb.velocity = pm.CurrentVelocity;

        // ปิด CharacterController ชั่วคราว (ขัดแย้งกับ Rigidbody)
        pm.GetComponent<CharacterController>().enabled = false;

        _joint = pm.gameObject.AddComponent<SpringJoint>();
        _joint.autoConfigureConnectedAnchor = false;
        _joint.connectedAnchor = grapplePoint;

        float dist = Vector3.Distance(pm.transform.position, grapplePoint);

        // ความยาวเชือก = ระยะปัจจุบัน (แกว่งได้ไม่เกินระยะนี้)
        _joint.maxDistance = dist * 0.8f;
        _joint.minDistance = dist * 0.1f;

        // ค่า spring ทำให้เชือกรู้สึก taut (ตึง)
        _joint.spring = 4.5f;
        _joint.damper = 7f;
        _joint.massScale = 4.5f;
    }

    void ApplySwingAirControl()
    {
        Rigidbody rb = pm.GetComponent<Rigidbody>();
        if (rb == null) return;

        // ควบคุมทิศขณะแกว่งด้วย WASD
        if (Input.GetKey(KeyCode.W))
            rb.AddForce(cam.forward * airControl, ForceMode.Force);
        if (Input.GetKey(KeyCode.S))
            rb.AddForce(-cam.forward * airControl, ForceMode.Force);
        if (Input.GetKey(KeyCode.A))
            rb.AddForce(-cam.right * airControl, ForceMode.Force);
        if (Input.GetKey(KeyCode.D))
            rb.AddForce(cam.right * airControl, ForceMode.Force);
    }

    // ── Launch Mode (เดิม) ───────────────────────────────────────
    private void ExecuteGrapple()
    {
        Vector3 lowestPoint = new Vector3(
            transform.position.x,
            transform.position.y - 1f,
            transform.position.z
        );

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;
        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        JumpToPosition(grapplePoint, highestPointOnArc);
        Invoke(nameof(StopGrapple), 1f);
    }

    private void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        Vector3 velocity = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        pm.horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        pm.verticalVelocity = velocity.y;
        pm.externalVelocity = Vector3.zero;
    }

    private Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Mathf.Abs(pm.gravity != 0 ? pm.gravity : -25f);
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(
            endPoint.x - startPoint.x, 0f,
            endPoint.z - startPoint.z
        );

        float timeUp = Mathf.Sqrt(2 * trajectoryHeight / gravity);
        float timeDown = Mathf.Sqrt(2 * Mathf.Abs(trajectoryHeight - displacementY) / gravity);
        float totalTime = timeUp + timeDown;

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / totalTime;

        return velocityXZ + velocityY;
    }

    // ── Stop ─────────────────────────────────────────────────────
    public void StopGrapple()
    {
        if (!grappling) return;

        grappling = false;
        grapplingCdTimer = grapplingCd;

        // ลบ SpringJoint
        if (_joint != null)
        {
            Destroy(_joint);
            _joint = null;
        }

        // คืน velocity จาก Rigidbody กลับไป PlayerController
        Rigidbody rb = pm.GetComponent<Rigidbody>();
        if (rb != null)
        {
            pm.horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            pm.verticalVelocity = rb.velocity.y;
            Destroy(rb);
        }

        // เปิด CharacterController กลับ
        pm.GetComponent<CharacterController>().enabled = true;

        if (lr != null)
        {
            lr.enabled = false;
            lr.positionCount = 0;
        }
    }

    public bool IsGrappling() => grappling;
    public Vector3 GetGrapplePoint() => grapplePoint;
}