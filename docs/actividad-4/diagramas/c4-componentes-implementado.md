# Diagrama C4 nivel 3 actualizado: módulo Despacho y Rutas implementado

Actualiza el diagrama de componentes de la Actividad 3 con los nombres reales de la implementación.
Las líneas punteadas son los puertos definidos en Application que implementan los adaptadores.

```mermaid
flowchart TB
    Cliente["Cliente HTTP: operador de despacho"]

    subgraph Despacho["Módulo Despacho y Rutas (implementado)"]
        Ep["DeliveryEndpoints (Api)"]
        Mw["Middleware: correlación y mapeo de errores (Api)"]
        UC["AssignCourierUseCase (Application)"]
        Dom["Delivery y Route (Domain)"]
        RepoD["SqliteDeliveryRepository (adaptador)"]
        RepoR["SqliteRouteRepository (adaptador)"]
        Disp["CourierShiftAdapter (adaptador)"]
        Pub["OutboxEventPublisher (adaptador)"]
        Ses["SqliteSession: unidad de trabajo (adaptador)"]
    end

    BD["Base de datos SQLite"]
    Turnos["Sistema de turnos (simulado con una tabla)"]
    Outbox["Tabla outbox_events"]

    Cliente -->|"HTTP"| Mw
    Mw --> Ep
    Ep --> UC
    UC --> Dom
    UC -.->|"puerto"| RepoD
    UC -.->|"puerto"| RepoR
    UC -.->|"puerto"| Disp
    UC -.->|"puerto"| Pub
    UC -.->|"puerto"| Ses
    RepoD --> Ses
    RepoR --> Ses
    Pub --> Ses
    Ses --> BD
    Disp --> Turnos
    Pub --> Outbox
```

## Cambios frente al diseño de la Actividad 3

| Diseño (Actividad 3) | Implementación (Actividad 4) | Motivo |
|---|---|---|
| `SqlDeliveryRepository` | `SqliteDeliveryRepository` | Se eligió SQLite para poder ejecutar todo sin infraestructura externa (ADR-003) |
| `SqlRouteRepository` | `SqliteRouteRepository` | Igual |
| `QueueEventPublisher` | `OutboxEventPublisher` | El evento se guarda en una tabla outbox dentro de la misma transacción |
| `SqlUnitOfWork` | `SqliteSession` | Una conexión y transacción por petición |
| `CourierShiftApiAdapter` | `CourierShiftAdapter` | El sistema de turnos se simula con una tabla local |
| (no existía) | `Middleware` | Correlación de peticiones y traducción de errores a respuestas controladas |
