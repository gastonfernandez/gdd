USE [GD1C2017]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE SCHEMA [OSNR] AUTHORIZATION gd
GO

/*****************************************************************/
/***********************CREACION DE TABLAS************************/
/*****************************************************************/


CREATE TABLE OSNR.Marca (
	marca_id int IDENTITY(1,1) PRIMARY KEY,
	marca_nombre nvarchar(255) NOT NULL,
	)
GO

CREATE TABLE OSNR.Modelo (
	modelo_id int IDENTITY(1,1) PRIMARY KEY,
	modelo_nombre nvarchar(255) NOT NULL,
	modelo_id_marca int REFERENCES OSNR.Marca NOT NULL
	)
GO

CREATE TABLE OSNR.Turno (
	turno_id int IDENTITY(1,1) PRIMARY KEY,
	turno_descripcion nvarchar(255) NOT NULL,
	turno_hora_inicio time NOT NULL,
	turno_hora_fin time NOT NULL,
	turno_valor_km numeric(18,2) NOT NULL,
	turno_precio_base numeric(18,2) NOT NULL,
	turno_habilitado bit DEFAULT 1
	)
GO

CREATE TABLE OSNR.Usuario (
	usuario_id int IDENTITY(1,1) PRIMARY KEY,
	usuario_dni numeric(18, 0) NOT NULL,
	usuario_nombre nvarchar(255) NOT NULL,
	usuario_apellido nvarchar(255) NOT NULL,
	ususario_direccion nvarchar(255) NOT NULL,
	ususario_telefono numeric(18, 0) NOT NULL,
	ususario_mail nvarchar(255),
	ususario_fecha_nacimiento datetime NOT NULL,

	/* Esto es propio del Usuario de login y no de la persona */
	usuario_login nvarchar(255) UNIQUE,
	usuario_password varbinary(255) NOT NULL,
	usuario_cantidad_intentos smallint DEFAULT 0 NOT NULL,
	usuario_habilitado bit NOT NULL DEFAULT 1
	)
GO

CREATE TABLE OSNR.Chofer (
	chofer_id int IDENTITY(1,1) PRIMARY KEY,
	chofer_id_usuario int REFERENCES OSNR.Usuario NOT NULL,
	chofer_id_turno int REFERENCES OSNR.Turno NOT NULL
	)
GO

CREATE TABLE OSNR.Cliente (
	cliente_id int IDENTITY(1,1) PRIMARY KEY,
	liente_id_usuario int REFERENCES OSNR.Usuario NOT NULL
	)
GO

CREATE TABLE OSNR.Auto (
	auto_id int IDENTITY(1,1) PRIMARY KEY,
	auto_id_modelo int REFERENCES OSNR.Modelo NOT NULL,
	auto_patente nvarchar(255) NOT NULL,
	auto_licencia nvarchar(255) NOT NULL,
	auto_rodado nvarchar(255) NOT NULL,
	auto_habilitado bit DEFAULT 1,
	auto_id_chofer int REFERENCES OSNR.Chofer NOT NULL
	)
GO

CREATE TABLE OSNR.Viaje (
	viaje_id int IDENTITY(1,1) PRIMARY KEY,
	viaje_cantidad_km int NOT NULL,
	viaje_fecha datetime NOT NULL,
	viaje_id_chofer int REFERENCES OSNR.Chofer NOT NULL,
	)
GO

CREATE TABLE OSNR.Factura (
	factura_id int IDENTITY(1,1) PRIMARY KEY,
	factura_numero int NOT NULL,
	factura_fecha datetime NOT NULL, /* TODO: CUANTAS FECHAS HAY?? */
	factura_fecha_inicio datetime NOT NULL,
	factura_fecha_fin datetime NOT NULL,
	factura_importe numeric(18,2) NOT NULL,
	factura_id_cliente int REFERENCES OSNR.Cliente
	)
GO

CREATE TABLE OSNR.Rendicion (
	rendicion_id int IDENTITY(1,1) PRIMARY KEY,
	rendicion_numero int NOT NULL,
	rendicion_fecha datetime NOT NULL, /* TODO: CUANTAS FECHAS HAY?? */
	rendicion_importe numeric(18,2) NOT NULL,
	factura_id_chofer int REFERENCES OSNR.Chofer
	)
