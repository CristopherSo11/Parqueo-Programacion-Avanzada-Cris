-- 1. Insertar 5 automoviles
INSERT INTO prq_automovil
    (id, color, anio, fabricante, tipo)
VALUES
    (1, 'Rojo', 2020, 'Toyota', 'sedán'),
    (2, 'Negro', 2022, 'Honda', 'moto'),
    (3, 'Blanco', 2023, 'Hyundai', '4x4'),
    (4, 'Azul', 2019, 'Nissan', 'sedán'),
    (5, 'Gris', 2024, 'Suzuki', 'moto');

-- 2. Insertar 2 parqueos
INSERT INTO prq_parqueo
    (id, nombre_provincia, nombre, precio_por_hora)
VALUES
    (1, 'Cartago', 'Parqueo Central Cartago', 1000.00),
    (2, 'San José', 'Parqueo Avenida Central', 1500.00);

-- 3. Insertar 15 ingresos de automoviles
-- NULL en la salida indica que el vehiculo sigue estacionado

INSERT INTO prq_ingreso_automovil
    (
        consecutivo,
        id_parqueo,
        id_automovil,
        fecha_hora_entrada,
        fecha_hora_salida
    )
VALUES
    (1, 1, 1, '2026-10-01 08:00:00', '2026-10-01 10:30:00'),
    (2, 1, 2, '2026-10-01 09:15:00', '2026-10-01 11:15:00'),
    (3, 2, 3, '2026-10-02 07:30:00', '2026-10-02 12:00:00'),
    (4, 2, 4, '2026-10-02 10:00:00', '2026-10-02 11:45:00'),
    (5, 1, 5, '2026-10-03 13:00:00', '2026-10-03 15:30:00'),
    (6, 1, 3, '2026-10-04 08:20:00', '2026-10-04 09:50:00'),
    (7, 2, 1, '2026-10-05 14:00:00', '2026-10-05 17:00:00'),
    (8, 2, 2, '2026-10-06 09:00:00', '2026-10-06 10:30:00'),
    (9, 1, 4, '2026-10-07 11:15:00', '2026-10-07 14:45:00'),
    (10, 2, 5, '2026-10-08 15:00:00', '2026-10-08 16:15:00'),
    (11, 1, 1, '2026-10-20 08:00:00', NULL),
    (12, 2, 2, '2026-10-20 09:30:00', NULL),
    (13, 1, 3, '2026-10-21 10:00:00', NULL),
    (14, 2, 4, '2026-10-21 12:15:00', NULL),
    (15, 1, 5, '2026-10-22 07:45:00', NULL);

-- 4. Ajustar las secuencias de los identificadores
-- para que los siguientes INSERT generen IDs nuevos.

SELECT setval(
    pg_get_serial_sequence('prq_automovil', 'id'),
    (SELECT MAX(id) FROM prq_automovil)
);

SELECT setval(
    pg_get_serial_sequence('prq_parqueo', 'id'),
    (SELECT MAX(id) FROM prq_parqueo)
);

SELECT setval(
    pg_get_serial_sequence('prq_ingreso_automovil', 'consecutivo'),
    (SELECT MAX(consecutivo) FROM prq_ingreso_automovil)
);

-- 5. Consultas de verificacion

SELECT COUNT(*) AS total_automoviles
FROM prq_automovil;

SELECT COUNT(*) AS total_parqueos
FROM prq_parqueo;

SELECT COUNT(*) AS total_ingresos
FROM prq_ingreso_automovil;

SELECT COUNT(*) AS automoviles_sin_salida
FROM prq_ingreso_automovil
WHERE fecha_hora_salida IS NULL;
