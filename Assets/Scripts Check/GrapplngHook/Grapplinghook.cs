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
    public float swingForce = 20f;
    public float airControl = 8f;
    public bool enableSwing = true;       // true = แกว่ง, false = พุ่งตรง

    [Header("Zip (Pull to Point)")]
    public float zipSpeed = 30f;          // ความเร็วพุ่ง
    public float zipStopDistance = 1.5f;  // หยุดเมื่อเข้าใกล้เป้าแค่นี้

    [Header("Cooldown")]
    public float grapplingCd = 0.3f;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.E;

    private Vector3 grapplePoint;
    private bool grappling;
    private bool isZipping;

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

        if (grappling && lr != null && gunTip != null)
        {
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grapplePoint);
        }

        if (grappling && enableSwing)
            ApplySwingAirControl();

        if (isZipping)
            UpdateZip();
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
            StartZip();

        if (lr != null)
        {
            lr.enabled = true;
            lr.positionCount = 2;
        }
    }

    // ── Zip Mode ─────────────────────────────────────────────────
    private void StartZip()
    {
        isZipping = true;
        Vector3 direction = (grapplePoint - transform.position).normalized;
        pm.horizontalVelocity = new Vector3(direction.x, 0f, direction.z) * zipSpeed;
        pm.verticalVelocity = direction.y * zipSpeed;
        pm.externalVelocity = Vector3.zero;
    }

    private void UpdateZip()
    {
        if (Vector3.Distance(transform.position, grapplePoint) <= zipStopDistance)
            StopGrapple();
    }

    // ── Swing Mode ───────────────────────────────────────────────
    void StartSwing()
    {
        Rigidbody rb = pm.GetComponent<Rigidbody>();
        if (rb == null)
            rb = pm.gameObject.AddComponent<Rigidbody>();

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.velocity = pm.CurrentVelocity;

        pm.GetComponent<CharacterController>().enabled = false;

        _joint = pm.gameObject.AddComponent<SpringJoint>();
        _joint.autoConfigureConnectedAnchor = false;
        _joint.connectedAnchor = grapplePoint;

        float dist = Vector3.Distance(pm.transform.position, grapplePoint);
        _joint.maxDistance = dist * 0.8f;
        _joint.minDistance = dist * 0.1f;
        _joint.spring = 4.5f;
        _joint.damper = 7f;
        _joint.massScale = 4.5f;
    }

    void ApplySwingAirControl()
    {
        Rigidbody rb = pm.GetComponent<Rigidbody>();
        if (rb == null) return;

        if (Input.GetKey(KeyCode.W)) rb.AddForce(cam.forward * airControl, ForceMode.Force);
        if (Input.GetKey(KeyCode.S)) rb.AddForce(-cam.forward * airControl, ForceMode.Force);
        if (Input.GetKey(KeyCode.A)) rb.AddForce(-cam.right * airControl, ForceMode.Force);
        if (Input.GetKey(KeyCode.D)) rb.AddForce(cam.right * airControl, ForceMode.Force);
    }

    // ── Stop ─────────────────────────────────────────────────────
    public void StopGrapple()
    {
        if (!grappling) return;

        grappling = false;
        isZipping = false;
        grapplingCdTimer = grapplingCd;

        if (_joint != null) { Destroy(_joint); _joint = null; }

        Rigidbody rb = pm.GetComponent<Rigidbody>();
        if (rb != null)
        {
            pm.horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            pm.verticalVelocity = rb.velocity.y;
            Destroy(rb);
        }

        pm.GetComponent<CharacterController>().enabled = true;

        if (lr != null) { lr.enabled = false; lr.positionCount = 0; }
    }

    public bool IsGrappling() => grappling;
    public Vector3 GetGrapplePoint() => grapplePoint;
}