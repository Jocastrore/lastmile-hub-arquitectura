# ADR-001: Monolito modular con dominios separados como estructura inicial

- **Estado:** Aceptada
- **Fecha:** 24 de noviembre de 2026
- **Contexto de la decisión:** Actividad 2, requisitos y decisión estructural

## Contexto
LastMile Hub debe unificar pedidos, inventario, rutas, entregas e incidencias. La plataforma debe admitir
ventanas de entrega y picos de demanda sin interrumpir a los clientes actuales. El equipo es pequeño, el volumen
real aún no se conoce con precisión y existen integraciones con pagos, geolocalización, notificaciones y ERP o CRM.

## Requisitos que impulsan la decisión
RF-01, RF-02, RF-05, RF-06, RF-10, RNF-02 (disponibilidad), RNF-03 (escalabilidad), RNF-04 (seguridad) y
RNF-06 (facilidad de evolución). Ver escenarios EC-02, EC-03, EC-07 y EC-08.

## Opciones consideradas
1. Monolito modular con límites claros.
2. Microservicios.
3. Arquitectura por capas tradicional (descartada: no separa dominios).

## Decisión
Se adopta un **monolito modular** organizado por dominios (Pedidos, Inventario, Despacho y Rutas,
Notificaciones, Incidencias e Integraciones), con:

- comunicación entre módulos solo mediante interfaces públicas,
- una base de datos con esquemas separados por módulo,
- tareas asíncronas (notificaciones, sincronización con el ERP) mediante una cola,
- varias réplicas de la aplicación detrás de un balanceador,
- integraciones externas aisladas detrás de adaptadores.

## Justificación
- Mantiene la consistencia transaccional entre pedidos e inventario, lo que evita duplicados e inconsistencias
  (riesgos identificados en la Actividad 1).
- Menor costo y riesgo operativo para un equipo pequeño y una etapa inicial (puntaje 34 frente a 24 en la
  comparación de alternativas).
- Los límites de dominio permiten extraer módulos a servicios más adelante si hay evidencia de necesidad.
- Con réplicas, cola y despliegues progresivos se cubren los picos y se evita interrumpir a los clientes actuales.

## Consecuencias y trade-offs
- (+) Simplicidad, rapidez de entrega, menor costo y facilidad de prueba.
- (-) Escalar implica replicar toda la aplicación, no solo el módulo saturado.
- (-) Requiere disciplina para no acoplar los módulos.
- (-) Un fallo grave en un módulo puede afectar al conjunto; se mitiga con aislamiento de integraciones y manejo de errores.

## Riesgos y mitigaciones

| Riesgo | Mitigación |
|---|---|
| Módulos acoplados con el tiempo | Pruebas de arquitectura y revisión de dependencias |
| Un pico supera la capacidad de las réplicas | Pruebas de carga y autoescalado |
| El módulo de rutas o de rastreo necesita escalar aparte | Candidato a extraerse como servicio |
| Caída de un sistema externo | Cola, reintentos y degradación controlada |

## Criterios para reconsiderar la decisión
- Los picos exceden de forma sostenida la capacidad que se logra escalando réplicas.
- Un módulo requiere un ritmo de cambio o una escala muy distintos al resto.
- El equipo crece y necesita desplegar módulos de forma independiente.