GO


CREATE TABLE OSNR.Funcionalidad (
	funcionalidad_id int IDENTITY(1,1) PRIMARY KEY,
	funcionalidad_nombre nvarchar(255) NOT NULL
	)
GO

/* CREAER LAS FUNCIONALIDADES DEFAULT...
INSERT INTO OSNR.Funcionalidad (descripcion) values ('ABM Rol')
INSERT INTO OSNR.Funcionalidad (descripcion) values ('ABM Ruta Aerea')
INSERT INTO OSNR.Funcionalidad (descripcion) values ('ABM Aeronave')
INSERT INTO OSNR.Funcionalidad (descripcion) values ('Generar Viaje')
INSERT INTO OSNR.Funcionalidad (descripcion) values ('Registrar Llegada')
INSERT INTO OSNR.Funcionalidad (descripcion) values ('Compra Pasaje/Encomienda') --cliente
INSERT INTO OSNR.Funcionalidad (descripcion) values ('Cancelacion Pasaje/Encomienda')
INSERT INTO OSNR.Funcionalidad (descripcion) values ('Consultar Millas') --cliente
INSERT INTO OSNR.Funcionalidad (descripcion) values ('Canjear Millas') --cliente
INSERT INTO OSNR.Funcionalidad (descripcion) values ('Listados Estadisticos')
GO
*/

CREATE TABLE OSNR.Rol (
	rol_id int IDENTITY(1,1) PRIMARY KEY,
	rol_nombre nvarchar(255) UNIQUE NOT NULL,
	rol_habilitado bit NOT NULL DEFAULT 1
	)
GO

INSERT INTO OSNR.Rol (rol_nombre) values ('Administrador')
INSERT INTO OSNR.Rol (rol_nombre) values ('Cliente')
INSERT INTO OSNR.Rol (rol_nombre) values ('Chofer')

CREATE TABLE OSNR.FuncionalidadRol (
	funcrol_id_rol int REFERENCES OSNR.Rol,
	funcrol_id_funcionalidad int REFERENCES OSNR.Funcionalidad,
	PRIMARY KEY(funcrol_id_rol, funcrol_id_funcionalidad)
	)
GO

/* ASIGNAR FUNCIONALIDADES A CADA ROL..
INSERT INTO OSNR.FuncionalidadRol values (1,1)
INSERT INTO OSNR.FuncionalidadRol values (1,2)
INSERT INTO OSNR.FuncionalidadRol values (1,3)
INSERT INTO OSNR.FuncionalidadRol values (1,4)
INSERT INTO OSNR.FuncionalidadRol values (1,5)
INSERT INTO OSNR.FuncionalidadRol values (1,6)
INSERT INTO OSNR.FuncionalidadRol values (1,7)
INSERT INTO OSNR.FuncionalidadRol values (1,8)
INSERT INTO OSNR.FuncionalidadRol values (1,9)
INSERT INTO OSNR.FuncionalidadRol values (1,10)

INSERT INTO OSNR.FuncionalidadRol values (2,6)
INSERT INTO OSNR.FuncionalidadRol values (2,8)
INSERT INTO OSNR.FuncionalidadRol values (2,9)
GO
*/

/*
INSERT INTO OSNR.Usuario (nombre_usuario, password)
	values ('Juan', HASHBYTES('SHA2_256', 'w23e'))
INSERT INTO OSNR.Usuario (nombre_usuario, password)
	values ('admin', HASHBYTES('SHA2_256', 'w23e'))
GO
*/

CREATE TABLE OSNR.UsuarioRol (
	usurol_id_usuario int REFERENCES OSNR.Usuario,
	usurol_id_rol int REFERENCES OSNR.Rol,
	PRIMARY KEY(usurol_id_usuario, usurol_id_rol)
	)
GO

/* AGREGAR ROLES A LOS USUARIOS POR DEFAULT..
INSERT INTO OSNR.RolPorUsuario(id_rol, nombre_usuario)
	values(1, 1)
INSERT INTO OSNR.RolPorUsuario(id_rol, nombre_usuario)
	values(1, 2)
GO
*/