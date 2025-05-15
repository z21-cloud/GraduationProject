using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSpawnConfig", menuName = "Configs/ItemSpawnConfig")]
public class ItemSpawnConfig : ScriptableObject
{
    [Header("Параметры генерации")]
    public int ItemsToSpawn = 5;

    [Header("Вероятности")]
    [Range(0, 1)] public float SafeChance = 0.4f;
    [Range(0, 1)] public float DangerousChance = 0.3f;
    [Range(0, 1)] public float NeutralChance = 0.3f;

    [Header("Вероятности состояний")]
    [Range(0, 1)] public float ActiveChance = 0.4f;
    [Range(0, 1)] public float PassiveChance = 0.3f;
    [Range(0, 1)] public float PeriodicChance = 0.3f;

    [Header("Вероятность взаимодействия")]
    [Range(0, 1)] public float InteractableChance = 0.7f;
}