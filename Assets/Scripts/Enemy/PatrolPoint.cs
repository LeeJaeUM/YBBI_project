using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    public Transform _startTr;
    public Transform _endTr;

    public Vector3 _startPoint = Vector3.zero;
    public Vector3 _endPoint = Vector3.zero;

    private void Awake()
    {
        _startPoint = _startTr.position;
        _endPoint = _endTr.position;
    }

    public Vector2 GetStartPonint()
    {
        return _startPoint;
    }

    public Vector2 GetEndPonint()
    {
        return _endPoint;
    }


}
