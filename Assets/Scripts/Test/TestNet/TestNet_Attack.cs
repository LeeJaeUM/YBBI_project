using UnityEngine;
using UnityEngine.InputSystem;

public class TestNet_Attack : MonoBehaviour
{
    public float attackDamage = 5;
    public GameObject attackBullet;
    public TestNet_CharAir _myCharAir;

    private void OnAttack(InputValue value)
    {
        if(value.isPressed)
        {
            GameObject obj = Instantiate(attackBullet, transform.position, Quaternion.identity);
            _myCharAir.AddAir(attackDamage * -1);
        }
    }

    private void Awake()
    {
        _myCharAir = GetComponent<TestNet_CharAir>();
    }
}
