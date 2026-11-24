# Comparación de alternativas estructurales

## Alternativas

**Opción A: monolito modular con límites claros.**
Una sola unidad desplegable, dividida por dominio en módulos: Pedidos, Inventario, Despacho y Rutas,
Notificaciones e Incidencias. Cada módulo expone una interfaz y no accede a los datos de otro. Se escala con
varias réplicas detrás de un balanceador y una cola para tareas asíncronas.

**Opción B: microservicios.**
Un servicio independiente por dominio, con su propia base de datos, comunicados por APIs y mensajería.

## Criterios y puntajes

Escala de 1 a 5, donde 5 es lo mejor para el criterio (por ejemplo, 5 en costo significa el más barato).

| Criterio | A. Monolito modular | B. Microservicios |
|---|---|---|
| Costo inicial | 5 | 2 |
| Complejidad | 4 | 2 |
| Rapidez de implementación | 5 | 2 |
| Escalabilidad ante picos | 3 (réplicas de toda la aplicación) | 5 (por servicio) |
| Facilidad de evolución | 4 (si se respetan los límites) | 4 |
| Riesgo operativo | 4 | 2 |
| Disponibilidad | 4 | 4 |
| Alineación con el negocio en esta etapa | 5 | 3 |
| **Total** | **34** | **24** |

## Ventajas y desventajas

| | A. Monolito modular | B. Microservicios |
|---|---|---|
| Ventajas | Simple de desplegar y probar. Mantiene la consistencia transaccional entre pedidos e inventario. Menor costo. Un equipo pequeño puede mantenerlo. | Escala cada dominio por separado. Las fallas quedan aisladas. Despliegues independientes. |
| Desventajas | Escalar implica replicar todo. Riesgo de que los módulos se acoplen si no hay disciplina. Un despliegue afecta a toda la aplicación. | Alta complejidad operativa. La consistencia distribuida es difícil (inventario frente a pedidos). Más infraestructura y coordinación. Puede ser un sobrediseño. |

## Costos y riesgos

| | A. Monolito modular | B. Microservicios |
|---|---|---|
| Costo | Bajo: una aplicación, una base de datos, un despliegue | Alto: varios servicios, mensajería, observabilidad distribuida, más infraestructura |
| Riesgo principal | Acoplamiento entre módulos con el tiempo | Fallas en cascada y datos inconsistentes entre servicios |
| Cómo se mitiga | Pruebas de arquitectura y revisión de dependencias | Requiere prácticas maduras que el equipo aún no tiene |

## Alternativa descartada sin puntuar
**Arquitectura por capas tradicional.** Es simple, pero organiza el código por capas técnicas y no por dominios,
lo que dificulta cambiar una regla de negocio sin tocar todo el sistema (EC-07). No cumple bien el requisito de
facilidad de evolución.

## Conclusión
Los criterios decisivos fueron el **riesgo operativo**, el **costo** y la **consistencia** entre pedidos e
inventario. La alternativa A los favorece en esta etapa. Su debilidad (escalar todo junto) se compensa con
réplicas y colas para los picos y se revisa con criterios de reconsideración (ver ADR-001).
