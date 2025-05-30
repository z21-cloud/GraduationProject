using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

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
    public ItemType Type
    {
        get => _type;
        protected set => _type = value;
    }

    public bool Interactable
    {
        get => _interactable;
        protected set => _interactable = value;
    }

    public float Frequency
    {
        get => _frequency;
        protected set => _frequency = value;
    }

    public int ID => _id;
    public Color Color
    {
        get => _color;
        protected set => _color = value;
    }
    public ItemStateType StateType
    {
        get => _stateType;
        protected set => _stateType = value;
    }

    public bool IsDestroyed => _isDestroyed;
    public IItemStateStrategy CurrentStrategy { get; private set; }
    private IFrequencyStrategy frequencyStrategy;

    public bool IsIRDevice => Type == ItemType.IRDevice;

    // Инициализация предмета
    public virtual void Initialize(ItemType type, bool interactable, IItemStateStrategy stateStrategy, IFrequencyStrategy freqStrategy)
    {
        _type = type;
        _interactable = interactable;
        _id = IdGenerator.GetNextId();
        _stateType = GetStateTypeFromStrategy(stateStrategy);
        _isDestroyed = false;
        CurrentStrategy = stateStrategy;
        frequencyStrategy = freqStrategy;
        _frequency = frequencyStrategy.GetCurrentFrequency();

        // Устанавливаем цвет
        _color = GetColorFromType(type);

        // Применяем цвет к материалу
        if (TryGetComponent(out Renderer renderer))
        {
            renderer.material.color = _color;
        }

        // Инициируем стратегию
        stateStrategy.OnStateEnter(this);

        if (_type == ItemType.Bluetooth || _type == ItemType.Wifi)
        {
            StartCoroutine(UpdateFrequencyRoutine());
        }
    }

    protected virtual IEnumerator UpdateFrequencyRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(15f);

            if (_isDestroyed)
                yield break;

            frequencyStrategy.UpdateFrequency();
            _frequency = frequencyStrategy.GetCurrentFrequency();
        }
    }

    // Метод для изменения состояния
    public virtual void ChangeState(IItemStateStrategy newStrategy)
    {
        CurrentStrategy.OnStateExit();
        CurrentStrategy = newStrategy;
        this._stateType = GetStateTypeFromStrategy(newStrategy);
        newStrategy.OnStateEnter(this);
    }

    // Передача данных
    public virtual void TransmitData()
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

        CurrentStrategy.TransmitData(data);
    }

    // Уничтожение предмета
    public virtual void DestroyItem()
    {
        _isDestroyed = true;
        IdGenerator.ReleaseId(_id);
        Debug.Log($"Предмет {_id} уничтожен");
        Destroy(gameObject);
    }

    // Вспомогательные методы
    protected virtual ItemStateType GetStateTypeFromStrategy(IItemStateStrategy strategy)
    {
        return strategy switch
        {
            ActiveStateStrategy _ => ItemStateType.Active,
            PeriodicStateStrategy _ => ItemStateType.Periodic,
            IRActiveStateStrategy _ => ItemStateType.Active, 
            IRPeriodicStateStrategy _ => ItemStateType.Periodic,
            _ => ItemStateType.Active
        }; ;
    }

    protected virtual Color GetColorFromType(ItemType type)
    {
        return type switch
        {
            ItemType.GSM => Color.green,
            ItemType.LTE=> Color.red,
            ItemType.Bluetooth => Color.blue,
            ItemType.Wifi=> new Color(1, 0.64f, 0), // Оранжевый
            ItemType.IRDevice => Color.magenta,
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