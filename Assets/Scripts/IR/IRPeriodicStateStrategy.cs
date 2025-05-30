using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IRPeriodicStateStrategy : IItemStateStrategy
{
    private IRDeviceContext _context;
    private Coroutine _coroutine;
    private bool _isActive = true;

    public bool IsActive { get; private set; } = true;

    public void OnStateEnter(ItemContext context)
    {
        _context = context as IRDeviceContext;
        if (_context != null)
        {
            _coroutine = _context.StartCoroutine(ToggleRoutine());
        }
    }

    public void OnStateExit()
    {
        if (_coroutine != null && _context != null)
        {
            _context.StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public void TransmitData(ItemData data)
    {
        if (_isActive)
        {
            data.IsIRDevice = true;
            data.IsActive = true;
            Debug.Log($"[IRDevice] {data}");
        }
        else
        {
            data.IsIRDevice = true;
            data.IsActive = false;
        }
    }

    public void TransmitIRData(IRDeviceData data)
    {
        if (_isActive)
        {
            Debug.Log($"[IRDevice] {data}");
        }
    }

    private IEnumerator ToggleRoutine()
    {
        while (true)
        {
            IsActive = !IsActive;
            _context.GetComponent<Renderer>().material.color = IsActive ? Color.magenta : Color.gray;
            yield return new WaitForSeconds(Random.Range(5f, 10f));
        }
    }
}
