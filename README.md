
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

##ItemSpawner and ItemFactory
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
