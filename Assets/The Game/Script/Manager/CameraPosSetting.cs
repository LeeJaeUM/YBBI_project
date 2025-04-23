using UnityEngine;
using Unity.Netcode;
using static UnityEngine.GraphicsBuffer;

public class CameraPosSetting : NetworkBehaviour
{
    [SerializeField] float _speed = 0.15f;

    Vector3 _offset = new Vector3(0, 0, -10);
    Transform _target;
    Vector3 _desiredPos;
    Vector3 _smoothPos;

    public void SetTarget(Transform newTarget)
    {
        _target = newTarget;
    }

    void FixedUpdate()
    {
        if (_target == null) return;

        _desiredPos = _target.position + _offset;
        _smoothPos = Vector3.Lerp(Camera.main.transform.position, _desiredPos, _speed);
        Camera.main.transform.position = _smoothPos;
    }
}