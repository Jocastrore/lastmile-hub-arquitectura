# ADR-002: Dominio protegido con puertos y adaptadores

- **Estado:** Aceptada
- **Fecha:** 26 de noviembre de 2026
- **Relacionada con:** ADR-001 (monolito modular)

## Contexto
El flujo "asignar repartidor" contiene reglas críticas (un solo repartidor, capacidad de ruta, estados válidos,
disponibilidad) que no deben depender de la base de datos, de APIs externas ni de un framework. Además, el
sistema es un monolito modular con varios contextos que se comunican entre sí.

## Decisiones
1. Las invariantes viven en las entidades (`Delivery`, `Route`), no en controladores ni en repositorios.
2. Cada módulo se organiza en tres capas (Domain, Application, Infrastructure) con las dependencias hacia adentro.
3. El caso de uso depende de puertos (interfaces) y recibe sus dependencias por constructor.
4. Los módulos se comunican mediante eventos con contratos explícitos.
5. Geolocalización y ERP se integran con una capa anticorrupción.
6. `Delivery` y `Route` son agregados separados; la asignación los modifica en una misma unidad de trabajo.
7. La disponibilidad del repartidor (INV-04) se valida en el caso de uso mediante un puerto, porque depende de un
   sistema externo que el dominio no debe conocer.

## Alternativas descartadas

| Alternativa | Motivo del descarte |
|---|---|
| Reglas en servicios que usan directamente la base de datos | Acopla negocio e infraestructura y dificulta las pruebas |
| Un único agregado "Ruta" que contenga todas sus entregas | Cargaría demasiados datos en cada operación y complicaría la consistencia en los picos |
| Modelo anémico (entidades solo con datos) | Las reglas quedarían dispersas y se romperían las invariantes |
| Comunicación entre módulos por consulta directa a tablas ajenas | Rompe los límites de contexto |
| Una sola capa o dos capas | No separa las reglas del negocio de los casos de uso ni de la tecnología |

## Por qué tres capas
Cada capa cambia por una razón distinta: las reglas del negocio (Domain), los casos de uso y su coordinación
(Application) y la tecnología (Infrastructure). Separarlas permite cambiar de base de datos o de proveedor sin
tocar las reglas.

## Trade-offs
- (+) El dominio se prueba con pruebas unitarias sin infraestructura.
- (+) Se puede cambiar la base de datos o un proveedor sin tocar el negocio.
- (-) Más interfaces y más código de enlace.
- (-) Modificar dos agregados en una misma unidad de trabajo exige cuidado; si el sistema crece, se evaluaría consistencia eventual entre ambos.
- (-) Requiere disciplina del equipo para no filtrar detalles de infraestructura hacia el dominio.
