using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ST500UIController : MonoBehaviour
{
    [Header("�������� ��������")]
    [SerializeField] private Button scanButton;
    [SerializeField] private Button frequencySettingsButton;
    [SerializeField] private Button cableScanButton;

    [Header("������")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject frequencySettingsPanel;
    [SerializeField] private GameObject scanPanel;
    [SerializeField] private GameObject cableScanPanel;

    [Header("�������� �������")]
    [SerializeField] private Color blinkRed = Color.red;
    [SerializeField] private Color blinkGreen = Color.green;
    [SerializeField] private float blinkDuration = 2f;
    [SerializeField] private float blinkInterval = 0.5f;

    private Coroutine blinkCoroutine;
    private GameObject currentActivePanel;
    private bool isFrequencySet = false;

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
        if (!isFrequencySet)
        {
            // ������ ��������
            BlinkButtons(scanButton, frequencySettingsButton);
            return;
        }

        // ��������� ������ ������������
        OpenPanel(scanPanel);
    }

    public void OpenFrequencySettings()
    {
        OpenPanel(frequencySettingsPanel);
    }

    public void BackButton()
    {
        CloseCurrentPanel();
        OpenPanel(mainMenuPanel);
    }

    public void ConfirmFrequencySettings()
    {
        // ������ ���������� �������
        isFrequencySet = true;
        CloseCurrentPanel();
        OpenPanel(mainMenuPanel);
    }

    private void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        frequencySettingsPanel.SetActive(false);
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

    private void BlinkButtons(params Button[] buttons)
    {
        // ������������� ���������� ��������
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        // ��������� �������
        blinkCoroutine = StartCoroutine(BlinkRoutine(buttons));
    }

    private IEnumerator BlinkRoutine(Button[] buttons)
    {
        float elapsedTime = 0f;
        while (elapsedTime < blinkDuration)
        {
            foreach (Button button in buttons)
            {
                if (button == scanButton)
                    button.image.color = blinkRed;
                else if (button == frequencySettingsButton)
                    button.image.color = blinkGreen;
            }

            yield return new WaitForSeconds(blinkInterval / 2f);

            foreach (Button button in buttons)
                button.image.color = Color.white;

            yield return new WaitForSeconds(blinkInterval / 2f);

            elapsedTime += blinkInterval;
        }

        // ���������� �������� ����
        foreach (Button button in buttons)
            button.image.color = Color.white;
    }
}