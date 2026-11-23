# D. Supuestos y decisiones iniciales

## Qué asumo
- Los canales de venta ya existen y pueden enviar pedidos por API o por integración.
- El ERP o CRM interno es la fuente maestra de productos y clientes.
- Los pagos los procesa un sistema externo y la plataforma solo consulta su resultado.
- Los repartidores tienen un teléfono inteligente con datos móviles y GPS.
- La operación inicia en una sola ciudad o región.

## Qué decisiones ya tomo
- Se construirá una plataforma centralizada con una vista única del pedido.
- No se construirá una pasarela de pagos ni un servicio de mapas propio: se integran servicios externos.
- El diseño debe ser abierto a integraciones, porque hay varios sistemas externos.
- El primer alcance es registrar pedidos, validar inventario, asignar rutas, rastrear entregas y gestionar incidencias.

## Qué aún no sé
- Cuántos pedidos diarios habrá y qué tan grandes serán los picos de demanda.
- Cuántos canales de venta hay y qué capacidad de integración tiene cada uno.
- Si el inventario se actualiza en tiempo real o por lotes.
- Si la flota de reparto es propia, de terceros o mixta.

## Qué habrá que confirmar luego
- Requisitos no funcionales: disponibilidad, tiempos de respuesta, seguridad y protección de datos personales.
- Qué APIs ofrece realmente el ERP o CRM.
- Las reglas de negocio para asignar rutas y para reintentar entregas fallidas.
- Presupuesto y tecnología (se decide en la Actividad 2, no aquí).

## Decisión inicial
Se propone una plataforma centralizada que permita registrar pedidos, validar inventario, asignar rutas,
rastrear entregas y gestionar incidencias desde un único sistema.

**Por qué es coherente con el negocio.** El problema central es la falta de una vista única y confiable del
estado de cada pedido. Un sistema centralizado ataca esa causa directamente, reduce la duplicación y los
errores, y da trazabilidad sin obligar a decidir todavía tecnologías ni estructuras internas.
