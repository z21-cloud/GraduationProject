using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ST500Scanner))]
[RequireComponent(typeof(ST500Piranya))]
public class ST500ScannerAudioController : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float minVolume = 0.1f;
    [SerializeField] private float maxVolume = 0.5f;
    [SerializeField] private float pitchMin = 0.9f;
    [SerializeField] private float pitchMax = 1.1f;
    [SerializeField] private float volumeSmoothTime = 0.2f;
    [SerializeField] private float pitchSmoothTime = 0.2f;

    private ST500Scanner scanner;
    private ST500Piranya piranya;
    private float currentVolumeVelocity;
    private float currentPitchVelocity;
    private float targetVolume;
    private float targetPitch;
    private bool wasPlaying;

    private void Awake()
    {
        scanner = GetComponent<ST500Scanner>();
        piranya = GetComponent<ST500Piranya>();

        if (scanner == null)
            Debug.LogError("ST500Scanner component not found!");
        if (piranya == null)
            Debug.LogError("ST500Piranya component not found!");
    }

    private void OnEnable()
    {
        ST500Scanner.OnScanningStateChanged += HandleScanningStateChanged;
    }

    private void OnDisable()
    {
        ST500Scanner.OnScanningStateChanged -= HandleScanningStateChanged;
        StopDeviceScanningSound();
    }

    private void Update()
    {
        UpdateScanningAudio();
    }

    private void UpdateScanningAudio()
    {
        if (AudioManager.Instance == null) return;

        if (scanner != null && scanner.IsScanningActive && piranya.IsDeviceActive)
        {
            List<ItemData> detectedItems = scanner.GetDetectedItems();

            if (detectedItems.Count > 0)
            {
                /// Находим ближайший объект
                ItemData closestItem = detectedItems[0];
                float minDistance = closestItem.Distance;

                // Ищем реальное минимальное расстояние
                foreach (ItemData item in detectedItems)
                {
                    if (item.Distance < minDistance) minDistance = item.Distance;
                }

                // Рассчитываем целевую громкость и тон
                targetVolume = CalculateVolume(minDistance);
                targetPitch = CalculatePitch(minDistance);

                // Плавное изменение параметров
                float volume = Mathf.SmoothDamp(
                    AudioManager.Instance.GetLoopingVolume(),
                    targetVolume,
                    ref currentVolumeVelocity,
                    volumeSmoothTime
                );

                float pitch = Mathf.SmoothDamp(
                    AudioManager.Instance.GetLoopingPitch(),
                    targetPitch,
                    ref currentPitchVelocity,
                    pitchSmoothTime
                );

                // Воспроизводим/обновляем звук
                PlayDeviceScanningSound(volume, pitch);
            }
            else
            {
                // Плавное уменьшение громкости перед остановкой
                targetVolume = 0;
                float volume = Mathf.SmoothDamp(
                    AudioManager.Instance.GetLoopingVolume(),
                    targetVolume,
                    ref currentVolumeVelocity,
                    volumeSmoothTime
                );

                if (volume < 0.1f)
                {
                    StopDeviceScanningSound();
                }
                else
                {
                    AudioManager.Instance.UpdateLoopingSound(volume, AudioManager.Instance.GetLoopingPitch());
                }
            }
        }
        else
        {
            StopDeviceScanningSound();
        }
    }

    private float CalculateVolume(float distance)
    {
        // Громкость обратно пропорциональна расстоянию
        return Mathf.Lerp(minVolume, maxVolume,
            Mathf.InverseLerp(maxDistance, minDistance, distance));
    }

    private float CalculatePitch(float distance)
    {
        // Чем ближе объект, тем выше тон
        return Mathf.Lerp(pitchMin, pitchMax,
            1 - Mathf.InverseLerp(minDistance, maxDistance, distance));
    }

    private void PlayDeviceScanningSound(float volume, float pitch)
    {
        if (AudioManager.Instance == null) return;
        if (!AudioManager.Instance.IsLoopingSoundPlaying())
        {
            AudioManager.Instance.PlaySound(
                AudioManager.Instance.deviceScanningSound,
                volume,
                pitch,
                true
            );
        }
        else
        {
            AudioManager.Instance.UpdateLoopingSound(volume, pitch);
        }
    }

    private void StopDeviceScanningSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopLoopingSound();
        }
    }

    private void HandleScanningStateChanged(bool isScanningActive)
    {
        if (!isScanningActive)
        {
            StopDeviceScanningSound();
        }

        // Добавляем проверку на активность устройства
        if (!piranya.IsDeviceActive)
        {
            StopDeviceScanningSound();
        }
    }
}