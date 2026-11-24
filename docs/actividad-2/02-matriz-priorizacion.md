# Matriz de priorización

**Escala:** Alta, Media, Baja.
**Prioridad:** qué tan importante es para el negocio hoy.
**Impacto en arquitectura:** cuánto condiciona la estructura del sistema si se equivoca la decisión.
**Complejidad:** esfuerzo estimado para cumplirlo.
**Significativo:** "Sí" cuando la prioridad es Alta **y** el impacto en arquitectura es Alto.

| Requisito | Prioridad | Impacto en negocio | Impacto en arquitectura | Complejidad | Significativo |
|---|---|---|---|---|---|
| RF-01 Registrar pedidos multicanal | Alta | Alta | Alta | Media | Sí |
| RF-02 Validar inventario | Alta | Alta | Alta | Alta | Sí |
| RF-03 Evitar duplicados | Alta | Alta | Media | Media | No |
| RF-04 Asignar rutas y repartidores | Alta | Alta | Media | Alta | No |
| RF-05 Ventanas de entrega | Alta | Alta | Alta | Media | Sí |
| RF-06 Rastreo en tiempo real | Alta | Alta | Alta | Alta | Sí |
| RF-07 Notificaciones | Media | Media | Baja | Baja | No |
| RF-08 Evidencia y entregas fallidas | Media | Media | Baja | Media | No |
| RF-09 Gestionar incidencias | Media | Media | Baja | Media | No |
| RF-10 Sincronizar con ERP o CRM | Alta | Alta | Alta | Alta | Sí |
| RF-11 Historial del pedido | Media | Media | Baja | Baja | No |
| RNF-01 Rendimiento | Media | Alta | Media | Media | No |
| RNF-02 Disponibilidad | Alta | Alta | Alta | Alta | Sí |
| RNF-03 Escalabilidad | Alta | Alta | Alta | Alta | Sí |
| RNF-04 Seguridad | Alta | Alta | Alta | Media | Sí |
| RNF-05 Trazabilidad | Alta | Alta | Media | Media | No |
| RNF-06 Facilidad de evolución | Alta | Media | Alta | Media | Sí |
| RNF-07 Mantenibilidad | Media | Media | Alta | Media | No |
| RNF-08 Interoperabilidad | Media | Alta | Alta | Media | No |
| RNF-09 Costo operativo | Media | Alta | Media | Baja | No |

## Requisitos arquitectónicamente significativos
RF-01, RF-02, RF-05, RF-06, RF-10, RNF-02, RNF-03, RNF-04 y RNF-06.

Son los que más condicionan la estructura: las integraciones y el inventario piden consistencia; la
disponibilidad, la escalabilidad y las ventanas de entrega piden soportar picos sin interrupciones; y la
evolución y la seguridad piden límites claros entre las partes del sistema.
