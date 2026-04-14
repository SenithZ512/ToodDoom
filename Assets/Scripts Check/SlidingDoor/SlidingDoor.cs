using UnityEngine;
using DG.Tweening;

public class SlidingDoor : MonoBehaviour
{
    [Header("Settings")]
    public Transform doorMesh;
    public float slideDistance = 2.5f;
    public float duration = 0.5f;

    [Header("Slide Direction")]
    public Vector3 slideDirection = Vector3.right;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen = false;
    private bool isMoving = false;

    void Start()
    {
        closedPos = doorMesh.localPosition;
        openPos = closedPos + slideDirection.normalized * slideDistance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            OpenDoor();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isOpen)
        {
            CloseDoor();
        }
    }

    void OpenDoor()
    {
        if (isMoving) return;
        isMoving = true;

        doorMesh.DOLocalMove(openPos, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => {
                isOpen = true;
                isMoving = false;
            });
    }

    void CloseDoor()
    {
        if (isMoving) return;
        isMoving = true;

        doorMesh.DOLocalMove(closedPos, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => {
                isOpen = false;
                isMoving = false;
            });
    }
}