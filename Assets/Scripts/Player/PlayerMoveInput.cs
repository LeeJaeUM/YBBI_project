using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveInput : MonoBehaviour
{
    [SerializeField]
    public float _speed = 5f;

    Vector2 _inputVec;
    Vector2 _nextVec;

    Rigidbody2D _rigid;

    void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        _nextVec = _inputVec * _speed * Time.fixedDeltaTime;
        _rigid.MovePosition(_rigid.position + _nextVec);
    }

    void OnMove(InputValue value)
    {
        _inputVec = value.Get<Vector2>();
    }
}
