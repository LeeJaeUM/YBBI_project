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
}
