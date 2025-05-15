using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    public GameObject CreateItem(ItemType type, bool interactable, IItemStateStrategy stateStrategy, Vector3 position)
    {
        GameObject newItem = Instantiate(itemPrefab, position, Quaternion.identity);
        ItemContext context = newItem.GetComponent<ItemContext>();
        context.Initialize(type, interactable, stateStrategy);
        return newItem;
    }
}
