using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class IRDeviceContext : ItemContext
{
    private CapsuleCollider irCollider;

    private void Awake()
    {
        Type = ItemType.IRDevice;
        Frequency = 0f; // ИК-устройства не имеют частоты
        StateType = ItemStateType.Active;

        if (TryGetComponent<Renderer>(out var renderer))
        {
            renderer.material.color = Color.magenta;
        }
    }

    public override void Initialize(ItemType type, bool interactable, IItemStateStrategy stateStrategy, IFrequencyStrategy freqStrategy)
    {
        base.Initialize(type, interactable, stateStrategy, new StaticFrequencyStrategy(0f));
    }

    /*public override void DestroyItem()
    {
        base.DestroyItem();
        Debug.Log($"[IRDevice] {ID} уничтожено");
    }

    protected override ItemStateType GetStateTypeFromStrategy(IItemStateStrategy strategy)
    {
        return strategy switch
        {
            IRActiveStateStrategy _ => ItemStateType.Active,
            IRPeriodicStateStrategy _ => ItemStateType.Periodic,
            _ => base.GetStateTypeFromStrategy(strategy)
        };
    }*/
}
