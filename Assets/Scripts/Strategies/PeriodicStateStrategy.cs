using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicStateStrategy : IItemStateStrategy
{
    private Coroutine _coroutine;
    private ItemContext _itemContext;
    private bool _isActive = true;

    public void TransmitData(ItemData itemData)
    {
        if (_isActive)
            Debug.Log($"[Периодический] {itemData}");
    }

    public void OnStateEnter(ItemContext itemContext)
    {
        _itemContext = itemContext;
        _coroutine = itemContext.StartCoroutine(SwitchStates(itemContext));
    }

    public void OnStateExit()
    {
        if (_coroutine != null)
            _itemContext.StopCoroutine(_coroutine);
    }

    private IEnumerator SwitchStates(ItemContext itemContext)
    {
        while (true)
        {
            _isActive = true;
            yield return new WaitForSeconds(Random.Range(5f, 10f));

            _isActive = false;
            yield return new WaitForSeconds(Random.Range(5f, 10f));
        }
    }
}
