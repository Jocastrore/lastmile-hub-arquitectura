# LastMile Hub

## Resumen ejecutivo

**Producto.** LastMile Hub, plataforma centralizada de gestión logística de última milla para una empresa de retail digital.

**Problema.** La empresa recibe pedidos desde varios canales, maneja inventarios distribuidos y coordina entregas con
diferentes rutas y repartidores, pero no cuenta con una vista centralizada del estado real de cada pedido.
Esto genera duplicación de información, retrasos, errores operativos y poca trazabilidad. Además, la plataforma debe
admitir ventanas de entrega y picos de demanda sin interrumpir a los clientes actuales.

**Usuarios.** Cliente, operador de comercio o tienda, administrador de almacén, repartidor y soporte al cliente.

**Alcance.** Registro de pedidos, validación de inventario, asignación de rutas y repartidores, rastreo de entregas,
notificaciones e incidencias. Quedan fuera el procesamiento de pagos, la optimización de rutas con inteligencia
artificial, la facturación electrónica y las compras a proveedores.

## Requisitos prioritarios

Se priorizaron según su importancia para el negocio y su impacto en la arquitectura. Los siguientes son los más
significativos (prioridad alta e impacto alto en la arquitectura):

| ID | Requisito | Por qué importa |
|---|---|---|
| RF-01 | Registrar pedidos desde múltiples canales | Es la entrada de todo el negocio |
| RF-02 | Validar inventario antes de confirmar un pedido | Evita prometer lo que no hay |
| RF-05 | Gestionar ventanas de entrega | Es una necesidad explícita del planteamiento |
| RF-06 | Rastrear el estado del pedido en tiempo real | Resuelve la falta de vista única |
| RF-10 | Sincronizar con el ERP o CRM | El ERP es la fuente maestra de productos y stock |
| RNF-02 | Disponibilidad | No se puede interrumpir a los clientes actuales |
| RNF-03 | Escalabilidad | Los picos de demanda no pueden degradar el servicio |
| RNF-04 | Seguridad | Hay datos personales de clientes |
| RNF-06 | Facilidad de evolución | Las reglas de negocio cambiarán |

## Decisión arquitectónica

**Monolito modular con dominios bien separados**: una sola aplicación desplegable, dividida internamente en módulos
(Pedidos, Inventario, Despacho y Rutas, Notificaciones e Incidencias), cada uno con su propia lógica y sin acceder
a los datos de los demás.

**Por qué.** Frente a microservicios, reduce el riesgo y el costo para un equipo pequeño y una etapa inicial,
mantiene la consistencia entre pedidos e inventario (clave para evitar duplicados e inconsistencias) y permite
cubrir los picos con varias réplicas, colas para tareas asíncronas y despliegues progresivos, sin interrumpir a los
clientes actuales.

**Trade-offs.** Escalar implica replicar toda la aplicación y no solo el módulo saturado; además exige disciplina
para que los módulos no se acoplen.

**Cuándo reconsiderarla.** Si los picos superan de forma sostenida lo que se logra con réplicas, si un módulo
necesita escalar o cambiar a un ritmo muy distinto, o si el equipo crece y necesita desplegar módulos por separado,
se extraería ese módulo a un servicio.
