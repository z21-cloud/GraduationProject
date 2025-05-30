using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab; // Для обычных предметов
    [SerializeField] private GameObject irDevicePrefab; // Для ИК-устройств

    public GameObject CreateItem(ItemType type, bool interactable, IItemStateStrategy stateStrategy, IFrequencyStrategy freqStrategy, Vector3 position)
    {
        GameObject newItem = null;
        ItemContext context = null;

        if (type == ItemType.IRDevice)
        {
            newItem = Instantiate(irDevicePrefab, position, Quaternion.identity);
            context = newItem.GetComponent<IRDeviceContext>();
        }
        else
        {
            newItem = Instantiate(itemPrefab, position, Quaternion.identity);
            context = newItem.GetComponent<ItemContext>();
        }

        if (context != null)
        {
            context.Initialize(type, interactable, stateStrategy, freqStrategy);
        }

        return newItem;
    }
}
