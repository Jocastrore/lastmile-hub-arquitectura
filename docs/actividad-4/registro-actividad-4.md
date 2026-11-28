# Registro de la Actividad 4

## 1. Trazabilidad diseño (Actividad 3) -> implementación

| Elemento de diseño | Implementación |
|---|---|
| Entidad `Delivery` | `src/LastMileHub.Domain/Dispatch/Delivery.cs` |
| Entidad `Route` | `src/LastMileHub.Domain/Dispatch/Route.cs` |
| Objetos de valor | `ValueObjects.cs` |
| INV-01 (un solo repartidor) | `Delivery.AssignCourier`; PK `route_deliveries.delivery_id`; guarda en el `UPSERT` |
| INV-02 / INV-06 (solo listas para despacho) | `Delivery.AssignCourier` |
| INV-03 (capacidad de ruta) | `Route.AddDelivery`; conteo en `SqliteRouteRepository.SaveAsync` |
| INV-04 (repartidor disponible) | `ICourierAvailabilityPort` + `CourierShiftAdapter` |
| Caso de uso `AssignCourierUseCase` | `src/LastMileHub.Application/Dispatch/AssignCourierUseCase.cs` |
| Puertos | `Ports.cs` |
| Adaptadores | `src/LastMileHub.Infrastructure/Persistence/` |
| Escenarios Dado-Cuando-Entonces 1 a 6 | `AssignCourierApiTests.cs`, `AssignCourierUseCaseTests.cs`, `DomainTests.cs` |
| Evento `EntregaAsignada` | `DeliveryAssigned` guardado en `outbox_events` |
| ADR-002 | Verificado por `ArchitectureTests.cs` |

| Escenario de la Actividad 3 | Prueba |
|---|---|
| 1. Asignación exitosa | `Happy_path_assigns_persists_and_writes_outbox_event` |
| 2. Entrega ya asignada | `Duplicate_assignment_returns_409` |
| 3. Ruta saturada | `Saturated_route_returns_409_and_leaves_delivery_untouched` |
| 4. Entrega no asignable | `Cancelled_delivery_returns_409_not_assignable` |
| 5. Repartidor no disponible | `Courier_off_shift_returns_422` |
| 6. Entrega inexistente | `Unknown_delivery_returns_404` |
| Integración no disponible (límite) | `Availability_integration_down_returns_503_without_changing_state` |

## 2. Riesgos operativos y de seguridad

| # | Riesgo | Tipo | Mitigación | Evidencia de verificación |
|---|---|---|---|---|
| R1 | Asignación duplicada o simultánea de la misma entrega | Operativo | INV-01 en el dominio; `UPSERT` con guarda; PK única | `Duplicate_assignment_returns_409`, `AssignCourier_twice_throws_already_assigned` |
| R2 | Ruta que supera su capacidad por concurrencia | Operativo | Regla en `Route` y verificación de conteo en la transacción | `Saturated_route_returns_409...`, `AddDelivery_beyond_capacity_throws_saturated` |
| R3 | Caída del sistema de turnos | Operativo | Error controlado (503), sin cambios de estado, llamada fuera de la transacción | `Availability_integration_down_returns_503...` |
| R4 | Pérdida del evento tras cambiar el estado | Operativo | Outbox en la misma transacción | `Happy_path...` verifica la fila en `outbox_events` |
| R5 | Fuga de datos personales (dirección) en logs o respuestas | Seguridad | Logs solo con identificadores; `DeliveryResponse` sin dirección | Revisión de `AssignCourierUseCase` y `Dtos.cs` |
| R6 | Inyección SQL | Seguridad | Todas las consultas parametrizadas | Revisión de repositorios; sin concatenación de entradas |
| R7 | Inyección en logs vía `X-Correlation-Id` | Seguridad | Se valida `^[A-Za-z0-9-]{1,64}$`, si no cumple se genera uno | `Middleware.cs` |
| R8 | Fuga de detalles internos en errores 500 | Seguridad | Mensaje genérico; el detalle solo queda en el log | `Middleware.Map` |
| R9 | Acceso no autenticado a la API | Seguridad | **No mitigado en esta versión** (ver riesgos residuales) | n/a |
| R10 | Datos de demo en un entorno real | Operativo | Se cargan solo con `Seed:Demo=true` (apagado por defecto) | `appsettings.json` |
| R11 | Degradación de la arquitectura con el tiempo | Calidad | Fitness functions en CI | `ArchitectureTests.cs` |
| R12 | Dependencias vulnerables | Seguridad | `dotnet list package --vulnerable` en CI | `.github/workflows/ci.yml` |

