using System.Collections;
using UnityEngine;

public class DropAndFreezeOnRelease : MonoBehaviour
{
    [Header("Freeze Settings")]
    [SerializeField] private float settleDuration = 0.25f;
    [SerializeField] private float linearSpeedThreshold = 0.08f;
    [SerializeField] private float angularSpeedThreshold = 0.15f;
    [SerializeField] private float maximumDropTime = 5f;

    private Rigidbody flowerRigidbody;
    private Coroutine freezeCoroutine;

    // 是否接触到了下方的支撑表面
    private bool touchingSupportSurface;

    private void Awake()
    {
        flowerRigidbody = GetComponent<Rigidbody>();
    }

    // 绑定到 When Select
    public void PrepareForGrab()
    {
        if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
            freezeCoroutine = null;
        }

        touchingSupportSurface = false;
    }

    // 绑定到 When Unselect
    public void DropAndFreeze()
    {
        if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
        }

        freezeCoroutine = StartCoroutine(DropAndFreezeRoutine());
    }

    private IEnumerator DropAndFreezeRoutine()
    {
        // 等待 Grab Interaction 完成释放
        yield return null;

        touchingSupportSurface = false;

        flowerRigidbody.isKinematic = false;
        flowerRigidbody.useGravity = true;

        float dropTimer = 0f;
        float settledTimer = 0f;

        while (dropTimer < maximumDropTime)
        {
            dropTimer += Time.deltaTime;

            bool movingSlowly =
                flowerRigidbody.linearVelocity.sqrMagnitude
                <= linearSpeedThreshold * linearSpeedThreshold;

            bool rotatingSlowly =
                flowerRigidbody.angularVelocity.sqrMagnitude
                <= angularSpeedThreshold * angularSpeedThreshold;

            if (touchingSupportSurface && movingSlowly && rotatingSlowly)
            {
                settledTimer += Time.deltaTime;

                if (settledTimer >= settleDuration)
                {
                    FreezeFlower();
                    yield break;
                }
            }
            else
            {
                settledTimer = 0f;
            }

            yield return null;
        }

        // 超过等待时间后，如果已经接触支撑面就直接固定
        if (touchingSupportSurface)
        {
            FreezeFlower();
        }

        freezeCoroutine = null;
    }

    private void OnCollisionStay(Collision collision)
    {
        touchingSupportSurface = false;

        foreach (ContactPoint contact in collision.contacts)
        {
            // 法线向上，说明物体下方存在地面、桌面等支撑面
            if (contact.normal.y > 0.5f)
            {
                touchingSupportSurface = true;
                break;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        touchingSupportSurface = false;
    }

    private void FreezeFlower()
    {
        flowerRigidbody.linearVelocity = Vector3.zero;
        flowerRigidbody.angularVelocity = Vector3.zero;
        flowerRigidbody.useGravity = false;
        flowerRigidbody.isKinematic = true;

        freezeCoroutine = null;
    }
}