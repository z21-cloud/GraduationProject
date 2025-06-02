using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ST500UIController : MonoBehaviour
{
    [Header("Основные элементы")]
    [SerializeField] private Button scanButton;
    [SerializeField] private Button irScanButton;
    [SerializeField] private Button cableScanButton;
    [SerializeField] private ST500Scanner scanner;
    [SerializeField] private CableScanner cableScanner;

    [Header("Панели")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject scanPanel;
    [SerializeField] private GameObject cableScanPanel;

    [Header("Сканирование устройств")]
    [SerializeField] private Toggle showIRToggle;
    [SerializeField] private Toggle showFrequencyToggle;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject itemInfoTemplate;
    [SerializeField] private int maxDisplayedItems = 4;
    [SerializeField] private float itemSpacing = 40f;

    [Header("Подсказка для уничтожения объекта")]
    [SerializeField] private PickupController pickupController;

    [Header("Cable Scanning UI")]
    [SerializeField] private TextMeshProUGUI cableStatusText;
    [SerializeField] private Slider cableProgressSlider;
    [SerializeField] private Image sliderFill;
    public Color scanningColor = Color.blue;
    public Color vulnerabilityColor = Color.red;

    // Приватные переменные
    private GameObject currentActivePanel;
    private Coroutine scanCoroutine;
    private bool isScanningActive = false;
    private Dictionary<int, ScannedItemUI> scannedItemUIs = new Dictionary<int, ScannedItemUI>();

    public static ST500UIController Instance { get; private set; }

    private void OnEnable()
    {
        ST500Piranya.OnDeviceStateChanged += HandleDeviceStateChange;

        if (pickupController != null)
        {
            PickupController.OnItemInRange += HandleItemInRange;
            PickupController.OnItemOutOfRange += HandleItemOutOfRange;
        }

        // Подписываемся на события сканера кабелей
        if (cableScanner != null)
        {
            cableScanner.OnStatusChanged += HandleCableStatusChange;
            cableScanner.OnVulnerabilityStateChanged += HandleVulnerabilityState;
            cableScanner.OnProgressUpdated += HandleProgressUpdate;
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

        // Отписываемся от событий сканера кабелей
        if (cableScanner != null)
        {
            cableScanner.OnStatusChanged -= HandleCableStatusChange;
            cableScanner.OnVulnerabilityStateChanged -= HandleVulnerabilityState;
            cableScanner.OnProgressUpdated -= HandleProgressUpdate;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        HideAllPanels();
        SetupCableUI();
    }

    private void SetupCableUI()
    {
        if (cableProgressSlider != null)
        {
            cableProgressSlider.gameObject.SetActive(false);
            sliderFill.color = scanningColor;
            cableProgressSlider.minValue = 0;
            cableProgressSlider.maxValue = 1;
            cableProgressSlider.value = 0;
        }
    }

    private IEnumerator ScanRoutine()
    {
        while (isScanningActive)
        {
            List<ItemData> items = scanner.GetDetectedItems();
            List<ItemData> filteredItems = new List<ItemData>();

            foreach (ItemData item in items)
            {
                bool isIR = item.IsIRDevice;
                bool showIR = showIRToggle.isOn;
                bool showFreq = showFrequencyToggle.isOn;

                // Исправленная логика фильтрации:
                if ((isIR && showIR) || (!isIR && showFreq))
                {
                    filteredItems.Add(item);
                }
            }

            filteredItems.Sort((a, b) => a.Distance.CompareTo(b.Distance));

            foreach (var uiElement in scannedItemUIs.Values)
                Destroy(uiElement.gameObject);
            scannedItemUIs.Clear();

            for (int i = 0; i < Mathf.Min(filteredItems.Count, maxDisplayedItems); i++)
            {
                ItemData item = filteredItems[i];
                GameObject newItemUI = Instantiate(itemInfoTemplate, itemsContainer);
                ScannedItemUI scannedUI = newItemUI.GetComponent<ScannedItemUI>();

                RectTransform rectTransform = newItemUI.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -i * itemSpacing);

                scannedUI.Initialize(item);
                scannedItemUIs[item.ID] = scannedUI;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void OnCableScanButtonClick()
    {
        bool newScanState = !cableScanner.isScanningMode;
        cableScanner.SetScanningMode(newScanState);

        if (newScanState)
        {
            OpenPanel(cableScanPanel);
        }
        else
        {
            OpenPanel(mainMenuPanel);
        }
    }

    private void HandleCableStatusChange(string message)
    {
        if (cableStatusText != null)
        {
            cableStatusText.text = message;
        }
    }

    private void HandleVulnerabilityState(bool isFound)
    {
        if (sliderFill != null)
        {
            sliderFill.color = isFound ? vulnerabilityColor : scanningColor;
        }
    }

    private void HandleProgressUpdate(float progress)
    {
        if (cableProgressSlider != null)
        {
            if (!cableProgressSlider.gameObject.activeSelf)
            {
                cableProgressSlider.gameObject.SetActive(true);
            }

            cableProgressSlider.value = progress;
        }
    }

    public void OnRemoveVulnerability()
    {
        if (cableScanner != null && cableScanner.vulnerabilityFound)
        {
            cableScanner.RemoveVulnerability();
        }
    }

    private void Update()
    {
        // Обработка ввода для удаления уязвимости
        if (cableScanner != null &&
            cableScanner.isScanningMode &&
            cableScanner.vulnerabilityFound &&
            Input.GetKeyDown(KeyCode.E))
        {
            OnRemoveVulnerability();
        }
    }

    private void HandleDeviceStateChange(bool isActive)
    {
        if (isActive)
        {
            mainMenuPanel.SetActive(true);
        }
        else
        {
            HideAllPanels();
            HandleItemOutOfRange();

            // Выключаем сканирование устройств
            if (scanner != null && scanner.IsScanningActive)
            {
                scanner.ToggleScanning();
            }

            // Выключаем сканирование кабеля
            if (cableScanner != null && cableScanner.isScanningMode)
            {
                cableScanner.SetScanningMode(false);
            }
        }
    }

    public void OnScanButtonClick()
    {
        scanner.ToggleScanning();
        isScanningActive = scanner.IsScanningActive;

        if (scanner.IsScanningActive)
        {
            OpenPanel(scanPanel);
            scanCoroutine = StartCoroutine(ScanRoutine());
        }
    }
    public void SetScanModeToggles(bool irActive, bool frequencyActive)
    {
        showIRToggle.isOn = irActive;
        showFrequencyToggle.isOn = frequencyActive;
    }

    public void OnFrequencyScanButtonClick()
    {
        // Устанавливаем режим частотного сканирования
        scanner.SetScanMode(ScanMode.Frequency);

        // Переключаем тогглы
        showFrequencyToggle.isOn = true;
        showIRToggle.isOn = false;

        // Запускаем/обновляем сканирование
        if (!scanner.IsScanningActive)
        {
            OnScanButtonClick();
        }
        else
        {
            if (scanCoroutine != null) StopCoroutine(scanCoroutine);
            scanCoroutine = StartCoroutine(ScanRoutine());
        }
    }

    public void OnIRScanButtonClick()
    {
        // Устанавливаем режим сканирования
        scanner.SetScanMode(ScanMode.IR);

        // Настраиваем тогглы
        showIRToggle.isOn = true;
        showFrequencyToggle.isOn = false;

        // Запускаем/обновляем сканирование
        if (!scanner.IsScanningActive)
        {
            OnScanButtonClick();
        }
        else
        {
            if (scanCoroutine != null) StopCoroutine(scanCoroutine);
            scanCoroutine = StartCoroutine(ScanRoutine());
        }
    }

    public void OnBackButtonClick()
    {
        if(currentActivePanel == scanPanel && isScanningActive == true)
        {
            scanner.ToggleScanning();
            isScanningActive = false;
        }

        if(currentActivePanel == cableScanPanel && cableScanner.isScanningMode == true)
        {
            cableScanner.isScanningMode = false;
            cableScanner.SetScanningMode(false);
        }

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

    #region Panel Management
    private void OpenPanel(GameObject panel)
    {
        HideAllPanels();
        currentActivePanel = panel;
        panel.SetActive(true);

        // Специальная обработка для панели сканирования кабеля
        if (panel == cableScanPanel && cableScanner != null)
        {
            cableProgressSlider.gameObject.SetActive(cableScanner.isConnectedToCable);
        }
    }

    private void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        scanPanel.SetActive(false);
        cableScanPanel.SetActive(false);

        // Скрываем слайдер при закрытии панели
        if (cableProgressSlider != null)
        {
            cableProgressSlider.gameObject.SetActive(false);
        }
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