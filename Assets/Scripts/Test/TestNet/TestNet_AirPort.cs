using Unity.VisualScripting;
using UnityEngine;

public class TestNet_AirPort : MonoBehaviour
{
    public float _spreadAir = 50;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("닿음");
        TestNet_CharAir charAir = collision.GetComponent<TestNet_CharAir>();
        if (charAir != null)
        {
            Debug.Log("공기 줌");
            charAir.AddAir(_spreadAir);
        }
    }
}
