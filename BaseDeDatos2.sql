-- ==========================================
-- OBLIGATORIO - BASE DE DATOS II
-- ==========================================

USE master;
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'ProyectoApicultura1')
BEGIN
    ALTER DATABASE ProyectoApicultura1 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ProyectoApicultura1;
END
GO
CREATE DATABASE ProyectoApicultura1;
GO
USE ProyectoApicultura1;
GO

-- CREATE TABLES

CREATE TABLE Apiario (
    id_apiario INT PRIMARY KEY,
    departamento VARCHAR(50) NOT NULL,
    seccional_policial VARCHAR(50),
    fecha_registro DATE NOT NULL,
    latitud DECIMAL(10, 8),
    longitud DECIMAL(11, 8)
);

CREATE TABLE Colmena (
    id_colmena INT PRIMARY KEY,
    id_apiario INT NOT NULL REFERENCES Apiario(id_apiario),
    tipo_caja VARCHAR(50),
    estado_colmena VARCHAR(50) CHECK (estado_colmena IN ('Fuerte', 'Media', 'Debil', 'Critica')),
    reina_estado VARCHAR(50) CHECK (reina_estado IN ('Sana', 'Joven', 'Vieja', 'Ausente', 'Reemplazada')),
    activa BIT DEFAULT 1,
    observaciones VARCHAR(500)
);

CREATE TABLE Inspeccion (
    id_inspeccion INT PRIMARY KEY,
    id_colmena INT NOT NULL REFERENCES Colmena(id_colmena),
    fecha_hora DATETIME NOT NULL,
    estado_colmena VARCHAR(50),
    estado_reina VARCHAR(50),
    poblacion_estimada INT CHECK (poblacion_estimada >= 0),
    observaciones VARCHAR(500)
);

CREATE TABLE Cosecha (
    id_cosecha INT PRIMARY KEY,
    fecha DATE NOT NULL,
    kg_total DECIMAL(10, 2) NOT NULL,
    lote VARCHAR(50) NOT NULL,
    observaciones VARCHAR(500)
);

CREATE TABLE Barril_exportacion (
    id_barril INT PRIMARY KEY,
    id_cosecha INT NOT NULL REFERENCES Cosecha(id_cosecha),
    peso_kg DECIMAL(6, 2) NOT NULL CHECK (peso_kg > 0),
    estado VARCHAR(50) CHECK (estado IN ('Listo', 'Exportado', 'Reservado')),
    destino VARCHAR(100)
);

CREATE TABLE Alimentacion (
    id_alimentation INT PRIMARY KEY,
    id_apiario INT NOT NULL REFERENCES Apiario(id_apiario),
    tipo_alimento VARCHAR(100) NOT NULL,
    fecha_programada DATE NOT NULL,
    cantidad DECIMAL(8, 2) CHECK (cantidad > 0)
);

CREATE TABLE Tratam_sanitarios (
    idTratamiento INT PRIMARY KEY,
    medicamento VARCHAR(100) NOT NULL,
    dosis VARCHAR(50),
    fech_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL,
    duracion INT CHECK (duracion > 0),
    CHECK (fecha_fin IS NULL OR fecha_fin >= fech_inicio)
);

CREATE TABLE Cosecha_Colmena (
    id_cosecha INT NOT NULL REFERENCES Cosecha(id_cosecha),
    id_colmena INT NOT NULL REFERENCES Colmena(id_colmena),
    PRIMARY KEY (id_cosecha, id_colmena)
);

CREATE TABLE Colmena_Tratamiento (
    id_colmena INT NOT NULL REFERENCES Colmena(id_colmena),
    idTratamiento INT NOT NULL REFERENCES Tratam_sanitarios(idTratamiento),
    fec_aplicacion DATE NOT NULL,
    PRIMARY KEY (id_colmena, idTratamiento, fec_aplicacion)
);


-- INDICES


CREATE INDEX IX_Apiario_Ubicacion 
    ON Apiario (departamento, seccional_policial);

CREATE INDEX IX_Cosecha_Fecha 
    ON Cosecha (fecha);

CREATE INDEX IX_Inspeccion_Colmena_Fecha 
    ON Inspeccion (id_colmena, fecha_hora);

CREATE INDEX IX_Barril_Estado 
    ON Barril_exportacion (estado);


-- JUEGO DE DATOS DE PRUEBA FUNCIONALES


