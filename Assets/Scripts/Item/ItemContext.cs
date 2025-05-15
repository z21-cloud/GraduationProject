using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ItemContext : MonoBehaviour
{
    // ����, ������������ � ����������
    [SerializeField] private ItemType _type;
    [SerializeField] private bool _interactable;
    [SerializeField] private float _frequency;
    [SerializeField] private int _id;
    [SerializeField] private Color _color;
    [SerializeField] private ItemStateType _stateType;
    [SerializeField] private bool _isDestroyed;

    // �������� (��� ����������� �������������)
    public ItemType Type => _type;
    public bool Interactable => _interactable;
    public float Frequency => _frequency;
    public int ID => _id;
    public Color Color => _color;
    public ItemStateType StateType => _stateType;
    public bool IsDestroyed => _isDestroyed;

    // ��������� (���������� ���������)
    private IItemStateStrategy currentStrategy;

    // ����������� ������� ID
    private static int idCounter = 1;

    // ������������� ��������
    public void Initialize(ItemType type, bool interactable, IItemStateStrategy strategy)
    {
        this._type = type;
        this._interactable = interactable;
        this._frequency = UnityEngine.Random.Range(1f, 1000f);
        this._id = idCounter++;
        this._stateType = GetStateTypeFromStrategy(strategy);
        this._isDestroyed = false;
        this.currentStrategy = strategy;

        // ������������� ����
        this._color = GetColorFromType(type);

        // ��������� ���� � ���������
        if (TryGetComponent(out Renderer renderer))
        {
            renderer.material.color = _color;
        }

        // ���������� ���������
        strategy.OnStateEnter(this);
    }

    // ����� ��� ��������� ���������
    public void ChangeState(IItemStateStrategy newStrategy)
    {
        currentStrategy.OnStateExit();
        currentStrategy = newStrategy;
        this._stateType = GetStateTypeFromStrategy(newStrategy);
        newStrategy.OnStateEnter(this);
    }

    // �������� ������
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

    // ����������� ��������
    public void DestroyItem()
    {
        _isDestroyed = true;
        Debug.Log($"������� {_id} ���������");
        Destroy(gameObject);
    }

    // ��������������� ������
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
            ItemType.Neutral => new Color(1, 0.64f, 0), // ���������
            _ => Color.white
        };
    }

    // ��� ���������� ����� � ���������
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
            Debug.LogWarning($"� ������� {name} ��� Renderer. ���� �� ����� ������� � ���������.");
        }
    }
}