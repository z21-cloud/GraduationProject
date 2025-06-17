using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oblik2Controller : MonoBehaviour
{
    [SerializeField] private GameObject UI;
    private SimpleScanner scanner;

    private void Awake()
    {
        scanner = GetComponent<SimpleScanner>();
    }

    public void Initialize()
    {
        if (scanner != null)
        {
            scanner.Initialize();
        }
        Debug.Log("�����-2 ���������������");
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        UI.SetActive(active);
        if (scanner != null)
        {
            scanner.SetActive(active);
        }
    }

    public bool IsActive => gameObject.activeSelf && (scanner?.isBinocularEquipped ?? false);
}
