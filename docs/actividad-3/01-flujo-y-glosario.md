# Flujo de negocio elegido y glosario

## Retoma de las actividades 1 y 2
- **Problema (Actividad 1):** sin una vista única del estado de cada pedido hay duplicados, retrasos, errores y poca trazabilidad.
- **Decisión (Actividad 2):** monolito modular con dominios separados (ADR-001). Este diseño detalla el módulo **Despacho y Rutas**.

## Flujo elegido: asignar un repartidor a una entrega

| Aspecto | Descripción |
|---|---|
| Actores | Operador de despacho (o el sistema de asignación automática), repartidor, almacén (deja el pedido listo) y cliente (recibe la notificación) |
| Propósito | Garantizar que cada pedido confirmado y listo para despacho tenga un único repartidor responsable, dentro de una ruta con capacidad disponible |
| Resultado esperado | La entrega queda en estado *Asignada*, vinculada a un repartidor y a una ruta, y se publica el evento `EntregaAsignada` para notificar al cliente y al repartidor |

Se eligió este flujo porque concentra varias reglas críticas del negocio (un solo repartidor, capacidad de ruta,
estados válidos, disponibilidad) y conecta con tres contextos: Pedidos, Despacho y Notificaciones.

## Glosario de lenguaje ubicuo

| Término | Definición | Ambigüedad a confirmar |
|---|---|---|
| Pedido | Solicitud de compra del cliente, con líneas de producto | ¿Un pedido puede dividirse en varias entregas? Se asume 1 pedido = 1 entrega en esta versión |
| Entrega | Unidad de trabajo logístico que lleva un pedido al cliente | Igual que arriba |
| Repartidor | Persona que realiza entregas | ¿Propio o tercero? Se asume que ambos se tratan igual |
| Ruta | Conjunto de entregas asignadas a un repartidor en una jornada | ¿Quién la crea: el sistema o el operador? |
| Capacidad de ruta | Máximo de entregas que admite una ruta | ¿Se mide por cantidad, peso o tiempo? Se asume por cantidad |
| Saturada | Ruta que alcanzó su capacidad máxima | Sin ambigüedad una vez definida la capacidad |
| Ventana de entrega | Rango horario en que el cliente espera recibir el pedido | ¿Es un compromiso firme o un estimado? |
| Asignar | Vincular una entrega con un repartidor y una ruta | ¿Es reversible? Se asume que sí (desasignar) |
| Reasignar | Cambiar el repartidor de una entrega ya asignada | Caso de uso aparte, fuera del alcance de esta actividad |
| Disponible (repartidor) | Activo, en turno y con cupo en su ruta | ¿Cómo se informa el turno? |
| Entrega fallida | Intento de entrega que no se completó (cliente ausente, dirección errónea) | ¿Cuántos reintentos se permiten? |
| Evidencia de entrega | Firma, foto o código que prueba la entrega | Fuera del flujo elegido |
