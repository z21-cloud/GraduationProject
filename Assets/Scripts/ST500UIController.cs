using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ST500UIController : MonoBehaviour
{
    [Header("Основные элементы")]
    [SerializeField] private Button scanButton;
    [SerializeField] private Button cableScanButton;
    [SerializeField] private ST500Scanner scanner;

    [Header("Панели")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject scanPanel;

    [Header("Сканирование")]
    [SerializeField] private Toggle showIRToggle;
    [SerializeField] private Toggle showFrequencyToggle;
    [SerializeField] private Transform itemsContainer; // Контейнер для элементов
    [SerializeField] private GameObject itemInfoTemplate; // Префаб ScannedItemUI
    [SerializeField] private int maxDisplayedItems = 4; // Максимум 4 предмета
    [SerializeField] private float itemSpacing = 40f; // Расстояние между элементами

    [Header("Подсказка для уничтожения объекта")]
    //[SerializeField] private TextMeshProUGUI destroyPromptText; // Текст подсказки
    [SerializeField] private PickupController pickupController; // Ссылка на PickupController

    [Header("Cable Scanning")]
    [SerializeField] private GameObject cableScanPanel;
    [SerializeField] private TextMeshProUGUI cableStatusText;
    [SerializeField] private Slider scanProgressSlider;

    [SerializeField] private CableScanner cableScanner;
    private GameObject currentActivePanel;
    private Coroutine scanCoroutine;
    private bool isScanningActive = false;
    private Dictionary<int, ScannedItemUI> scannedItemUIs = new Dictionary<int, ScannedItemUI>();

    private void OnEnable()
    {
        ST500Piranya.OnDeviceStateChanged += HandleDeviceStateChange;

        if (pickupController != null)
        {
            PickupController.OnItemInRange += HandleItemInRange;
            PickupController.OnItemOutOfRange += HandleItemOutOfRange;
        }
    }

    private void OnDisable()
    {
        ST500Piranya.OnDeviceStateChanged -= HandleDeviceStateChange;

        if (pickupController != null)
        {
            PickupController.OnItemInRange -= HandleItemInRange;
            PickupController.OnItemOutOfRange -= HandleItemOutOfRange;
        }
    }

    private IEnumerator ScanRoutine()
    {
        while (isScanningActive)
        {
            List<ItemData> items = scanner.GetDetectedItems();
            Debug.Log($"Найдено устройств: {items.Count}");
            // Фильтруем по галочкам
            List<ItemData> filteredItems = new List<ItemData>();
            foreach (ItemData item in items)
            {
                Debug.Log($"Устройство: ID={item.ID}, Тип={item.Type}, IR={item.IsIRDevice}");
                if ((showIRToggle.isOn && item.IsIRDevice) ||
                    (showFrequencyToggle.isOn && !item.IsIRDevice))
                {
                    filteredItems.Add(item);
                }
            }

            // Сортируем по расстоянию
            filteredItems.Sort((a, b) => a.Distance.CompareTo(b.Distance));

            // Очищаем старые элементы
            foreach (var uiElement in scannedItemUIs.Values)
                Destroy(uiElement.gameObject);
            scannedItemUIs.Clear();

            // Создаем новые элементы UI
            for (int i = 0; i < Mathf.Min(filteredItems.Count, maxDisplayedItems); i++)
            {
                ItemData item = filteredItems[i];
                if ((showIRToggle.isOn && item.IsIRDevice) || (showFrequencyToggle.isOn && !item.IsIRDevice))
                {
                    GameObject newItemUI = Instantiate(itemInfoTemplate, itemsContainer);
                    ScannedItemUI scannedUI = newItemUI.GetComponent<ScannedItemUI>();

                    // Устанавливаем позицию через RectTransform
                    RectTransform rectTransform = newItemUI.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(0, -i * itemSpacing);

                    scannedUI.Initialize(item);
                    scannedItemUIs[item.ID] = scannedUI;
                }
            }

            yield return new WaitForSeconds(0.5f); // Обновление каждые 0.5 секунд
        }
    }

    private void Start()
    {
        // Скрываем все панели при старте
        HideAllPanels();
        //cableScanner = GetComponent<CableScanner>();
    }

    public void OnCableScanButtonClick()
    {
        cableScanner.SetScanningMode(!cableScanner.isScanningMode);
        OpenPanel(cableScanPanel);

        if (!cableScanner.isScanningMode)
        {
            mainMenuPanel.SetActive(true);
        }
    }

    public void ShowCableScanStatus(string message)
    {
        cableStatusText.text = message;
    }

    private void UpdateCableScanUI()
    {
        if (!cableScanner.isScanningMode) return;

        if (cableScanner.vulnerabilityFound)
        {
            // Рассчитываем прогресс по расстоянию
            float distance = Vector3.Distance(
                cableScanner.transform.position,
                cableScanner.currentVulnerability.transform.position
            );

            scanProgressSlider.value = 1 - Mathf.Clamp01(distance / 2f);
        }
    }

    private void Update()
    {
        UpdateCableScanUI();

        if (cableScanner.isScanningMode && Input.GetKeyDown(KeyCode.E))
        {
            cableScanner.RemoveVulnerability();
        }
    }

    private void HandleDeviceStateChange(bool isActive)
    {
        if (isActive)
        {
            // Включаем UI и курсор
            mainMenuPanel.SetActive(true);
        }
        else
        {
            // Отключаем UI и курсор
            HideAllPanels();
            HandleItemOutOfRange();
        }
    }

    public void OnScanButtonClick()
    {
        //включаем сканнер
        // Открываем панель сканирования
        scanner.ToggleScanning();
        isScanningActive = scanner.IsScanningActive;
        if (scanner.IsScanningActive)
        {
            OpenPanel(scanPanel); 
            scanCoroutine = StartCoroutine(ScanRoutine());
        }
    }

    public void OnBackButtonClick()
    {
        scanner.ToggleScanning();
        CloseCurrentPanel();
        OpenPanel(mainMenuPanel);
    }

    #region DestroyPrompt
    private void HandleItemInRange(ItemData itemData)
    {
        if (scannedItemUIs.TryGetValue(itemData.ID, out ScannedItemUI ui))
        {
            ui.SetHighlight(true);
        }
    }

    private void HandleItemOutOfRange()
    {
        foreach (var pair in scannedItemUIs)
        {
            pair.Value.SetHighlight(false);
        }
    }
    #endregion

    #region Open/CloseCurrent/HideAll Panel
    private void OpenPanel(GameObject panel)
    {
        HideAllPanels();
        currentActivePanel = panel;
        panel.SetActive(true);
    }

    private void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        scanPanel.SetActive(false);
        cableScanPanel.SetActive(false);
    }

    private void CloseCurrentPanel()
    {
        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
            currentActivePanel = null;
        }
    }
    #endregion
}