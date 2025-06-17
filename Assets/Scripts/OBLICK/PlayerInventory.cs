using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // Упрощаем, так как управление теперь через PickupController
    [SerializeField] private SimpleScanner binocularScanner;

    public void SetScanner(SimpleScanner scanner)
    {
        binocularScanner = scanner;
    }
}
