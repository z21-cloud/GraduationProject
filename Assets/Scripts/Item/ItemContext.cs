using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ItemContext : MonoBehaviour
{
    // Поля, отображаемые в инспекторе
    [SerializeField] private ItemType _type;
    [SerializeField] private bool _interactable;
    [SerializeField] private float _frequency;
    [SerializeField] private int _id;
    [SerializeField] private Color _color;
    [SerializeField] private ItemStateType _stateType;
    [SerializeField] private bool _isDestroyed;

    // Свойства (для внутреннего использования)
    public ItemType Type => _type;
    public bool Interactable => _interactable;
    public float Frequency => _frequency;
    public int ID => _id;
    public Color Color => _color;
    public ItemStateType StateType => _stateType;
    public bool IsDestroyed => _isDestroyed;

    // Стратегия (внутреннее состояние)
    private IItemStateStrategy currentStrategy;

    // Статический счетчик ID
    private static int idCounter = 1;

    // Инициализация предмета
    public void Initialize(ItemType type, bool interactable, IItemStateStrategy strategy)
    {
        this._type = type;
        this._interactable = interactable;
        this._frequency = UnityEngine.Random.Range(1f, 1000f);
        this._id = idCounter++;
        this._stateType = GetStateTypeFromStrategy(strategy);
        this._isDestroyed = false;
        this.currentStrategy = strategy;

        // Устанавливаем цвет
        this._color = GetColorFromType(type);

        // Применяем цвет к материалу
        if (TryGetComponent(out Renderer renderer))
        {
            renderer.material.color = _color;
        }

        // Инициируем стратегию
        strategy.OnStateEnter(this);
    }

    // Метод для изменения состояния
    public void ChangeState(IItemStateStrategy newStrategy)
    {
        currentStrategy.OnStateExit();
        currentStrategy = newStrategy;
        this._stateType = GetStateTypeFromStrategy(newStrategy);
        newStrategy.OnStateEnter(this);
    }

    // Передача данных
    public void TransmitData()
    {
        if (_isDestroyed) return;

        ItemData data = new ItemData
        {
            ID = _id,
            Type = _type,
            Frequency = _frequency,
            Interactable = _interactable,
            Color = _color,
            State = _stateType.ToString()
        };

        currentStrategy.TransmitData(data);
    }

    // Уничтожение предмета
    public void DestroyItem()
    {
        _isDestroyed = true;
        Debug.Log($"Предмет {_id} уничтожен");
        Destroy(gameObject);
    }

    // Вспомогательные методы
    private ItemStateType GetStateTypeFromStrategy(IItemStateStrategy strategy)
    {
        return strategy switch
        {
            ActiveStateStrategy _ => ItemStateType.Active,
            PassiveStateStrategy _ => ItemStateType.Passive,
            PeriodicStateStrategy _ => ItemStateType.Periodic,
            _ => ItemStateType.Passive
        };
    }

    private Color GetColorFromType(ItemType type)
    {
        return type switch
        {
            ItemType.Safe => Color.green,
            ItemType.Dangerous => Color.red,
            ItemType.Neutral => new Color(1, 0.64f, 0), // Оранжевый
            _ => Color.white
        };
    }

    // Для обновления цвета в редакторе
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        _color = GetColorFromType(_type);

        if (TryGetComponent(out Renderer renderer))
        {
            renderer.sharedMaterial.color = _color;
        }
        else
        {
            Debug.LogWarning($"У объекта {name} нет Renderer. Цвет не будет применён в редакторе.");
        }
    }
}