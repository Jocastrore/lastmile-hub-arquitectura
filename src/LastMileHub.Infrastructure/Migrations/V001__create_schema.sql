CREATE TABLE deliveries (
    id         TEXT PRIMARY KEY,
    status     TEXT NOT NULL,
    courier_id TEXT NULL,
    route_id   TEXT NULL,
    address    TEXT NOT NULL
);

CREATE TABLE routes (
    id       TEXT PRIMARY KEY,
    capacity INTEGER NOT NULL CHECK (capacity > 0)
);

-- delivery_id es PK: una entrega solo puede estar en una ruta (refuerza INV-01).
CREATE TABLE route_deliveries (
    route_id    TEXT NOT NULL,
    delivery_id TEXT NOT NULL PRIMARY KEY
);
CREATE INDEX ix_route_deliveries_route ON route_deliveries(route_id);

-- Simula el sistema externo de turnos.
CREATE TABLE courier_shifts (
    courier_id TEXT PRIMARY KEY,
    on_shift   INTEGER NOT NULL
);

-- Outbox: eventos pendientes de publicar (una cola real los consumiría).
CREATE TABLE outbox_events (
    id           TEXT PRIMARY KEY,
    type         TEXT NOT NULL,
    payload      TEXT NOT NULL,
    occurred_at  TEXT NOT NULL,
    processed_at TEXT NULL
);
