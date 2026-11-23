# Riesgos principales

| # | Riesgo | Impacto | Mitigación inicial |
|---|---|---|---|
| 1 | Duplicación de pedidos | Doble despacho y costos innecesarios | Identificador único por pedido y control de duplicados al recibirlo |
| 2 | Inconsistencias de inventario | Promesas de entrega que no se cumplen | Inventario centralizado y validación antes de confirmar |
| 3 | Entregas fallidas o retrasadas | Mala experiencia y reprocesos | Seguimiento de estados y alertas |
| 4 | Pérdida de trazabilidad | No se sabe dónde está el pedido | Registro de cada cambio de estado con fecha y autor |
| 5 | Mala coordinación entre almacén, rutas y soporte | Errores operativos | Vista compartida con permisos por rol |
| 6 | Crecimiento operativo sin control | Sistema que no escala | Definir métricas y límites desde el inicio |
| 7 | Dependencia de sistemas externos (pagos, GPS, notificaciones) | Fallas en cascada | Tolerancia a fallos y reintentos |
