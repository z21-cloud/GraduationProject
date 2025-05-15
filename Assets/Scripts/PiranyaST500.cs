using UnityEngine;

[RequireComponent(typeof(ItemContext))]
public class ST500Piranya : MonoBehaviour
{
    [SerializeField] private bool isDeviceActive = false;
    public bool IsDeviceActive => isDeviceActive;

    private ItemContext itemContext;

    private void Awake()
    {
        itemContext = GetComponent<ItemContext>();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            // ��� ������� � ���������
            GetComponent<Renderer>().sharedMaterial.color = isDeviceActive ? Color.green : Color.red;
            Debug.Log($"ST-500: ��������� ���������� �������� �� {(isDeviceActive ? "��������" : "���������")}");
        }
    }

    public void ToggleDevice()
    {
        isDeviceActive = !isDeviceActive;
        GetComponent<Renderer>().sharedMaterial.color = isDeviceActive ? Color.green : Color.red; //������� ��������� ��������� �� ����� ���� ��� ������
        Debug.Log($"ST-500: ���������� {(isDeviceActive ? "��������" : "���������")}");
    }

    public bool CanInteract()
    {
        return isDeviceActive && itemContext != null && !itemContext.IsDestroyed;
    }
}