INSERT INTO Apiario (id_apiario, departamento, seccional_policial, fecha_registro, latitud, longitud) VALUES 
(1, 'Colonia', '5ta Seccional', '2025-04-10', -34.46260000, -57.84000000),
(2, 'San José', '3ra Seccional', '2024-08-15', -34.33672100, -56.71453200),
(3, 'Canelones', '1ra Seccional', '2024-10-01', -34.52289100, -56.01874500),
(4, 'Flores',  '2da Seccional', '2025-01-20', -33.52801200, -56.87543200);

INSERT INTO Colmena (id_colmena, id_apiario, tipo_caja, estado_colmena, reina_estado, activa, observaciones) VALUES 
(101, 1, 'Langstroth', 'Media', 'Vieja', 1, 'Requiere cambio de reina pronto'),
(102, 1, 'Langstroth', 'Fuerte', 'Joven', 1, 'Colmena productiva, excelente postura'),
(103, 2, 'Langstroth', 'Media', 'Sana', 1, 'Estado normal, bajo monitoreo'),
(104, 2, 'Cuadro', 'Debil', 'Vieja', 1, 'Requiere refuerzo de población'),
(105, 3, 'Langstroth', 'Fuerte', 'Joven', 1, 'Nueva colmena temporada 2025'),
(106, 3, 'Langstroth', 'Critica', 'Ausente', 0, 'Sin reina, posible enjambrazón'),
(107, 4, 'Langstroth', 'Fuerte', 'Sana', 1, 'Apiario Norte, buena florada cercana');

INSERT INTO Inspeccion (id_inspeccion, id_colmena, fecha_hora, estado_colmena, estado_reina, poblacion_estimada, observaciones) VALUES
(501, 101, '2026-05-10 09:30:00', 'Fuerte', 'Sana', 45000, 'Buena postura de huevos, sin signos de varroa'),
(502, 102, '2026-05-10 10:00:00', 'Fuerte', 'Joven', 55000, 'Excelente estado, cuadros llenos de miel'),
(503, 103, '2026-05-11 08:30:00', 'Media', 'Sana', 32000, 'Estado normal'),
(504, 106, '2026-05-11 09:15:00', 'Critica', 'Ausente', 8000, 'Sin reina detectada'),
(505, 107, '2026-05-12 07:45:00', 'Fuerte', 'Sana', 48000, 'Muy buena actividad');

INSERT INTO Cosecha (id_cosecha, fecha, kg_total, lote, observaciones) VALUES 
(301, '2026-05-20', 250.50, 'LOTE-2026-A', 'Miel de pradera con excelente densidad'),
(302, '2025-11-15', 380.00, 'LOTE-2025-B', 'Cosecha de primavera, miel multifloral'),
(303, '2026-04-10', 420.50, 'LOTE-2026-B', 'Mejor cosecha histórica del apiario 1'),
(304, '2026-05-25', 195.00, 'LOTE-2026-C', 'Cosecha parcial apiario 3');

INSERT INTO Barril_exportacion (id_barril, id_cosecha, peso_kg, estado, destino) VALUES 
(401, 301, 120.00, 'Listo', 'Argentina'),
(402, 301, 130.50, 'Listo', 'Argentina'),
(403, 302, 295.00, 'Exportado', 'Alemania'),
(404, 302, 85.00, 'Reservado', 'Alemania'),
(405, 303, 300.00, 'Listo', 'Estados Unidos'),
(406, 303, 120.50, 'Listo', 'JapOn');

INSERT INTO Alimentacion (id_alimentation, id_apiario, tipo_alimento, fecha_programada, cantidad) VALUES 
(701, 1, 'Jarabe', '2026-06-01', 5.00),
(702, 1, 'Jarabe de azúcar', '2026-06-15', 8.00),
(703, 2, 'Candi', '2026-06-10', 4.50),
(704, 3, 'Polen sustituto', '2026-06-20', 3.00);

INSERT INTO Tratam_sanitarios (idTratamiento, medicamento, dosis, fech_inicio, fecha_fin, duracion) VALUES 
(801, 'Timol', '50ml por colmena', '2026-05-01', '2026-05-15', 14),
(802, 'Apivar', '2 tiras por colmena', '2026-04-01', '2026-05-27', 56),
(803, 'Oxabiol', '5ml por colmena', '2026-05-15', '2026-05-18', 3),
(804, 'Api-Bioxal', '3g por colmena', '2026-05-20', '2026-05-21', 1);

