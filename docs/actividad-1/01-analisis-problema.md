# C. Análisis del problema

## ¿Qué ocurre hoy?
Los pedidos llegan por varios canales (web, app, tienda, redes) y cada canal guarda su propia información.
El inventario está repartido en varios puntos y las rutas se coordinan sin una vista compartida entre
almacén, repartidores y soporte. No existe una fuente única que diga en qué estado está cada pedido.

## ¿Por qué es un problema?
- Nadie sabe con certeza dónde está un pedido ni en qué estado.
- Se duplican pedidos y se promete stock que no existe.
- Soporte tiene que preguntar a varias áreas para responder una consulta simple.
- Los errores se detectan tarde, cuando el cliente ya reclama.

## ¿Qué consecuencias tiene?

| Para quién | Consecuencia |
|---|---|
| La empresa | Retrasos, entregas fallidas, costos operativos altos, devoluciones y pérdida de ventas |
| Los usuarios (clientes) | Mala experiencia, desconfianza y reclamos |
| La operación | Crecimiento difícil de controlar y decisiones sin datos confiables |

## ¿Qué variables importan?
- Estado del pedido y sus cambios en el tiempo.
- Disponibilidad de inventario por ubicación o almacén.
- Capacidad de los repartidores y de cada ruta.
- Tiempos y distancias de entrega.
- Ventanas de entrega acordadas con el cliente.
- Volumen de pedidos y picos de demanda.
- Calidad de la dirección del cliente.
- Tiempo de respuesta de los sistemas externos (pagos, geolocalización, notificaciones).
