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
	mar_id int IDENTITY(1,1) PRIMARY KEY,
	mar_nombre nvarchar(255) NOT NULL,
	)
GO

CREATE TABLE OSNR.Modelo (
	mod_id int IDENTITY(1,1) PRIMARY KEY,
	mod_nombre nvarchar(255) NOT NULL,
	mod_id_marca int REFERENCES OSNR.Marca NOT NULL
	)
GO

CREATE TABLE OSNR.Turno (
	tur_id int IDENTITY(1,1) PRIMARY KEY,
	tur_descripcion nvarchar(255) NOT NULL,
	tur_hora_inicio time NOT NULL,
	tur_hora_fin time NOT NULL,
	tur_valor_km numeric(18,2) NOT NULL,
	tur_precio_base numeric(18,2) NOT NULL,
	tur_habilitado bit DEFAULT 1 NOT NULL
	)
GO

CREATE TABLE OSNR.Usuario (
	usu_id int IDENTITY(1,1) PRIMARY KEY,
	usu_dni numeric(18, 0) NOT NULL,
	usu_nombre nvarchar(255) NOT NULL,
	usu_apellido nvarchar(255) NOT NULL,
	usu_direccion nvarchar(255) NOT NULL,
	usu_telefono numeric(18, 0) NOT NULL,
	usu_mail nvarchar(255) NOT NULL,
	usu_fecha_nacimiento datetime NOT NULL,

	/* Esto es propio del Usuario de login y no de la persona */
	usu_login nvarchar(255) UNIQUE NOT NULL,
	usu_password varbinary(255) NOT NULL,
	usu_cantidad_intentos smallint DEFAULT 0 NOT NULL,
	usu_habilitado bit DEFAULT 1 NOT NULL
	)
GO

CREATE TABLE OSNR.Chofer (
	cho_id int IDENTITY(1,1) PRIMARY KEY,
	cho_id_usuario int REFERENCES OSNR.Usuario NOT NULL,
	cho_id_turno int REFERENCES OSNR.Turno NOT NULL
	)
GO

CREATE TABLE OSNR.Cliente (
	cli_id int IDENTITY(1,1) PRIMARY KEY,
	cli_id_usuario int REFERENCES OSNR.Usuario NOT NULL
	)
GO

CREATE TABLE OSNR.Auto (
	aut_id int IDENTITY(1,1) PRIMARY KEY,
	aut_id_modelo int REFERENCES OSNR.Modelo NOT NULL,
	aut_patente nvarchar(255) NOT NULL,
	aut_licencia nvarchar(255) NOT NULL,
	aut_rodado nvarchar(255) NOT NULL,
	aut_habilitado bit DEFAULT 1 NOT NULL,
	aut_id_chofer int REFERENCES OSNR.Chofer NOT NULL
	)
GO

CREATE TABLE OSNR.Viaje (
	via_id int IDENTITY(1,1) PRIMARY KEY,
	via_cantidad_km int NOT NULL,
	via_fecha datetime NOT NULL,
	via_id_chofer int REFERENCES OSNR.Chofer NOT NULL,
	)
GO

CREATE TABLE OSNR.Rendicion (
	ren_id int IDENTITY(1,1) PRIMARY KEY,
	ren_numero int NOT NULL,
	ren_fecha datetime NOT NULL,
	ren_importe numeric(18,2) NOT NULL,
	ren_id_chofer int REFERENCES OSNR.Chofer NOT NULL
	)
GO

CREATE TABLE OSNR.RendicionViaje (
	renvia_id_rendicion int REFERENCES OSNR.Rendicion NOT NULL,
	renvia_id_viaje int REFERENCES OSNR.Viaje NOT NULL,
	renvia_porcentaje numeric(18,2) NOT NULL,
	PRIMARY KEY(renvia_id_rendicion, renvia_id_viaje)
)

CREATE TABLE OSNR.Factura (
	fac_id int IDENTITY(1,1) PRIMARY KEY,
	fac_numero int NOT NULL,
	fac_fecha datetime NOT NULL, /* TODO: CUANTAS FECHAS HAY?? */
	fac_fecha_inicio datetime NOT NULL,
	fac_fecha_fin datetime NOT NULL,
	fac_importe numeric(18,2) NOT NULL,
	fac_id_cliente int REFERENCES OSNR.Cliente NOT NULL
	)
GO


CREATE TABLE OSNR.Funcionalidad (
	fun_id int IDENTITY(1,1) PRIMARY KEY,
	fun_nombre nvarchar(255) NOT NULL
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
	rol_habilitado bit DEFAULT 1 NOT NULL
	)
GO

INSERT INTO OSNR.Rol (rol_nombre) values ('Administrador')
INSERT INTO OSNR.Rol (rol_nombre) values ('Cliente')
INSERT INTO OSNR.Rol (rol_nombre) values ('Chofer')

CREATE TABLE OSNR.FuncionalidadRol (
	funcrol_id_rol int REFERENCES OSNR.Rol NOT NULL,
	funcrol_id_funcionalidad int REFERENCES OSNR.Funcionalidad NOT NULL,
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
	usurol_id_usuario int REFERENCES OSNR.Usuario NOT NULL,
	usurol_id_rol int REFERENCES OSNR.Rol NOT NULL,
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