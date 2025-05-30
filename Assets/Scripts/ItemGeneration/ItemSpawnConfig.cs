using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSpawnConfig", menuName = "Configs/ItemSpawnConfig")]
public class ItemSpawnConfig : ScriptableObject
{
    [Header("Параметры генерации")]
    public int ItemsToSpawn = 5;

    [Header("Вероятности")]
    [Range(0, 1)] public float GSMChance = 0.2f;
    [Range(0, 1)] public float LTEChance = 0.2f;
    [Range(0, 1)] public float WifiChance = 0.2f;
    [Range(0, 1)] public float BluetoothChance = 0.2f;

    [Header("Вероятности состояний")]
    [Range(0, 1)] public float ActiveChance = 0.7f;
    [Range(0, 1)] public float PeriodicChance = 0.3f;

    [Header("Вероятность взаимодействия")]
    [Range(0, 1)] public float InteractableChance = 0.9f;
}