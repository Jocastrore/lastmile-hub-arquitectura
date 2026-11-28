# ADR-003: Persistencia SQLite, outbox y observabilidad mínima

- **Estado:** Aceptada
- **Fecha:** 27 de noviembre de 2026
- **Relacionadas:** ADR-001 (monolito modular), ADR-002 (dominio, puertos y adaptadores)

## Contexto
La Actividad 3 propuso `SqlDeliveryRepository`, `QueueEventPublisher` y `CourierShiftApiAdapter` como
adaptadores posibles. La Actividad 4 exige un flujo ejecutable que se pueda correr y probar sin infraestructura
externa, manteniendo los límites del diseño.

## Decisiones (y ajustes frente a la Actividad 3)
| # | Decisión | Ajuste respecto a la Actividad 3 |
|---|---|---|
| 1 | Persistencia en **SQLite** con SQL explícito (Microsoft.Data.Sqlite) tras `IDeliveryRepository` / `IRouteRepository`. | Antes se mencionaba SQL Server. Cambiar de motor solo afecta a Infrastructure. |
| 2 | **Outbox**: el evento `DeliveryAssigned` se guarda en `outbox_events` en la misma transacción que el cambio. | Antes `QueueEventPublisher`. Un proceso posterior publicaría a una cola real. |
| 3 | `IUnitOfWork` implementado por `SqliteSession` (una conexión y transacción por petición). | Se concreta el puerto de transacción previsto. |
| 4 | El caso de uso llama al sistema de turnos **antes** de la transacción. | Evita mantener transacciones abiertas durante llamadas externas. |
| 5 | Migraciones SQL versionadas, aplicadas por `MigrationRunner` y registradas en `schema_version`. | Nuevo: persistencia reproducible. |
| 6 | Concurrencia: PK única en `route_deliveries.delivery_id`, `UPSERT` con guarda en `deliveries` y conteo de capacidad dentro de la transacción. | Refuerzo en base de datos de INV-01 e INV-03. |
| 7 | Errores de negocio e integración se traducen en la API a `ProblemDetails` con un `code` estable. | Nuevo: contrato de errores. |
| 8 | Observabilidad: logs estructurados con `CorrelationId`, métricas con `System.Diagnostics.Metrics`, `/health`. | Nuevo. |
| 9 | Fitness functions con pruebas de reflexión (sin dependencias extra). | Nuevo. |

## Alternativas descartadas
| Alternativa | Motivo |
|---|---|
| Entity Framework Core | Más magia y dependencias; el SQL explícito hace visible la frontera repositorio-dominio en la defensa. |
| Cola real (RabbitMQ, Service Bus) | Exige infraestructura que no aporta a validar las reglas; el outbox deja la puerta abierta. |
| Dead Letter Queue, process manager, event sourcing | No aplican a un solo flujo síncrono; se justifican cuando haya consumidores asíncronos. |
| API Gateway | El sistema es un monolito modular con una sola entrada. |
| Publicar el evento directamente tras guardar | Puede perder el evento si el proceso cae entre ambos pasos. |

## Trade-offs
- (+) Todo corre con `dotnet test` y `dotnet run`, sin servicios externos.
- (+) Estado y evento se guardan atómicamente.
- (-) SQLite admite pocas escrituras concurrentes; para producción se cambiaría el adaptador.
- (-) El outbox no tiene aún proceso que lo consuma.
- (-) El sistema de turnos se simula con una tabla local.
- (-) `Delivery` y `Route` se modifican en la misma transacción; si el sistema crece, se evaluaría consistencia eventual.

## Criterios para reconsiderar
- Volumen de escrituras concurrentes superior a lo que soporta SQLite.
- Aparición de consumidores asíncronos reales (notificaciones, incidencias).
- Necesidad de desplegar el módulo de forma independiente.
