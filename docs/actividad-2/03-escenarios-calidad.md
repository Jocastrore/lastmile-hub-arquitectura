# Escenarios de calidad

Cada escenario describe un estímulo, el entorno en que ocurre, la respuesta esperada y una medida concreta.

> Las cifras son supuestos de diseño razonables para esta etapa. Se validarán con datos reales del negocio
> (ver "Qué aún no sé" en la Actividad 1).

| ID | Atributo | Estímulo y entorno | Respuesta esperada | Medida |
|---|---|---|---|---|
| EC-01 | Rendimiento (RNF-01) | Un cliente consulta el estado de su pedido con carga normal | El sistema devuelve el estado actual | Menos de 2 segundos (percentil 95) |
| EC-02 | Escalabilidad (RNF-03) | Una campaña multiplica por 5 los pedidos por hora | Se procesan todos los pedidos sin perder ninguno | 0 pedidos perdidos; degradación del tiempo de respuesta menor al 20% |
| EC-03 | Disponibilidad (RNF-02) | Se despliega una nueva versión en horario operativo | Los clientes no perciben interrupciones | 0 minutos de interrupción; disponibilidad mensual de al menos 99,5% |
| EC-04 | Disponibilidad (RNF-02) | Falla el servicio externo de notificaciones | Los pedidos siguen registrándose y las notificaciones se reintentan | 0 pedidos rechazados por esa falla; notificaciones enviadas al recuperarse |
| EC-05 | Seguridad (RNF-04) | Un repartidor intenta ver pedidos que no tiene asignados | Se deniega el acceso y se registra el intento | 100% de los intentos denegados y registrados |
| EC-06 | Seguridad (RNF-04) | Se almacenan datos personales del cliente | Los datos sensibles están protegidos | Cifrados en tránsito y en reposo |
| EC-07 | Evolución (RNF-06) | El negocio cambia la regla de asignación de rutas | Solo se modifica el módulo de Despacho y Rutas | Cambio en menos de 3 días de trabajo, sin tocar otros módulos |
| EC-08 | Consistencia (RF-03) | Llegan dos solicitudes idénticas del mismo pedido | Se crea un solo pedido | 1 pedido por identificador único |
| EC-09 | Trazabilidad (RNF-05) | Soporte investiga un pedido retrasado | Se muestra todo el historial de estados | Historial completo visible en menos de 5 segundos |
