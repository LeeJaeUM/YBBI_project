using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerCountroller : MonoBehaviour
{
    [SerializeField] float _speed = 4.0f;

    Vector2 moveDir;


    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if(input != null )
        {
            moveDir = new Vector2 (input.x, input.y);
        }
    }



    // Update is called once per frame
    void Update()
    {
        bool hasContol = (moveDir != Vector2.zero);
        if(hasContol)
        {
            transform.Translate(moveDir * Time.deltaTime * _speed);
        }

    }




}
