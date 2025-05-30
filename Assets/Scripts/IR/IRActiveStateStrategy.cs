using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IRActiveStateStrategy : IItemStateStrategy
{
    private IRDeviceContext _context;

    public void OnStateEnter(ItemContext context)
    {
        _context = context as IRDeviceContext;
        if (_context != null)
        {
            Debug.Log($"[IRDevice] Активировано (ID: {_context.ID})");
        }
    }

    public void OnStateExit()
    {
        if (_context != null)
        {
            Debug.Log($"[IRDevice] Деактивировано (ID: {_context.ID})");
        }
    }

    public void TransmitData(ItemData data)
    {
        // Для IRDevice всегда активны
        data.IsIRDevice = true;
        data.IsActive = true;
        Debug.Log($"[IRDevice] {data}");
    }

    public void TransmitIRData(IRDeviceData data)
    {
        Debug.Log($"[IRDevice] {data}");
    }
}
