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
    [SerializeField] private GameObject cableScanPanel;

    [Header("Сканирование")]
    [SerializeField] private Transform itemsContainer; // Контейнер для элементов
    [SerializeField] private GameObject itemInfoTemplate; // Префаб ScannedItemUI
    [SerializeField] private int maxDisplayedItems = 4; // Максимум 4 предмета
    [SerializeField] private float itemSpacing = 40f; // Расстояние между элементами

    [Header("Подсказка для уничтожения объекта")]
    [SerializeField] private TextMeshProUGUI destroyPromptText; // Текст подсказки
    [SerializeField] private PickupController pickupController; // Ссылка на PickupController

    private GameObject currentActivePanel;
    private Coroutine scanCoroutine;
    private bool isScanningActive = false;
    private Dictionary<int, ScannedItemUI> scannedItemUIs = new Dictionary<int, ScannedItemUI>();

    private void OnEnable()
    {
        ST500Piranya.OnDeviceStateChanged += HandleDeviceStateChange;

        if (pickupController != null)
        {
            PickupController.OnItemInRange += ShowDestroyPrompt;
            PickupController.OnItemOutOfRange += HideDestroyPrompt;
        }
    }

    private void OnDisable()
    {
        ST500Piranya.OnDeviceStateChanged -= HandleDeviceStateChange;

        if (pickupController != null)
        {
            PickupController.OnItemInRange -= ShowDestroyPrompt;
            PickupController.OnItemOutOfRange -= HideDestroyPrompt;
        }
    }

    private IEnumerator ScanRoutine()
    {
        while (isScanningActive)
        {
            List<ItemData> items = scanner.GetDetectedItems();

            // Сортируем по расстоянию
            items.Sort((a, b) => a.Distance.CompareTo(b.Distance));

            // Очищаем старые элементы
            foreach (var uiElement in scannedItemUIs.Values)
                Destroy(uiElement.gameObject);
            scannedItemUIs.Clear();

            // Создаем новые элементы UI
            for (int i = 0; i < Mathf.Min(items.Count, maxDisplayedItems); i++)
            {
                ItemData item = items[i];
                GameObject newItemUI = Instantiate(itemInfoTemplate, itemsContainer);
                ScannedItemUI scannedUI = newItemUI.GetComponent<ScannedItemUI>();

                // Устанавливаем позицию через RectTransform
                RectTransform rectTransform = newItemUI.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -i * itemSpacing);

                scannedUI.Initialize(item);
                scannedItemUIs[item.ID] = scannedUI;
            }

            yield return new WaitForSeconds(0.5f); // Обновление каждые 0.5 секунд
        }
    }

    private void Start()
    {
        // Скрываем все панели при старте
        HideAllPanels();
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
            HideDestroyPrompt();
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
    private void ShowDestroyPrompt(ItemData itemData)
    {
        if (itemData.Interactable)
        {
            destroyPromptText.text = $"Нажмите E для уничтожения ID: {itemData.ID}";
            destroyPromptText.gameObject.SetActive(true);
        }
    }

    private void HideDestroyPrompt()
    {
        if (destroyPromptText != null)
            destroyPromptText.gameObject.SetActive(false);
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