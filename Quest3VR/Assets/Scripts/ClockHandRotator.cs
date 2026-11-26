using UnityEngine;

public class ClockHandRotator : MonoBehaviour
{
    public enum HandType
    {
        Minute,
        Hour
    }

    [Header("ClockHand Type")]
    public HandType handType = HandType.Minute;

    [Header("Global Time Speed")]
    public float baseSpeed = 180f;

    public Vector3 rotationAxis = new Vector3(0, 0, 1);

    public bool isClockwise = true;

    void Update()
    {
        float direction = isClockwise ? -1f : 1f;
        float currentSpeed = 0f;

        // 根据枚举类型分配实际转速，时针永远是分针速度的 1/12
        switch (handType)
        {
            case HandType.Minute:
                currentSpeed = baseSpeed;
                break;
            case HandType.Hour:
                currentSpeed = baseSpeed / 12f;
                break;
        }

        // 计算当前帧的旋转量
        float rotationThisFrame = currentSpeed * Time.deltaTime * direction;

        // 沿自身的局部坐标轴进行旋转
        transform.Rotate(rotationAxis, rotationThisFrame, Space.Self);
    }
}