# Catálogo de requisitos

## Punto de partida
Surge del diagnóstico de la Actividad 1: falta de una vista única del estado de cada pedido, riesgo de pedidos
duplicados e inventario inconsistente, mala coordinación entre almacén, rutas y soporte, y dependencia de sistemas
externos (pagos, geolocalización, notificaciones, ERP o CRM). A eso se suma la necesidad central de esta actividad:
**la plataforma debe admitir ventanas de entrega y picos de demanda sin interrumpir a los clientes actuales.**

## Requisitos funcionales (lo que el sistema debe hacer)

| ID | Requisito |
|---|---|
| RF-01 | Registrar pedidos desde múltiples canales (web, app, tienda). |
| RF-02 | Validar la disponibilidad de inventario antes de confirmar un pedido. |
| RF-03 | Evitar pedidos duplicados mediante un identificador único. |
| RF-04 | Asignar pedidos a rutas y repartidores. |
| RF-05 | Gestionar ventanas de entrega (reservar, modificar y cancelar). |
| RF-06 | Rastrear el estado del pedido en tiempo real. |
| RF-07 | Notificar al cliente los cambios de estado. |
| RF-08 | Registrar evidencia de entrega y entregas fallidas. |
| RF-09 | Gestionar incidencias y reclamos desde soporte. |
| RF-10 | Sincronizar productos, clientes e inventario con el ERP o CRM. |
| RF-11 | Consultar el historial de un pedido. |

## Requisitos no funcionales (atributos de calidad)

| ID | Atributo | Requisito |
|---|---|---|
| RNF-01 | Rendimiento | Las consultas de estado responden rápido incluso con carga alta. |
| RNF-02 | Disponibilidad | El servicio sigue operando durante picos y despliegues. |
| RNF-03 | Escalabilidad | Se absorben aumentos de pedidos sin rediseñar el sistema. |
| RNF-04 | Seguridad | Datos personales protegidos y acceso por roles. |
| RNF-05 | Trazabilidad | Cada cambio de estado queda registrado con fecha y autor. |
| RNF-06 | Facilidad de evolución | Las reglas de negocio se cambian sin reescribir toda la aplicación. |
| RNF-07 | Mantenibilidad | Módulos con baja dependencia entre sí y alta cohesión interna. |
| RNF-08 | Interoperabilidad | Integración con sistemas externos mediante APIs con contratos definidos. |
| RNF-09 | Costo operativo | Infraestructura y equipo acordes a una operación en etapa inicial. |

## Restricciones
- Los pagos los procesa un sistema externo.
- El equipo de desarrollo es pequeño.
- Los clientes actuales no pueden sufrir interrupciones durante cambios o despliegues.
- La operación inicia en una sola ciudad o región.
