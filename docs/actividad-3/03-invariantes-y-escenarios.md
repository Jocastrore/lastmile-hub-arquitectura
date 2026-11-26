# Invariantes y escenarios de aceptación

## Invariantes (reglas que siempre deben cumplirse)

| ID | Invariante | Quién la protege |
|---|---|---|
| INV-01 | Una entrega solo tiene un repartidor activo a la vez | `Delivery` |
| INV-02 | Solo se asigna una entrega en estado *ListaParaDespacho* | `Delivery` |
| INV-03 | Una ruta no puede superar su capacidad máxima | `Route` |
| INV-04 | Solo se asigna a un repartidor disponible (activo y en turno) | Caso de uso, mediante un puerto |
| INV-05 | Una entrega no puede estar en dos estados a la vez ni hacer transiciones no permitidas | `Delivery` |
| INV-06 | Una entrega cancelada o entregada no puede asignarse | `Delivery` |

## Escenarios Dado-Cuando-Entonces

**Escenario 1: asignación exitosa**
- **Dado** una entrega en estado *ListaParaDespacho*, un repartidor disponible y una ruta con cupo
- **Cuando** el operador asigna el repartidor a la entrega
- **Entonces** la entrega pasa a *Asignada*, queda vinculada al repartidor y a la ruta, y se publica `EntregaAsignada`.

**Escenario 2: entrega ya asignada (INV-01)**
- **Dado** una entrega en estado *Asignada* con el repartidor A
- **Cuando** se intenta asignar al repartidor B
- **Entonces** el sistema rechaza con "La entrega ya tiene un repartidor" y no cambia nada.

**Escenario 3: ruta saturada (INV-03)**
- **Dado** una ruta con 20 de 20 entregas y una entrega lista para despacho
- **Cuando** se intenta asignar la entrega a esa ruta
- **Entonces** el sistema rechaza con "Ruta saturada" y la entrega permanece en *ListaParaDespacho*.

**Escenario 4: entrega no asignable (INV-02, INV-06)**
- **Dado** una entrega en estado *Cancelada* o *Pendiente*
- **Cuando** se intenta asignar un repartidor
- **Entonces** el sistema rechaza con "La entrega no está lista para despacho".

**Escenario 5: repartidor no disponible (INV-04)**
- **Dado** un repartidor fuera de turno
- **Cuando** se intenta asignarle una entrega
- **Entonces** el sistema rechaza con "Repartidor no disponible" y no guarda nada ni publica eventos.

**Escenario 6: entrega inexistente**
- **Dado** un identificador que no corresponde a ninguna entrega
- **Cuando** se ejecuta la asignación
- **Entonces** el sistema responde "Entrega no encontrada".

## Trazabilidad: escenario, invariante y prueba

| Escenario | Invariante | Prueba automatizada (carpeta `tests`) |
|---|---|---|
| 1. Asignación exitosa | todas | `AssignCourierUseCaseTests.Success_saves_both_aggregates_and_publishes_event` y `DeliveryTests.AssignCourier_when_ready_moves_to_assigned` |
| 2. Entrega ya asignada | INV-01 | `DeliveryTests.AssignCourier_twice_throws_already_assigned` |
| 3. Ruta saturada | INV-03 | `RouteTests.AddDelivery_beyond_capacity_throws_saturated` y `AssignCourierUseCaseTests.Saturated_route_is_rejected_and_nothing_is_saved` |
| 4. Entrega no asignable | INV-02, INV-06 | `DeliveryTests.AssignCourier_when_not_ready_throws_not_assignable` |
| 5. Repartidor no disponible | INV-04 | `AssignCourierUseCaseTests.Unavailable_courier_is_rejected_without_side_effects` |
| 6. Entrega inexistente | n/a | `AssignCourierUseCaseTests.Unknown_delivery_throws_not_found` |
