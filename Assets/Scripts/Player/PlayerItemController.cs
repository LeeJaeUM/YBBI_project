using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    [SerializeField]
    public BuffController _buffCtrl;

    public void SetBuff(BuffType buff)
    {
        _buffCtrl.SetBuff(buff);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
