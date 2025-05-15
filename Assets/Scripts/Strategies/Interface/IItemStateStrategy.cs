using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemStateStrategy
{
    void TransmitData(ItemData itemData);
    void OnStateEnter(ItemContext itemContext);
    void OnStateExit();
}
