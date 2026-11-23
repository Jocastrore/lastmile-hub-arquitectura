# B. Diagrama de contexto

Muestra al usuario, el sistema principal, los sistemas externos, los flujos de información y los límites del sistema.

```mermaid
flowchart LR
    Cliente(["Cliente"])
    Tienda(["Operador de comercio o tienda"])
    Almacen(["Administrador de almacén"])
    Repartidor(["Repartidor"])
    Soporte(["Soporte al cliente"])

    subgraph Limite["Límite del sistema"]
        Sistema["LastMile Hub<br/>Plataforma de gestión logística de última milla"]
    end

    Pagos[("Sistema de pagos")]
    Geo[("Servicio de geolocalización")]
    Notif[("Sistema de notificaciones")]
    ERP[("ERP o CRM interno")]

    Cliente -->|"Crea pedido, consulta estado"| Sistema
    Sistema -->|"Estado, ETA, confirmaciones"| Cliente
    Tienda -->|"Registra pedidos de canal físico"| Sistema
    Almacen -->|"Actualiza stock, prepara despacho"| Sistema
    Sistema -->|"Órdenes de preparación"| Almacen
    Sistema -->|"Ruta y pedidos asignados"| Repartidor
    Repartidor -->|"Estado de entrega, evidencias"| Sistema
    Soporte -->|"Consulta pedido, registra incidencia"| Sistema
    Sistema -->|"Validar y confirmar pago"| Pagos
    Pagos -->|"Resultado del pago"| Sistema
    Sistema -->|"Coordenadas, cálculo de rutas"| Geo
    Geo -->|"Distancias, tiempos, ubicación"| Sistema
    Sistema -->|"Eventos a notificar"| Notif
    Notif -->|"SMS, push o email al cliente"| Cliente
    Sistema -->|"Sincroniza clientes, productos, stock"| ERP
    ERP -->|"Catálogo, inventario maestro"| Sistema
```

## Elementos

| Elemento | Descripción |
|---|---|
| Actores | Cliente, operador de comercio o tienda, administrador de almacén, repartidor, soporte al cliente |
| Sistema principal | LastMile Hub, dentro del recuadro "Límite del sistema" |
| Sistemas externos | Pagos, geolocalización, notificaciones, ERP o CRM |
| Flujos de información | Flechas con su etiqueta |
| Límites | Todo lo que está dentro del recuadro se construye; lo que está fuera se integra por interfaces |