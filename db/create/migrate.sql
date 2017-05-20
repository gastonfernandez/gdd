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
	tur_hora_inicio numeric(18,0) NOT NULL,
	tur_hora_fin numeric(18,0) NOT NULL,
	tur_valor_km numeric(18,2) NOT NULL,
	tur_precio_base numeric(18,2) NOT NULL,
	tur_habilitado bit DEFAULT 1 NOT NULL
	)
GO

CREATE TABLE OSNR.Usuario (
	usu_id int IDENTITY(1,1) PRIMARY KEY,
	usu_dni numeric(18, 0) UNIQUE NOT NULL,
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
	cho_id_usuario int REFERENCES OSNR.Usuario UNIQUE NOT NULL
	)
GO

CREATE TABLE OSNR.Cliente (
	cli_id int IDENTITY(1,1) PRIMARY KEY,
	cli_id_usuario int REFERENCES OSNR.Usuario UNIQUE NOT NULL
	)
GO

CREATE TABLE OSNR.Auto (
	aut_id int IDENTITY(1,1) PRIMARY KEY,
	aut_id_modelo int REFERENCES OSNR.Modelo NOT NULL,
	aut_id_chofer int REFERENCES OSNR.Chofer NOT NULL,
	aut_patente nvarchar(255) NOT NULL,
	aut_licencia nvarchar(255) NOT NULL,
	aut_rodado nvarchar(255) NOT NULL,
	aut_habilitado bit DEFAULT 1 NOT NULL
	)
GO

CREATE TABLE OSNR.AutoTurno (
	auttur_id_auto int REFERENCES OSNR.Auto NOT NULL,
	auttur_id_turno int REFERENCES OSNR.Turno NOT NULL,
	PRIMARY KEY(auttur_id_auto, auttur_id_turno)
	)
GO

CREATE TABLE OSNR.Viaje (
	via_id int IDENTITY(1,1) PRIMARY KEY,
	via_cantidad_km int NOT NULL,
	via_fecha datetime NOT NULL,
	via_id_chofer int REFERENCES OSNR.Chofer NOT NULL,
	via_id_cliente int REFERENCES OSNR.Cliente NOT NULL,
	via_id_auto int REFERENCES OSNR.Auto NOT NULL
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
	fac_numero int UNIQUE NOT NULL,
	fac_fecha datetime NOT NULL,
	fac_fecha_inicio datetime NOT NULL,
	fac_fecha_fin datetime NOT NULL,
	fac_importe numeric(18,2) NOT NULL,
	fac_id_cliente int REFERENCES OSNR.Cliente NOT NULL
	)
GO

CREATE TABLE OSNR.FacturaViaje (
	facvia_id_factura int REFERENCES OSNR.Factura NOT NULL,
	facvia_id_viaje int REFERENCES OSNR.Viaje NOT NULL,
	PRIMARY KEY(facvia_id_factura, facvia_id_viaje)
)

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


/*****************************************************************/
/************************** MIGRACION ****************************/
/*****************************************************************/


/* Marca */
INSERT INTO OSNR.Marca
	SELECT DISTINCT Auto_Marca
	FROM gd_esquema.Maestra
GO

/* Modelo */
INSERT INTO OSNR.Modelo
	SELECT DISTINCT Auto_Modelo, mar_id
	FROM gd_esquema.Maestra, OSNR.Marca
	WHERE mar_nombre = Auto_Marca
GO


/* Usuarios */
/* Tomamos como usuario el nombre+apellido y como password su nombre */
INSERT INTO OSNR.Usuario
	SELECT DISTINCT
		Chofer_Dni
		Chofer_Nombre,
		Chofer_Apellido,
		Chofer_Direccion,
		Chofer_Telefono,
		Chofer_Mail,
		Chofer_Fecha_Nac,
		Chofer_Nombre + Chofer_Apellido,
		HashBytes('SHA2_256',convert(varchar(255), Chofer_Nombre)),
		0, /* Intentos login */
		1  /* Habilitado */
	FROM gd_esquema.Maestra
GO

INSERT INTO OSNR.Usuario
	SELECT DISTINCT
		Cliente_Dni,
		Cliente_Nombre,
		Cliente_Apellido,
		Cliente_Direccion,
		Cliente_Telefono,
		Cliente_Mail,
		Cliente_Fecha_Nac,
		Cliente_Nombre + Cliente_Apellido,
		HashBytes('SHA2_256',convert(varchar(255), Cliente_Nombre)),
		0, /* Intentos login */
		1  /* Habilitado */
	FROM gd_esquema.Maestra
GO


/* Chofer */
INSERT INTO OSNR.Chofer
	SELECT DISTINCT usu_id
	FROM gd_esquema.Maestra, OSNR.Usuario
	WHERE usu_dni = Chofer_Dni
GO

/* Cliente */
INSERT INTO OSNR.Cliente
	SELECT DISTINCT usu_id
	FROM gd_esquema.Maestra, OSNR.Usuario
	WHERE usu_dni = Cliente_Dni
GO

/* Auto */
INSERT INTO OSNR.Auto
	SELECT DISTINCT mod_id, cho_id, Auto_Patente, Auto_Licencia, Auto_Rodado, 1
	FROM gd_esquema.Maestra, OSNR.Modelo, OSNR.Usuario, OSNR.Chofer
	WHERE mod_nombre = Auto_Modelo AND usu_dni = Chofer_Dni AND cho_id_usuario = usu_id
GO

/* Turno */
INSERT INTO OSNR.Turno
	SELECT DISTINCT Turno_Descripcion, Turno_Hora_Inicio, Turno_Hora_Fin, Turno_Valor_Kilometro, Turno_Precio_Base, 1
	FROM gd_esquema.Maestra
GO

/* AutoTurno */
INSERT INTO OSNR.AutoTurno
	SELECT DISTINCT aut_id, tur_id
	FROM gd_esquema.Maestra, OSNR.Auto, OSNR.Turno
	WHERE aut_patente = Auto_Patente AND tur_descripcion = Turno_Descripcion
GO

/* Vije */
INSERT INTO OSNR.Viaje
	SELECT DISTINCT Viaje_Cant_Kilometros, Viaje_Fecha, 1, cli_id, aut_id
	FROM gd_esquema.Maestra, OSNR.Cliente, OSNR.Usuario, OSNR.Auto
	WHERE Viaje_Cant_Kilometros IS NOT NULL AND aut_patente = Auto_Patente 
	/* TODO CHOFER .. */
	AND usu_dni = Cliente_Dni AND cli_id_usuario = usu_id
GO

/* Factura */
INSERT INTO OSNR.Factura
	/* TODO: Importe total, podriamos sacarlo incluso... */
	SELECT DISTINCT Factura_Nro, Factura_Fecha, Factura_Fecha_Inicio, Factura_Fecha_Fin, 100, 1
	FROM gd_esquema.Maestra, OSNR.Usuario, OSNR.Cliente
	WHERE Factura_Nro IS NOT NULL AND usu_dni = Cliente_Dni AND cli_id_usuario = usu_id
GO

/* FacturaViaje */
INSERT INTO OSNR.FacturaViaje
	SELECT DISTINCT fac_id, via_id /* TODO ESTO NO PARECERIA ANDAR */
	FROM gd_esquema.Maestra, OSNR.Factura, OSNR.Viaje
	WHERE Factura_Nro IS NOT NULL AND Factura_Nro = fac_numero AND Viaje_Fecha = via_fecha
GO


 