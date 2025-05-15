using UnityEngine;

[RequireComponent(typeof(ItemContext))]
public class ST500Piranya : MonoBehaviour
{
    [SerializeField] private bool isDeviceActive = false;
    [SerializeField] private ST500ButtonAnimator buttonAnimator;
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
            Debug.Log($"ST-500: ��������� ���������� �������� �� {(isDeviceActive ? "��������" : "���������")}");
        }
    }

    public void ToggleDevice()
    {
        isDeviceActive = !isDeviceActive;
        Debug.Log($"ST-500: ���������� {(isDeviceActive ? "��������" : "���������")}");
        buttonAnimator.UpdateButtonState(isDeviceActive);
    }

    public bool CanInteract()
    {
        return isDeviceActive && itemContext != null && !itemContext.IsDestroyed;
    }
}