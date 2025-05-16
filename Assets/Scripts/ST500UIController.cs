using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ST500UIController : MonoBehaviour
{
    [Header("Основные элементы")]
    [SerializeField] private Button scanButton;
    [SerializeField] private Button frequencySettingsButton;
    [SerializeField] private Button cableScanButton;

    [Header("Панели")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject frequencySettingsPanel;
    [SerializeField] private GameObject scanPanel;
    [SerializeField] private GameObject cableScanPanel;

    [Header("Анимация мигания")]
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
        }
    }

    public void OnScanButtonClick()
    {
        if (!isFrequencySet)
        {
            // Мигаем кнопками
            BlinkButtons(scanButton, frequencySettingsButton);
            return;
        }

        // Открываем панель сканирования
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
        // Логика сохранения частоты
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
        // Останавливаем предыдущую корутину
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        // Запускаем мигание
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

        // Возвращаем исходный цвет
        foreach (Button button in buttons)
            button.image.color = Color.white;
    }
}