-- Datos SOLO para la demo. Se aplican con Seed__Demo=true.
INSERT OR IGNORE INTO courier_shifts VALUES ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 1);
INSERT OR IGNORE INTO courier_shifts VALUES ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 0);

INSERT OR IGNORE INTO routes VALUES ('cccccccc-cccc-cccc-cccc-cccccccccccc', 2);
INSERT OR IGNORE INTO routes VALUES ('dddddddd-dddd-dddd-dddd-dddddddddddd', 1);

INSERT OR IGNORE INTO deliveries VALUES ('11111111-1111-1111-1111-111111111111', 'ReadyForDispatch', NULL, NULL, 'Calle 10 # 20-30');
INSERT OR IGNORE INTO deliveries VALUES ('22222222-2222-2222-2222-222222222222', 'ReadyForDispatch', NULL, NULL, 'Carrera 43A # 5-15');
INSERT OR IGNORE INTO deliveries VALUES ('33333333-3333-3333-3333-333333333333', 'ReadyForDispatch', NULL, NULL, 'Avenida 80 # 30-40');
INSERT OR IGNORE INTO deliveries VALUES ('44444444-4444-4444-4444-444444444444', 'Cancelled', NULL, NULL, 'Calle 50 # 45-10');
INSERT OR IGNORE INTO deliveries VALUES ('55555555-5555-5555-5555-555555555555', 'Assigned', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'Calle 33 # 70-20');

INSERT OR IGNORE INTO route_deliveries VALUES ('dddddddd-dddd-dddd-dddd-dddddddddddd', '55555555-5555-5555-5555-555555555555');
