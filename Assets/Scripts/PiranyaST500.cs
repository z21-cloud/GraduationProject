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
            // Для отладки в редакторе
            GetComponent<Renderer>().sharedMaterial.color = isDeviceActive ? Color.green : Color.red;
            Debug.Log($"ST-500: Состояние устройства изменено на {(isDeviceActive ? "включено" : "выключено")}");
        }
    }

    public void ToggleDevice()
    {
        isDeviceActive = !isDeviceActive;
        GetComponent<Renderer>().sharedMaterial.color = isDeviceActive ? Color.green : Color.red; //добавил изменение материала во время игры для дебага
        Debug.Log($"ST-500: Устройство {(isDeviceActive ? "включено" : "выключено")}");
    }

    public bool CanInteract()
    {
        return isDeviceActive && itemContext != null && !itemContext.IsDestroyed;
    }
}