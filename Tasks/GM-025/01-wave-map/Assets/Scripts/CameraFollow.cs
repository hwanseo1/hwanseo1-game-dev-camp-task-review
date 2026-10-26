using UnityEngine;

// 위에서 비스듬히 내려다보며 플레이어를 따라간다.
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 18f, -9f);

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}
