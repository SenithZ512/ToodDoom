#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class FixMaterials : MonoBehaviour
{
    [MenuItem("Tools/Fix Pink Materials")]
    static void Fix()
    {
        Material urpMat = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/VFX Check/Beam.mat"); // ← เปลี่ยนเป็น path ของ material ที่สร้างไว้

        MeshRenderer[] renderers = FindObjectsOfType<MeshRenderer>();
        foreach (var r in renderers)
        {
            r.sharedMaterial = urpMat;
        }

        Debug.Log("Fixed " + renderers.Length + " objects!");
    }
}
#endif