using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ItemFactory itemFactory;
    [SerializeField] private List<Transform> spawnNodes;
    [SerializeField] private ItemSpawnConfig spawnConfig;

    private readonly List<Transform> availableNodes = new List<Transform>();

    private void Start()
    {
        InitializeNodes();
        StartCoroutine(SpawnItems());
    }

    private void InitializeNodes()
    {
        availableNodes.Clear();
        availableNodes.AddRange(spawnNodes);

        if (availableNodes.Count == 0)
        {
            Debug.LogError("Не найдено нод для спавна предметов!");
        }
    }

    private void OnDrawGizmos()
    {
        if (spawnNodes == null || spawnNodes.Count == 0) return;

        Gizmos.color = Color.yellow;
        foreach (Transform node in spawnNodes)
        {
            Gizmos.DrawSphere(node.position, 0.2f);
        }
    }

    private IEnumerator SpawnItems()
    {
        for (int i = 0; i < spawnConfig.ItemsToSpawn; i++)
        {
            if (availableNodes.Count == 0)
            {
                Debug.LogWarning("Недостаточно нод для спавна всех предметов");
                break;
            }

            // Выбираем случайную ноду
            int randomIndex = Random.Range(0, availableNodes.Count);
            Transform spawnPoint = availableNodes[randomIndex];
            availableNodes.RemoveAt(randomIndex);

            // Генерируем параметры предмета
            ItemType itemType = GetRandomItemType();
            bool isInteractable = Random.value <= spawnConfig.InteractableChance;
            IItemStateStrategy stateStrategy = GetRandomStateStrategy();

            // Создаем предмет
            GameObject newItem = itemFactory.CreateItem(
                itemType,
                isInteractable,
                stateStrategy,
                spawnPoint.position
            );

            // Опционально: добавляем визуальный эффект спавна
            yield return new WaitForSeconds(0.2f); // Задержка между спавнами
        }

        Debug.Log("Генерация предметов завершена");
    }

    private ItemType GetRandomItemType()
    {
        float randomValue = Random.value;

        if (randomValue < spawnConfig.SafeChance)
            return ItemType.Safe;
        else if (randomValue < spawnConfig.SafeChance + spawnConfig.DangerousChance)
            return ItemType.Dangerous;
        else
            return ItemType.Neutral;
    }

    private IItemStateStrategy GetRandomStateStrategy()
    {
        float randomValue = Random.value;

        if (randomValue < spawnConfig.ActiveChance)
            return new ActiveStateStrategy();
        else if (randomValue < spawnConfig.ActiveChance + spawnConfig.PassiveChance)
            return new PassiveStateStrategy();
        else
            return new PeriodicStateStrategy();
    }
}