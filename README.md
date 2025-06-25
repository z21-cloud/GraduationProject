Дипломная работа - Разработка комплекса цифровых двойников ST-500 "Пиранья" и многофункционального бинокля ОБЛИК-2


## Общая схема взаимодействия

```mermaid
graph TD
    PlayerController --> Raycast
    PlayerController --> ItemContext
    ItemContext -- ЛКМ --> TransmitData
    ItemContext -- E --> DestroyItem
    TransmitData --> CurrentStrategy
    ItemSpawner --> ItemFactory
    ItemFactory --> ItemContext
    ItemFactory --> ItemType
    ItemFactory --> IItemStateStrategy
    IItemStateStrategy --> ActiveStateStrategy
    IItemStateStrategy --> PassiveStateStrategy
    IItemStateStrategy --> PeriodicStateStrategy
```

### ItemSpawner and ItemFactory
```mermaid
graph TD
    subgraph "ItemSpawner"
        IS[ItemSpawner]
    end
    
    subgraph "ItemFactory"
        IF[ItemFactory] --> RandomSettings["Устанавливает:
        - случайный ItemType
        - случайный IItemStateStrategy
        - случайный Interactable"]
    end
    
    subgraph "ItemContext"
        IC[ItemContext]
    end
    
    ISC[ItemSpawnConfig]
    
    IS -->|"берёт настройки"| ISC
    IS -->|"создаёт предметы"| IF
    IF -->|"Initialize(...)"| IC
    
    style IS fill:#ecf0f1,stroke:#2c3e50,stroke-width:2px
    style IF fill:#ecf0f1,stroke:#2c3e50,stroke-width:2px
    style IC fill:#d6eaf8,stroke:#2c3e50,stroke-width:2px
    style ISC fill:#ecf0f1,stroke:#2c3e50,stroke-width:2px
    style RandomSettings fill:#f8f9f9,stroke:#7f8c8d,stroke-width:1px,stroke-dasharray:4 2
```

### Детальное взаимодействие с PlayerController
```mermaid
graph LR
    subgraph "Player Input"
        PC[PlayerController]
    end
    
    subgraph "Object Detection"
        Ray[Raycast]
    end
    
    subgraph "Item Interaction"
        IC[ItemContext]
        CS[CurrentStrategy]
    end
    
    PC -->|"проверяет объекты"| Ray
    Ray -->|"если найден"| IC
    IC -->|"ЛКМ → TransmitData()"| CS
    IC -->|"E → DestroyItem()"| Destroy((Уничтожение))
    IC -.->|"обратная связь"| PC
    
    style PC fill:#ecf0f1,stroke:#2c3e50,stroke-width:2px
    style Ray fill:#ecf0f1,stroke:#2c3e50,stroke-width:2px
    style IC fill:#d6eaf8,stroke:#2c3e50,stroke-width:2px
    style CS fill:#ecf0f1,stroke:#2c3e50,stroke-width:2px
    style Destroy fill:#f5b7b1,stroke:#c0392b,stroke-width:1px
```

### Взаимодействие стратегий состояния
```mermaid
graph TD
    subgraph "ItemContext"
        IC[ItemContext]
        Management["Управляет:
        - изменением цвета
        - изменением состояния
        - уничтожением объекта"]
    end
    
    subgraph "State Strategies"
        IIS[IItemStateStrategy]
        ASS[ActiveStateStrategy]
        PSS[PassiveStateStrategy]
        PeSS[PeriodicStateStrategy]
        
        IIS --> ASS
        IIS --> PSS
        IIS --> PeSS
    end
    
    IC -->|"взаимодействует"| IIS
    ASS -->|"TransmitData()"| IC
    PeSS -->|"периодическое поведение"| IC
    
    style IC fill:#d6eaf8,stroke:#2c3e50,stroke-width:2px
    style Management fill:#f8f9f9,stroke:#7f8c8d,stroke-width:1px,stroke-dasharray:4 2
    style IIS fill:#ecf0f1,stroke:#2c3e50,stroke-width:2px
    style ASS fill:#ecf0f1,stroke:#2c3e50,stroke-width:2px
    style PSS fill:#ecf0f1,stroke:#2c3e50,stroke-width:2px
    style PeSS fill:#ecf0f1,stroke:#2c3e50,stroke-width:2px
```
