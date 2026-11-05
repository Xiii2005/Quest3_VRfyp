using System.Collections;
using UnityEngine;

public class ReturnToStartOnRelease : MonoBehaviour
{
    [SerializeField] private float returnDuration = 0.25f;

    private Rigidbody flowerRigidbody;
    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    private Coroutine returnCoroutine;

    private void Awake()
    {
        flowerRigidbody = GetComponent<Rigidbody>();

        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
    }

    public void ReturnToStart()
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }

        returnCoroutine = StartCoroutine(ReturnRoutine());
    }

    private IEnumerator ReturnRoutine()
    {
        // 等待Grab系统完成释放操作
        yield return null;

        if (flowerRigidbody != null)
        {
            flowerRigidbody.linearVelocity = Vector3.zero;
            flowerRigidbody.angularVelocity = Vector3.zero;
            flowerRigidbody.useGravity = false;
            flowerRigidbody.isKinematic = true;
        }

        transform.SetParent(originalParent, true);

        Vector3 returnStartPosition = transform.localPosition;
        Quaternion returnStartRotation = transform.localRotation;

        float elapsedTime = 0f;

        while (elapsedTime < returnDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / returnDuration);
            float smoothProgress =
                progress * progress * (3f - 2f * progress);

           // transform.localPosition = Vector3.Lerp(
            //    returnStartPosition,
            //    originalLocalPosition,
            //    smoothProgress
            //);

            /* transform.localRotation = Quaternion.Slerp(
                returnStartRotation,
                originalLocalRotation,
                smoothProgress
            ); */

            yield return null;
        }

        //transform.localPosition = originalLocalPosition;
        //transform.localRotation = originalLocalRotation;

        returnCoroutine = null;
    }
}