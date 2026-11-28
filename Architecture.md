# Architecture - LastMile Hub

## 1. Contexto
Plataforma de gestión logística de última milla para una empresa de retail digital. Ver el diagrama C4 nivel 1
en `docs/actividad-3/diagramas/c4.md`.

## 2. Estilo arquitectónico
Monolito modular con dominios separados (ADR-001). Dentro de cada módulo, el dominio es el centro y las
dependencias apuntan hacia él (ADR-002). La Actividad 4 implementa el módulo **Despacho y Rutas**.

## 3. Módulos (bounded contexts)
Pedidos, Inventario, **Despacho y Rutas (implementado)**, Notificaciones e Incidencias. Se comunican mediante
eventos con contratos explícitos; ningún módulo accede a los datos de otro. Ver
`docs/actividad-3/diagramas/mapa-de-contextos.md`.

## 4. Capas del módulo Despacho

| Capa | Proyecto | Contenido | Regla |
|---|---|---|---|
| Domain | `src/LastMileHub.Domain` | `Delivery`, `Route`, objetos de valor, excepciones de dominio | No depende de nada |
| Application | `src/LastMileHub.Application` | `AssignCourierUseCase`, puertos, errores de aplicación, métricas | Solo depende de Domain |
| Infrastructure | `src/LastMileHub.Infrastructure` | Repositorios SQLite, outbox, adaptador de turnos, migraciones | Implementa los puertos de Application |
| Api | `src/LastMileHub.Api` | Endpoints, DTOs, correlación y mapeo de errores | Compone las dependencias |

## 5. Flujo implementado: asignar repartidor
`POST /api/deliveries/{id}/assignment` llama al caso de uso, que carga la entrega y la ruta, consulta la
disponibilidad del repartidor mediante un puerto, aplica las invariantes en `Delivery` y `Route`, y guarda ambos
agregados y el evento en una sola transacción (outbox).

## 6. Reglas críticas
INV-01 a INV-06 protegidas en las entidades. INV-04 (disponibilidad) se verifica mediante un puerto. Refuerzos en
la base de datos: `delivery_id` único en `route_deliveries`, guarda de concurrencia en el `UPSERT` de
`deliveries` y verificación de capacidad dentro de la transacción.

## 7. Integraciones
El sistema de turnos está detrás de `ICourierAvailabilityPort` (adaptador `CourierShiftAdapter`). Cualquier fallo
se traduce a `CourierAvailabilityUnavailableException` (HTTP 503) sin cambiar el estado de la entrega. Los eventos
hacia Notificaciones salen por el outbox.

## 8. Observabilidad
Logs estructurados con `CorrelationId`, métricas `assignments_total` y `assignment_duration_ms` (medidor
`LastMileHub.Dispatch`) y endpoint `/health`. No se registran direcciones ni datos personales.

## 9. Comprobaciones automatizadas
`ArchitectureTests.cs` valida en cada ejecución que Domain no dependa de otras capas, que Application solo dependa
de Domain y que el caso de uso reciba únicamente abstracciones. El flujo de CI (`.github/workflows/ci.yml`) ejecuta
las pruebas y revisa vulnerabilidades en dependencias.

## 10. Decisiones
- ADR-001: monolito modular con dominios separados.
- ADR-002: dominio protegido con puertos y adaptadores.
- ADR-003: persistencia SQLite, outbox y observabilidad mínima.

## 11. Diagramas
C4 niveles 1 a 3 (Actividad 3), diagrama de estados, mapa de contextos y dirección de dependencias en
`docs/actividad-3/diagramas/`. El diagrama C4 nivel 3 con los componentes implementados está en
`docs/actividad-4/diagramas/c4-componentes-implementado.md`.

## 12. Trazabilidad diseño a implementación
Ver la sección 1 de `docs/actividad-4/registro-actividad-4.md`.