INSERT INTO Cosecha_Colmena (id_cosecha, id_colmena) VALUES 
(301, 101),
(302, 101),
(302, 102),
(302, 103),
(303, 101),
(303, 102),
(303, 105),
(304, 105),
(304, 106);

INSERT INTO Colmena_Tratamiento (id_colmena, idTratamiento, fec_aplicacion) VALUES 
(101, 801, '2026-05-01'),
(101, 802, '2026-04-01'),
(102, 802, '2026-04-01'),
(103, 803, '2026-05-15'),
(104, 803, '2026-05-15'),
(105, 804, '2026-05-20'),
(106, 804, '2026-05-20'),
(107, 801, '2026-05-01');


-- JUEGO DE DATOS QUE DEBEN SER RECHAZADOS POR RESTRICCIONES O INCOHERENCIAS

/*
-- A: Rechazo por FK (Clave foranea) Id de apiario inexistente
INSERT INTO Colmena (id_colmena, id_apiario, tipo_caja, estado_colmena, reina_estado, activa) 
VALUES (108, 99, 'Langstroth', 'Fuerte', 'Sana', 1);

-- B: Rechazo por restriccion CHECK de estado_colmena (el valor 'excelente' no esta presente ni incertado)
INSERT INTO Colmena (id_colmena, id_apiario, tipo_caja, estado_colmena, reina_estado, activa) 
VALUES (109, 1, 'Langstroth', 'Excelente', 'Sana', 1);

-- C: Rechazo por restriccion CHECK de peso. Peso de barril no puede ser cero o negativo
INSERT INTO Barril_exportacion (id_barril, id_cosecha, peso_kg, estado, destino) 
VALUES (407, 301, -5.00, 'Listo', 'Paraguay');

-- D: Rechazo por restricción CHECK de fechas discrepantes (fecha fin no podra ser anterior a fecha inicio)
INSERT INTO Tratam_sanitarios (idTratamiento, medicamento, dosis, fech_inicio, fecha_fin, duracion) 
VALUES (805, 'Ácido Cítrico', '20ml', '2026-06-15', '2026-06-10', 5);
*/


-- CONSULTAS REALIZADAS


-- 1: Ver cuantas colmenas hay en cada departamento
SELECT departamento, COUNT(*) AS total_colmenas
FROM Apiario
JOIN Colmena ON Apiario.id_apiario = Colmena.id_apiario
GROUP BY departamento;

-- 2: Listar los lotes de cosecha que superaron los 200 kilos
SELECT lote, fecha, kg_total 
FROM Cosecha
WHERE kg_total > 200.00
ORDER BY kg_total DESC;

-- 3: Ver las colmenas que estan graves y sus observaciones
SELECT id_colmena, id_apiario, estado_colmena, observaciones 
FROM Colmena
WHERE estado_colmena IN ('Debil', 'Critica');

-- 4: Saber la cantidad de kilos de miel que se han enviado o estan preparada para el pais de su destino
SELECT destino, SUM(peso_kg) AS total_kilos_destino
FROM Barril_exportacion
WHERE estado IN ('Exportado', 'Listo')
GROUP BY destino;

-- 5: Mostrar el historial de inspecciones de las colmenas, ordenado desde las mas recientes
SELECT id_colmena, fecha_hora, estado_colmena, estado_reina, poblacion_estimada
FROM Inspeccion
ORDER BY fecha_hora DESC;

-- 6: Listar los medicamentos que se le han aplicado a cada colmena y en su respectiva fecha
SELECT id_colmena, medicamento, fec_aplicacion
FROM Colmena_Tratamiento
JOIN Tratam_sanitarios ON Colmena_Tratamiento.idTratamiento = Tratam_sanitarios.idTratamiento;

-- 7: Identificar colmenas que nunca han sido inspeccionadas
SELECT c.id_colmena, c.id_apiario, c.tipo_caja, c.estado_colmena
FROM Colmena c
LEFT JOIN Inspeccion i ON c.id_colmena = i.id_colmena
WHERE i.id_inspeccion IS NULL;

-- 8: Encontrar colmenas que actualmente se marquen como inactivas pero que tienen anotaciones previas
SELECT id_colmena, id_apiario, observaciones 
FROM Colmena
WHERE activa = 0 AND observaciones IS NOT NULL;

---Procedimientos y Funciones realizadas

--1
GO
Create procedure HistorialInspecciones
@IdColmena int
as
begin

