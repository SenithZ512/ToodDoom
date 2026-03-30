using UnityEngine;

/// <summary>
/// PlayerModelSetup — สร้าง Player Model จาก Primitive อัตโนมัติ
/// แค่ Add Component นี้บน Player GameObject แล้วกด Play ได้เลย
/// ไม่ต้องสร้าง mesh เอง เหมาะสำหรับ Tech-Demo
///
/// Hierarchy ที่จะสร้าง:
/// Player
/// └── Body (Capsule — ซ่อนจาก FP camera)
/// └── CameraHolder
///     └── Main Camera
///         └── FP_Arms
///             ├── LeftArm
///             │   └── LeftHand
///             └── RightArm
///                 └── RightHand
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class PlayerModelSetup : MonoBehaviour
{
    [Header("Body")]
    public Material bodyMaterial;               // ถ้าไม่ใส่จะใช้ Default Material

    [Header("Arms / Hands")]
    public Material armMaterial;
    public Material handMaterial;

    [Header("Layer")]
    [Tooltip("Layer สำหรับ FP Arms — ต้องไม่ถูก cull โดย Main Camera\nสร้าง Layer ชื่อ 'FPArms' แล้วใส่เลข Layer ที่นี่")]
    public int fpArmsLayer = 6;                 // Default layer 6 (แก้ตาม project)

    [Header("Arm Positions (ปรับให้พอดีกล้อง)")]
    public Vector3 rightArmOffset = new Vector3(0.2f, -0.25f, 0.35f);
    public Vector3 leftArmOffset = new Vector3(-0.2f, -0.28f, 0.3f);

    // ── References ──────────────────────────────────────────────
    [HideInInspector] public Transform fpArmsRoot;
    [HideInInspector] public Transform rightHand;
    [HideInInspector] public Transform leftHand;

    private PlayerController _player;
    private Camera _mainCamera;

    // ────────────────────────────────────────────────────────────
    void Awake()
    {
        _player = GetComponent<PlayerController>();
        BuildModel();
        SetupCamera();
    }

    // ────────────────────────────────────────────────────────────
    void BuildModel()
    {
        BuildBody();
        BuildFPArms();
    }

    // ── Body (Capsule) ───────────────────────────────────────────
    void BuildBody()
    {
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Body";
        body.transform.SetParent(transform, false);
        body.transform.localPosition = new Vector3(0, 0, 0);
        body.transform.localScale = new Vector3(0.8f, 0.9f, 0.8f);

        // ใช้ material ที่กำหนด หรือ default
        if (bodyMaterial != null)
            body.GetComponent<Renderer>().material = bodyMaterial;

        // ลบ Collider ออก — PlayerController ใช้ CharacterController อยู่แล้ว
        Destroy(body.GetComponent<Collider>());

        // Body ไม่ควรถูก render ใน FP Camera (เห็นแค่แขน)
        // แต่ให้ถูก render กับกล้องอื่น (เช่น cutscene cam)
        // → จะจัดการใน SetupCamera()
    }

    // ── FP Arms ─────────────────────────────────────────────────
    void BuildFPArms()
    {
        // หา CameraHolder และ Camera
        Transform camHolder = transform.Find("CameraHolder");
        if (camHolder == null)
        {
            // สร้าง CameraHolder ถ้ายังไม่มี
            GameObject ch = new GameObject("CameraHolder");
            ch.transform.SetParent(transform, false);
            ch.transform.localPosition = new Vector3(0, 0.7f, 0);
            camHolder = ch.transform;
        }

        _mainCamera = camHolder.GetComponentInChildren<Camera>();
        if (_mainCamera == null)
        {
            // สร้าง Camera ถ้ายังไม่มี
            GameObject camGO = new GameObject("Main Camera");
            camGO.transform.SetParent(camHolder, false);
            camGO.transform.localPosition = Vector3.zero;
            _mainCamera = camGO.AddComponent<Camera>();
            camGO.AddComponent<AudioListener>();
            camGO.tag = "MainCamera";
        }

        // Root ของ FP Arms อยู่ใต้ Camera
        GameObject armsRoot = new GameObject("FP_Arms");
        armsRoot.transform.SetParent(_mainCamera.transform, false);
        armsRoot.transform.localPosition = Vector3.zero;
        fpArmsRoot = armsRoot.transform;

        // สร้างแขนและมือ
        rightHand = BuildArm("Right", rightArmOffset, true);
        leftHand = BuildArm("Left", leftArmOffset, false);

        // ส่ง reference ไป PlayerController (CameraHolder)
        _player.cameraHolder = camHolder;
    }

    /// <summary>สร้างแขน 1 ข้าง ประกอบจาก Upper Arm + Forearm + Hand</summary>
    Transform BuildArm(string side, Vector3 rootOffset, bool isRight)
    {
        float mirror = isRight ? 1f : -1f;

        // ── Upper Arm ────────────────────────────────────────────
        GameObject upperArmGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        upperArmGO.name = $"{side}UpperArm";
        upperArmGO.transform.SetParent(fpArmsRoot, false);
        upperArmGO.transform.localPosition = rootOffset;
        upperArmGO.transform.localRotation = Quaternion.Euler(80f, 0f, mirror * -15f);
        upperArmGO.transform.localScale = new Vector3(0.06f, 0.12f, 0.06f);
        ApplyMaterial(upperArmGO, armMaterial);
        Destroy(upperArmGO.GetComponent<Collider>());
        SetLayerRecursive(upperArmGO, fpArmsLayer);

        // ── Forearm ──────────────────────────────────────────────
        GameObject forearmGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        forearmGO.name = $"{side}Forearm";
        forearmGO.transform.SetParent(upperArmGO.transform, false);
        forearmGO.transform.localPosition = new Vector3(0f, 1.1f, 0.1f);
        forearmGO.transform.localRotation = Quaternion.Euler(-10f, 0f, 0f);
        forearmGO.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f); // relative to parent
        ApplyMaterial(forearmGO, armMaterial);
        Destroy(forearmGO.GetComponent<Collider>());
        SetLayerRecursive(forearmGO, fpArmsLayer);

        // ── Hand ─────────────────────────────────────────────────
        GameObject handGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        handGO.name = $"{side}Hand";
        handGO.transform.SetParent(forearmGO.transform, false);
        handGO.transform.localPosition = new Vector3(0f, 1.05f, 0.05f);
        handGO.transform.localRotation = Quaternion.Euler(10f, 0f, mirror * 5f);
        handGO.transform.localScale = new Vector3(1.1f, 0.55f, 0.7f); // relative to forearm
        ApplyMaterial(handGO, handMaterial);
        Destroy(handGO.GetComponent<Collider>());
        SetLayerRecursive(handGO, fpArmsLayer);

        return handGO.transform;
    }

    // ── Camera Setup ─────────────────────────────────────────────
    void SetupCamera()
    {
        if (_mainCamera == null) return;

        // Camera หลัก: render ทุก Layer ยกเว้น FP Arms
        // (เพื่อไม่ให้แขนโดน clip กับ geometry ผ่าน near clip plane)
        _mainCamera.cullingMask = ~0;                               // All layers
        _mainCamera.cullingMask &= ~(1 << fpArmsLayer);            // ยกเว้น FPArms layer
        _mainCamera.nearClipPlane = 0.1f;
        _mainCamera.fieldOfView = 75f;

        // สร้าง Secondary Camera สำหรับ render แค่ FP Arms
        // render บน top ของ main camera (depth สูงกว่า)
        // วิธีนี้ป้องกัน arm clip ผ่านผนัง
        GameObject armsCamGO = new GameObject("FP_ArmsCamera");
        armsCamGO.transform.SetParent(_mainCamera.transform, false);
        armsCamGO.transform.localPosition = Vector3.zero;
        armsCamGO.transform.localRotation = Quaternion.identity;

        Camera armsCamera = armsCamGO.AddComponent<Camera>();
        armsCamera.cullingMask = 1 << fpArmsLayer;   // render เฉพาะ FPArms layer
        armsCamera.clearFlags = CameraClearFlags.Depth; // clear แค่ depth (overlay)
        armsCamera.depth = _mainCamera.depth + 1;       // render หลัง main cam
        armsCamera.nearClipPlane = 0.01f;               // near clip น้อย = ไม่ clip แขน
        armsCamera.fieldOfView = _mainCamera.fieldOfView;
    }

    // ── Helpers ──────────────────────────────────────────────────
    void ApplyMaterial(GameObject go, Material mat)
    {
        if (mat == null) return;
        var rend = go.GetComponent<Renderer>();
        if (rend != null) rend.material = mat;
    }

    void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursive(child.gameObject, layer);
    }
}