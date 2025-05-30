using UnityEngine;

public class ST500Piranya : MonoBehaviour
{
    [SerializeField] private bool isDeviceActive = false;
    public bool IsDeviceActive => isDeviceActive;


    [SerializeField] private ST500ButtonAnimator buttonAnimator;

    public delegate void DeviceStateChanged(bool isActive);
    public static event DeviceStateChanged OnDeviceStateChanged;

    private ItemContext itemContext;

    private void Awake()
    {
        itemContext = GetComponent<ItemContext>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            // Для отладки в редакторе
            Debug.Log($"ST-500: Состояние устройства изменено на {(isDeviceActive ? "включено" : "выключено")}");
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleDevice();
        }
    }

    public void ToggleDevice()
    {
        isDeviceActive = !isDeviceActive;
        Debug.Log($"ST-500: Устройство {(isDeviceActive ? "включено" : "выключено")}");
        buttonAnimator.UpdateButtonState(isDeviceActive);
        OnDeviceStateChanged?.Invoke(isDeviceActive);
        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        if (isDeviceActive)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public bool CanInteract()
    {
        return isDeviceActive && itemContext != null && !itemContext.IsDestroyed;
    }
}