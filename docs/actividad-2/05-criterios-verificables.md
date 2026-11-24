# Criterios verificables

Criterios para comprobar si la decisión responde a las necesidades del sistema.

| Criterio | Escenario | Cómo se verifica | Umbral de aceptación |
|---|---|---|---|
| Consulta de estado rápida | EC-01 | Prueba de carga con herramientas como k6 o JMeter | Percentil 95 menor a 2 s |
| Absorber picos de demanda | EC-02 | Prueba de carga simulando 5 veces el volumen normal | 0 pedidos perdidos; degradación menor al 20% |
| Despliegue sin interrupción | EC-03 | Despliegue progresivo con monitoreo de errores durante el proceso | 0 minutos de interrupción |
| Tolerancia a fallas externas | EC-04 | Simular la caída del servicio de notificaciones | Pedidos siguen entrando; notificaciones se reenvían |
| Control de acceso | EC-05 | Prueba de seguridad con un usuario de rol repartidor | 100% de intentos denegados y registrados |
| Protección de datos | EC-06 | Revisión de configuración de cifrado y prueba de seguridad | Datos sensibles cifrados |
| Cambios aislados | EC-07 | Revisión de dependencias entre módulos tras un cambio de regla | El cambio no toca otros módulos |
| Un solo pedido por solicitud | EC-08 | Prueba automatizada enviando la misma solicitud dos veces | 1 pedido creado |
| Historial completo | EC-09 | Prueba funcional con el rol de soporte | Historial visible en menos de 5 s |
| Límites entre módulos | RNF-06, RNF-07 | Prueba automatizada de arquitectura que falla si un módulo depende de otro de forma no permitida | 0 dependencias no permitidas |

Estas verificaciones se implementan en las actividades siguientes. Aquí se definen para que la decisión pueda
comprobarse con evidencia y no solo con argumentos.
