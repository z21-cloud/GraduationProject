using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ST500UIController : MonoBehaviour
{
    [Header("�������� ��������")]
    [SerializeField] private Button scanButton;
    [SerializeField] private Button cableScanButton;
    [SerializeField] private ST500Scanner scanner;

    [Header("������")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject scanPanel;
    [SerializeField] private GameObject cableScanPanel;

    [Header("������������")]
    [SerializeField] private Transform itemsContainer; // ��������� ��� ���������
    [SerializeField] private GameObject itemInfoTemplate; // ������ ScannedItemUI
    [SerializeField] private int maxDisplayedItems = 4; // �������� 4 ��������
    [SerializeField] private float itemSpacing = 40f; // ���������� ����� ����������

    private GameObject currentActivePanel;
    private Coroutine scanCoroutine;
    private bool isScanningActive = false;
    private Dictionary<int, ScannedItemUI> scannedItemUIs = new Dictionary<int, ScannedItemUI>();

    private IEnumerator ScanRoutine()
    {
        while (isScanningActive)
        {
            List<ItemData> items = scanner.GetDetectedItems();

            // ��������� �� ����������
            items.Sort((a, b) => a.Distance.CompareTo(b.Distance));

            // ������� ������ ��������
            foreach (var uiElement in scannedItemUIs.Values)
                Destroy(uiElement.gameObject);
            scannedItemUIs.Clear();

            // ������� ����� �������� UI
            for (int i = 0; i < Mathf.Min(items.Count, maxDisplayedItems); i++)
            {
                ItemData item = items[i];
                GameObject newItemUI = Instantiate(itemInfoTemplate, itemsContainer);
                ScannedItemUI scannedUI = newItemUI.GetComponent<ScannedItemUI>();

                // ������������� ������� ����� RectTransform
                RectTransform rectTransform = newItemUI.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -i * itemSpacing);

                scannedUI.Initialize(item);
                scannedItemUIs[item.ID] = scannedUI;
            }

            yield return new WaitForSeconds(0.5f); // ���������� ������ 0.5 ������
        }
    }

    private void OnEnable()
    {
        ST500Piranya.OnDeviceStateChanged += HandleDeviceStateChange;
    }

    private void OnDisable()
    {
        ST500Piranya.OnDeviceStateChanged -= HandleDeviceStateChange;
    }

    private void Start()
    {
        // �������� ��� ������ ��� ������
        HideAllPanels();
    }

    private void HandleDeviceStateChange(bool isActive)
    {
        if (isActive)
        {
            // �������� UI � ������
            mainMenuPanel.SetActive(true);
        }
        else
        {
            // ��������� UI � ������
            HideAllPanels();
        }
    }

    public void OnScanButtonClick()
    {
        //�������� �������
        // ��������� ������ ������������
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

    private void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        scanPanel.SetActive(false);
        cableScanPanel.SetActive(false);
    }

    private void OpenPanel(GameObject panel)
    {
        HideAllPanels();
        currentActivePanel = panel;
        panel.SetActive(true);
    }

    private void CloseCurrentPanel()
    {
        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
            currentActivePanel = null;
        }
    }
}