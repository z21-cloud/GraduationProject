using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveStateStrategy : IItemStateStrategy
{
    public void OnStateEnter(ItemContext itemContext)
    {
        
    }

    public void OnStateExit()
    {
        
    }

    public void TransmitData(ItemData itemData)
    {
        //Debug.Log($"[Активный] {itemData}");
    }

    public void TransmitIRData(IRDeviceData data)
    {
        //throw new System.NotImplementedException();
    }
}
