using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ST500ScannerAudioController : MonoBehaviour
{
    [Header("Настройки звука")]
    [SerializeField] private float minDistance = 1f; // Минимальное расстояние (максимальная громкость)
    [SerializeField] private float maxDistance = 10f; // Максимальное расстояние (минимальная громкость)
    [SerializeField] private float minVolume = 0.1f; // Минимальная громкость
    [SerializeField] private float maxVolume = .5f; // Максимальная громкость

    private AudioSource audioSource;
    private ST500Scanner scanner;
    private ST500Piranya piranya;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        scanner = GetComponentInParent<ST500Scanner>();
        piranya = GetComponentInParent<ST500Piranya>();

        if (scanner == null)
            Debug.LogError("Не найден компонент ST500Scanner");
        if (piranya == null)
            Debug.LogError("Не найден компонент ST500Piranya");
    }

    private void OnEnable()
    {
        if (scanner != null)
            ST500Scanner.OnScanningStateChanged += HandleScanningStateChanged;
    }

    private void OnDisable()
    {
        if (scanner != null)
            ST500Scanner.OnScanningStateChanged -= HandleScanningStateChanged;
    }

    private void Start()
    {
        if (audioSource != null && audioSource.clip != null)
            audioSource.loop = true;
    }

    private void Update()
    {
        if (scanner != null && scanner.IsScanningActive && piranya.IsDeviceActive)
        {
            List<ItemData> detectedItems = scanner.GetDetectedItems();
            if (detectedItems.Count > 0)
            {
                // Берем ближайший объект
                ItemData closestItem = detectedItems[0];
                float distance = closestItem.Distance;

                // Рассчитываем громкость
                float volume = CalculateVolume(distance);
                audioSource.volume = volume;

                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
            else
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();
            }
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    private float CalculateVolume(float distance)
    {
        // Громкость зависит от расстояния
        return Mathf.InverseLerp(maxDistance, minDistance, distance) * (maxVolume - minVolume) + minVolume;
    }

    private void HandleScanningStateChanged(bool isScanningActive)
    {
        if (!isScanningActive && audioSource.isPlaying)
            audioSource.Stop();
    }
}