## 3. Observabilidad

| Señal | Qué permite diagnosticar | Dónde |
|---|---|---|
| Log `Courier assigned` (Information) | Asignaciones exitosas con ids y duración | `AssignCourierUseCase` |
| Log `Assignment rejected` (Warning) con `ErrorCode` | Motivo de cada rechazo | `AssignCourierUseCase` |
| Log `Unhandled error` (Error) con `CorrelationId` | Fallos inesperados | `Middleware.cs` |
| `CorrelationId` en logs y cabecera de respuesta | Seguir una petición de punta a punta | `Middleware.cs` |
| Métrica `assignments_total{result}` | Tasa de éxito y de cada tipo de rechazo | `AssignmentMetrics` |
| Métrica `assignment_duration_ms` | Latencia del flujo | `AssignmentMetrics` |
| `GET /health` | Disponibilidad del servicio | `Program.cs` |
| Tabla `outbox_events` (`processed_at` nulo) | Eventos pendientes de publicar | Base de datos |

**Datos que no se registran:** dirección de entrega, nombres, teléfonos, cuerpo completo de las peticiones.

Alertas sugeridas: aumento de `COURIER_AVAILABILITY_UNAVAILABLE` (integración caída), aumento de `ROUTE_SATURATED`
(falta de capacidad) y eventos pendientes en outbox por más de N minutos.

## 4. Comprobación automatizada de arquitectura
`ArchitectureTests.cs` verifica: Domain sin referencias a otras capas ni a `Microsoft.*`; Application sin referencias
a Infrastructure, Api, `Microsoft.Data` ni `Microsoft.AspNetCore`; el caso de uso recibe solo interfaces por constructor.
Se ejecutan en cada `dotnet test` y en CI.

## 5. Resultados de las pruebas
_Completar después de ejecutar localmente (no copiar resultados que no hayas obtenido)._

| Fecha | Comando | Pruebas totales | Aprobadas | Fallidas | Evidencia |
|---|---|---|---|---|---|
| | `dotnet test` | | | | captura de pantalla o `TestResults/results.trx` |

Ejecución de la demo (`requests.http`): _(pegar los códigos HTTP obtenidos en cada escenario)_

## 6. Riesgos residuales
- Sin autenticación ni autorización: cualquiera puede llamar al endpoint (R9). En producción se exigiría identidad y rol de despacho.
- Sin proceso que consuma el outbox: las notificaciones no se envían todavía.
- SQLite no soporta alta concurrencia de escritura.
- Sin reintentos con espera ante fallos del sistema de turnos (hoy falla de forma controlada).
- El sistema de turnos está simulado con una tabla local.
- No se implementa reasignación ni desasignación.
- Las métricas no se exportan aún a un recolector (el medidor está listo para OpenTelemetry).

## 7. Próximos pasos
1. Autenticación (JWT) y autorización por rol.
2. Proceso de publicación del outbox hacia una cola y consumidor de notificaciones, con cola de mensajes fallidos.
3. Reintentos con espera exponencial y disyuntor para el sistema de turnos.
4. Exportar métricas y trazas con OpenTelemetry.
5. Migrar el adaptador de persistencia a un motor cliente-servidor (PostgreSQL o SQL Server).
6. Casos de uso de desasignación, reasignación e inicio de recorrido.
7. Análisis estático de seguridad (CodeQL) en CI.
