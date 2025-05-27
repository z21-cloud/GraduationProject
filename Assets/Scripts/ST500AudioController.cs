using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ST500ScannerAudioController : MonoBehaviour
{
    [Header("��������� �����")]
    [SerializeField] private float minDistance = 1f; // ����������� ���������� (������������ ���������)
    [SerializeField] private float maxDistance = 10f; // ������������ ���������� (����������� ���������)
    [SerializeField] private float minVolume = 0.1f; // ����������� ���������
    [SerializeField] private float maxVolume = .5f; // ������������ ���������

    private AudioSource audioSource;
    private ST500Scanner scanner;
    private ST500Piranya piranya;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        scanner = GetComponentInParent<ST500Scanner>();
        piranya = GetComponentInParent<ST500Piranya>();

        if (scanner == null)
            Debug.LogError("�� ������ ��������� ST500Scanner");
        if (piranya == null)
            Debug.LogError("�� ������ ��������� ST500Piranya");
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
                // ����� ��������� ������
                ItemData closestItem = detectedItems[0];
                float distance = closestItem.Distance;

                // ������������ ���������
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
        // ��������� ������� �� ����������
        return Mathf.InverseLerp(maxDistance, minDistance, distance) * (maxVolume - minVolume) + minVolume;
    }

    private void HandleScanningStateChanged(bool isScanningActive)
    {
        if (!isScanningActive && audioSource.isPlaying)
            audioSource.Stop();
    }
}