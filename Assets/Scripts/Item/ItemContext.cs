using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

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
    public IItemStateStrategy CurrentStrategy { get; private set; }

    // ��������� (���������� ���������)
    private Coroutine frequencyCoroutine;
    private IFrequencyStrategy frequencyStrategy;

    // ����������� ������� ID
    private static int idCounter = 1;

    // ������������� ��������
    public void Initialize(ItemType type, bool interactable, IItemStateStrategy stateStrategy, IFrequencyStrategy freqStrategy)
    {
        _type = type;
        _interactable = interactable;
        _id = idCounter++;
        _stateType = GetStateTypeFromStrategy(stateStrategy);
        _isDestroyed = false;
        CurrentStrategy = stateStrategy;
        frequencyStrategy = freqStrategy;
        _frequency = frequencyStrategy.GetCurrentFrequency();

        // ������������� ����
        _color = GetColorFromType(type);

        // ��������� ���� � ���������
        if (TryGetComponent(out Renderer renderer))
        {
            renderer.material.color = _color;
        }

        // ���������� ���������
        stateStrategy.OnStateEnter(this);

        if (_type == ItemType.Bluetooth || _type == ItemType.Wifi)
        {
            frequencyCoroutine = StartCoroutine(UpdateFrequencyRoutine());
        }
    }

    private IEnumerator UpdateFrequencyRoutine()
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

    // ����� ��� ��������� ���������
    public void ChangeState(IItemStateStrategy newStrategy)
    {
        CurrentStrategy.OnStateExit();
        CurrentStrategy = newStrategy;
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

        CurrentStrategy.TransmitData(data);
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
            PeriodicStateStrategy _ => ItemStateType.Periodic,
            _ => ItemStateType.Active
        };
    }

    private Color GetColorFromType(ItemType type)
    {
        return type switch
        {
            ItemType.GSM => Color.green,
            ItemType.LTE=> Color.red,
            ItemType.Bluetooth => Color.blue,
            ItemType.Wifi=> new Color(1, 0.64f, 0), // ���������
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