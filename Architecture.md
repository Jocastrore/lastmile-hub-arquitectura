# Architecture - LastMile Hub

## 1. Contexto
Plataforma de gestión logística de última milla para una empresa de retail digital. Ver el diagrama C4 nivel 1
en `docs/actividad-3/diagramas/c4.md`.

## 2. Estilo arquitectónico
Monolito modular con dominios separados (ADR-001). Dentro de cada módulo, el dominio es el centro y las
dependencias apuntan hacia él (ADR-002).

## 3. Módulos (bounded contexts)
Pedidos, Inventario, Despacho y Rutas (núcleo), Notificaciones e Incidencias. Se comunican mediante eventos con
contratos explícitos; ningún módulo accede a los datos de otro. Ver `docs/actividad-3/diagramas/mapa-de-contextos.md`.

## 4. Capas dentro de cada módulo

| Capa | Contenido | Regla |
|---|---|---|
| Domain | Entidades, objetos de valor, invariantes | No depende de nada |
| Application | Casos de uso, contratos y puertos | Depende solo de Domain |
| Infrastructure | Adaptadores (SQL, colas, APIs externas) | Implementa los puertos de Application |

Estructura del código: `src/LastMileHub.Domain`, `src/LastMileHub.Application` y, en la siguiente actividad,
`src/LastMileHub.Infrastructure`. Las pruebas están en `tests/`.

## 5. Flujo diseñado: asignar repartidor
Detallado en `docs/actividad-3/` (glosario, modelo, invariantes, escenarios, contextos, casos de uso, puertos y adaptadores).

## 6. Reglas críticas (invariantes)
INV-01 a INV-06, protegidas en las entidades `Delivery` y `Route` y, para la disponibilidad, en el caso de uso
mediante un puerto.

## 7. Integraciones
Geolocalización y ERP detrás de adaptadores con capa anticorrupción. Los eventos hacia Notificaciones e
Incidencias salen por un puerto de publicación.

## 8. Decisiones
- ADR-001: monolito modular con dominios separados.
- ADR-002: dominio protegido con puertos y adaptadores.

## 9. Diagramas
C4 niveles 1 a 3, diagrama de estados, mapa de contextos y dirección de dependencias, todos en
`docs/actividad-3/diagramas/`.
