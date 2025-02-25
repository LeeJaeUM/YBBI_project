using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [SerializeField]
    Transform _player;
    [SerializeField]
    float _speed = 0.15f;

    Vector3 _offset = new Vector3(0, 0, -10);
    Vector3 _desiredPos;
    Vector3 _smoothPos;

    void FixedUpdate()
    {
        _desiredPos = _player.position + _offset;
        _smoothPos = Vector3.Lerp(transform.position, _desiredPos, _speed);

        transform.position = _smoothPos;
    }
}
