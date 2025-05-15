using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveStateStrategy : IItemStateStrategy
{
    public void OnStateEnter(ItemContext itemContext)
    {
    }

    public void OnStateExit()
    {
    }

    public void TransmitData(ItemData itemData)
    {
        Debug.Log("[Пассивный] Нет данных для передачи");
    }
}
