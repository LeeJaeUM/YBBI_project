using UnityEngine;
using Unity.Netcode;
public class SetCameraTarget : NetworkBehaviour
{
    public void SetTarget()
    {
        if (IsOwner) // 본인 플레이어인지 체크
        {
            Camera.main.GetComponent<CameraPosSetting>().SetTarget(transform);
        }
    }
}
