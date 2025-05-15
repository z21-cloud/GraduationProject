
## Схема взаимодействия

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
