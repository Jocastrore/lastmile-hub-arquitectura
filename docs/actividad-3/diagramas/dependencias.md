# Dirección de las dependencias

Las flechas indican quién depende de quién. Domain no depende de nada; Application depende de Domain; Infrastructure implementa los puertos que define Application.

```mermaid
flowchart TB
    API["API o controladores (entrada)"]
    subgraph Application["Application"]
        UC["AssignCourierUseCase"]
        Puertos["Puertos: IDeliveryRepository, IRouteRepository, ICourierAvailabilityPort, IEventPublisher, IUnitOfWork"]
    end
    subgraph Domain["Domain"]
        Entidades["Delivery, Route, objetos de valor, invariantes"]
    end
    subgraph Infrastructure["Infrastructure"]
        Adaptadores["SqlDeliveryRepository, CourierShiftApiAdapter, QueueEventPublisher"]
    end
    API --> UC
    UC --> Entidades
    UC --> Puertos
    Adaptadores -->|"implementa"| Puertos
```