select i.fecha_hora,i.estado_colmena,i.estado_reina,i.poblacion_estimada,i.observaciones
from Colmena c
inner join Inspeccion i on c.id_colmena = i.id_colmena
where c.id_colmena = @IdColmena;

END
GO
EXEC HistorialInspecciones 101;
GO

--2
Create procedure CantidadColmenasApiario
@IdApiario int, @Activas int output, @Inactivas int output
as
begin

select @Activas = count(id_colmena)
from Colmena
where id_apiario = @IdApiario
and activa = 1;

select @Inactivas = count(id_colmena)
from Colmena 
where id_apiario = @IdApiario
and activa = 0;
end
GO

declare @Activas int
declare @Inactivas int
exec CantidadColmenasApiario
@IdApiario = 1,
@Activas = @Activas output,
@Inactivas = @Inactivas output

select @Activas as Activas,
@Inactivas as Inactivas
GO

--3
create procedure ProduccionApiario
@IdApiario int, @TotalKg decimal(10,2) output
as
begin

select @TotalKg = sum(c.kg_total)
from Colmena co 
inner join Cosecha_Colmena cc on co.id_colmena = cc.id_colmena
inner join Cosecha c on cc.id_cosecha = c.id_cosecha
where co.id_apiario = @IdApiario;
end
GO

declare @Kg decimal(10,2)
exec ProduccionApiario
1,
@Kg output
select @Kg
GO

--4
create procedure InspeccionesPorEstado
@Estado varchar(50)
as
begin

select id_inspeccion, id_colmena,fecha_hora,estado_colmena,estado_reina,poblacion_estimada,observaciones
from Inspeccion
where estado_colmena = @Estado;
end
GO


--5 Funcion 1
Create function CantidadInspecciones
(
@IdColmena int
)
returns int 
as
begin
declare @Cantidad int

select @Cantidad = count(id_inspeccion)
from Inspeccion
where id_colmena = @IdColmena
return @Cantidad;
end
GO

--6 Funcion 2
create function CantidadTratamientos
(
@IdColmena int
)
returns int
as
begin
declare @Cantidad int

select @Cantidad = count(idTratamiento)
from Colmena_Tratamiento
where id_colmena = @IdColmena
return @Cantidad;
end
GO

--7 Funcion 3
create function KilosCosecha
(
@IdCosecha int
)
returns decimal(10,2)
as 
begin
declare @Kg decimal(10,2)

select @Kg = kg_total
from Cosecha
where id_cosecha = @IdCosecha
return @Kg
end
GO

--8 Funcion 4
create function EstadoColmena
(
@IdColmena int
)
returns varchar(30)
as
begin
declare @Estado varchar(50)
select @Estado = estado_colmena
from Colmena
where id_colmena = @IdColmena
return @Estado
end
GO

--Triggers
create trigger Tr_ActualizarEstadoColmena
on Inspeccion
after Insert
as
begin

update Colmena
set estado_colmena = i.estado_colmena,
reina_estado = i.estado_reina

from Colmena c
inner join inserted i
on c.id_colmena = i.id_colmena
end
GO

--2
create trigger TR_ColmenaCritica
on Colmena
after update 
as
begin
if UPDATE(estado_colmena)
begin
print 'La colmena cambio a estado critica'
end
end
GO

--3
create trigger TR_PoblacionBaja
on Inspeccion
after insert
as
begin
if exists
(
select id_inspeccion
from inserted 
where poblacion_estimada <10000
)
begin
print 'Poblacion muy baja'
end
end
GO

--4
create trigger TR_ColmenaInactiva
on Colmena
after update
as
begin
if update(activa)
begin
print 'La colmena fue desactivada'
end

end
GO

--Disparador del trigger 1
insert into Inspeccion
(
id_inspeccion,
id_colmena,
fecha_hora,
estado_colmena,
estado_reina,
poblacion_estimada,
observaciones
)
Values
(
506,
101,
GETDATE(),
'Media',
'Joven',
40000,
'Prueba trigger 1'
)
GO

--disparador 2
update Colmena
set estado_colmena= 'Critica'
where id_colmena= 103
GO

--disparador 3
insert into Inspeccion
(
id_inspeccion,
id_colmena,
fecha_hora,
estado_colmena,
estado_reina,
poblacion_estimada,
observaciones
)
Values
(
507,
104,
GETDATE(),
'Debil',
'Vieja',
5000,
'Poblacion baja'
)
GO

--disparador 4
update colmena 
set activa = 0
where id_colmena = 105
GO


