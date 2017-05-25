USE [GD1C2017]
GO

/*** BORRO LO EXISTENTE ***/
DECLARE @names_sp varchar(max)
	DECLARE @names_func varchar(max)
	DECLARE @names_veiws varchar(max)
	DECLARE @names_tables varchar(max)
	DECLARE @names_types varchar(max)
	DECLARE @names_triggers varchar(max)

	DECLARE @sql varchar(max)

	--Borro los triggers
	SELECT @names_triggers = coalesce(@names_triggers + ', ','') + '[OSNR].' + t.NAME
	FROM GD1C2017.sys.objects t, GD1C2017.sys.schemas s
	WHERE s.schema_id = t.schema_id AND s.name = 'OSNR' AND  t.type = 'TR'
	
	SET @sql = 'DROP TRIGGER ' + @names_triggers
	EXEC(@sql)

	--Borro los stored procedures
	SELECT @names_sp = coalesce(@names_sp + ', ','') + '[OSNR].' + p.NAME
	FROM GD1C2017.sys.procedures p, GD1C2017.sys.schemas s
	WHERE s.schema_id = p.schema_id AND p.NAME != 'CleanDatabase' AND p.NAME != 'NO_CHECK_CONSTRAINS' AND s.name = 'OSNR'
	
	SET @sql = 'DROP PROCEDURE ' + @names_sp
	EXEC(@sql)

	--Borro las functions
	SELECT @names_func = coalesce(@names_func + ', ','') + '[OSNR].' + f.NAME
	FROM GD1C2017.sys.objects f, GD1C2017.sys.schemas s
	WHERE s.schema_id = f.schema_id AND s.name = 'OSNR' AND  f.type IN ('FN', 'IF', 'TF')
	
	SET @sql = 'DROP FUNCTION ' + @names_func
	EXEC(@sql)


	--Borro las vistas
	SELECT @names_veiws = coalesce(@names_veiws + ', ','') + '[OSNR].' + TABLE_NAME
	FROM GD1C2017.INFORMATION_SCHEMA.VIEWS
	WHERE TABLE_SCHEMA = 'OSNR'

	SET @sql = 'DROP VIEW ' + @names_veiws
	EXEC(@sql)

	-- Deshabilito la integridad referencial de las tablas a borrar
	
	DECLARE tables_in_schema CURSOR FOR 
	SELECT f.name, Object_NAME(f.parent_object_id)
	FROM sys.foreign_keys AS f JOIN
	sys.schemas AS s ON s.schema_id = f.schema_id
	WHERE s.name = 'OSNR'

	DECLARE @table_name varchar(max)
	DECLARE @fk_name varchar(max)

	OPEN tables_in_schema 

	FETCH tables_in_schema INTO  @fk_name, @table_name

	WHILE (@@FETCH_STATUS = 0) 
	BEGIN 
		SET @sql = 'ALTER TABLE OSNR.' + @table_name + ' DROP CONSTRAINT ' + @fk_name
		EXEC(@sql)

		FETCH tables_in_schema INTO  @fk_name, @table_name
	END 

	CLOSE tables_in_schema 
	DEALLOCATE tables_in_schema



	--Borro las tablas excepto la maestra
	SELECT @names_tables = coalesce(@names_tables + ', ','') + '[OSNR].' + TABLE_NAME
	FROM GD1C2017.INFORMATION_SCHEMA.TABLES
	WHERE TABLE_SCHEMA = 'OSNR' and TABLE_TYPE = 'BASE TABLE'

	SET @sql = 'DROP TABLE ' + @names_tables
	EXEC(@sql)

	--Borro los User define types
	SELECT @names_types = coalesce( @names_types + ' DROP TYPE ','DROP TYPE ') + '[OSNR].' + t.NAME
	FROM GD1C2017.sys.types t, GD1C2017.sys.schemas s
	WHERE s.schema_id = t.schema_id AND s.name = 'OSNR'

	SET @sql = @names_types
	EXEC(@sql)

IF (EXISTS (SELECT * FROM sys.schemas WHERE name = 'OSNR')) 
BEGIN
	DROP SCHEMA OSNR
END


/*********************************************************************
*
*    FIN BORRADO 
*
*    COMIENZA CREACION E INSERCION
*
************************************************************************/


IF (NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'OSNR')) 
BEGIN
SET @sql = ' CREATE SCHEMA [OSNR] AUTHORIZATION gd'
	EXEC(@sql)

  
END


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
	usu_direccion nvarchar(255),
	usu_telefono numeric(18, 0),
	usu_mail nvarchar(255),
	usu_fecha_nacimiento datetime,

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

CREATE TABLE OSNR.Vehiculo (
	veh_id int IDENTITY(1,1) PRIMARY KEY,
	veh_id_modelo int REFERENCES OSNR.Modelo NOT NULL,
	veh_id_chofer int REFERENCES OSNR.Chofer NOT NULL,
	veh_patente nvarchar(255) NOT NULL,
	veh_licencia nvarchar(255) NOT NULL,
	veh_rodado nvarchar(255) NOT NULL,
	veh_habilitado bit DEFAULT 1 NOT NULL
	)
GO

CREATE TABLE OSNR.VehiculoTurno (
	auttur_id_vehiculo int REFERENCES OSNR.Vehiculo NOT NULL,
	auttur_id_turno int REFERENCES OSNR.Turno NOT NULL,
	PRIMARY KEY(auttur_id_vehiculo, auttur_id_turno)
	)
GO

CREATE TABLE OSNR.Viaje (
	via_id int IDENTITY(1,1) PRIMARY KEY,
	via_cantidad_km int NOT NULL,
	via_fecha datetime NOT NULL,
	via_id_chofer int REFERENCES OSNR.Chofer NOT NULL,
	via_id_cliente int REFERENCES OSNR.Cliente NOT NULL,
	via_id_vehiculo int REFERENCES OSNR.Vehiculo NOT NULL
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

CREATE TABLE OSNR.Rol (
	rol_id int IDENTITY(1,1) PRIMARY KEY,
	rol_nombre nvarchar(255) UNIQUE NOT NULL,
	rol_habilitado bit DEFAULT 1 NOT NULL
	)
GO

CREATE TABLE OSNR.FuncionalidadRol (
	funcrol_id_rol int REFERENCES OSNR.Rol NOT NULL,
	funcrol_id_funcionalidad int REFERENCES OSNR.Funcionalidad NOT NULL,
	PRIMARY KEY(funcrol_id_rol, funcrol_id_funcionalidad)
	)
GO

CREATE TABLE OSNR.UsuarioRol (
	usurol_id_usuario int REFERENCES OSNR.Usuario NOT NULL,
	usurol_id_rol int REFERENCES OSNR.Rol NOT NULL,
	PRIMARY KEY(usurol_id_usuario, usurol_id_rol)
	)
GO

/* Creamos los roles, y un usuario admin por default.. */
INSERT INTO OSNR.Rol (rol_nombre) values ('Administrador')	/* ID 1 */
INSERT INTO OSNR.Rol (rol_nombre) values ('Cliente')		/* ID 2 */
INSERT INTO OSNR.Rol (rol_nombre) values ('Chofer')			/* ID 3 */

INSERT INTO OSNR.Usuario (usu_nombre, usu_apellido, usu_dni, usu_login, usu_password)
	values ('Administrador', 'OSNR', '11111', 'admin', HASHBYTES('SHA2_256', 'w23e'))
GO
INSERT INTO OSNR.UsuarioRol(usurol_id_usuario, usurol_id_rol)
	values(1, 1)
GO

/* TODO: ASIGNAR FUNCIONALIDADES A CADA ROL..
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
	SELECT DISTINCT Auto_Modelo, 
					mar_id
	FROM	gd_esquema.Maestra mas 
			join OSNR.Marca m on  m.mar_nombre = mas.Auto_Marca
GO



/* Usuarios */
/* Tomamos como usuario el nombre+apellido y como password su nombre */
INSERT INTO OSNR.Usuario
	SELECT DISTINCT
		Chofer_Dni,
		Chofer_Nombre,
		Chofer_Apellido,
		Chofer_Direccion,
		Chofer_Telefono,
		Chofer_Mail,
		Chofer_Fecha_Nac,
		Chofer_Nombre + Chofer_Apellido,
		HASHBYTES('SHA2_256', Chofer_Nombre),
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
		HASHBYTES('SHA2_256', Cliente_Nombre),
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
INSERT INTO OSNR.Vehiculo
	SELECT DISTINCT mod_id, 
					cho_id, 
					mas.Auto_Patente, 
					mas.Auto_Licencia, 
					mas.Auto_Rodado, 
					1
	FROM	gd_esquema.Maestra mas
			join OSNR.Modelo m on m.mod_nombre = mas.Auto_Modelo
			join OSNR.Usuario u on u.usu_dni = mas.Chofer_Dni
			join OSNR.Chofer c on c.cho_id_usuario = u.usu_id
GO

/* Turno */

INSERT INTO	OSNR.Turno
	SELECT DISTINCT Turno_Descripcion, 
					Turno_Hora_Inicio, 
					Turno_Hora_Fin, 
					Turno_Valor_Kilometro, 
					Turno_Precio_Base, 
					1 /*habilitado*/
	FROM	gd_esquema.Maestra
GO

/* VehiculoTurno */
INSERT INTO OSNR.VehiculoTurno
	SELECT DISTINCT veh_id, 
					tur_id
	FROM gd_esquema.Maestra mas 
		 join OSNR.Vehiculo a on  veh_patente = Auto_Patente
		 join OSNR.Turno t on tur_descripcion = Turno_Descripcion

GO

/* Vije */
INSERT INTO OSNR.Viaje
(via_cantidad_km,via_fecha,via_id_chofer,via_id_cliente,via_id_vehiculo)
	SELECT DISTINCT Viaje_Cant_Kilometros, 
					Viaje_Fecha, 
					ch.cho_id, 
					c.cli_id, 
					v.veh_id
					 
	FROM gd_esquema.Maestra	mas
		 join OSNR.Usuario uc on uc.usu_dni = Cliente_Dni  
		 join OSNR.Cliente c on cli_id_usuario = uc.usu_id
	     join OSNR.Vehiculo v on veh_patente = Auto_Patente
   		 join OSNR.Usuario uch on uch.usu_dni = Chofer_Dni  
		 join OSNR.Chofer ch on ch.cho_id_usuario=uch.usu_id
	WHERE Viaje_Cant_Kilometros IS NOT NULL   
	
	
GO


/* Factura */
INSERT INTO OSNR.Factura
SELECT		    Factura_Nro, 
					Factura_Fecha, 
					Factura_Fecha_Inicio, 
					Factura_Fecha_Fin, 
					sum(viaje_cant_kilometros*turno_valor_kilometro + turno_precio_base), 
					c.cli_id
	FROM	gd_esquema.Maestra mas
			join OSNR.Usuario us on us.usu_dni = Cliente_Dni
			join OSNR.Cliente c on  cli_id_usuario = usu_id
	WHERE Factura_Nro IS NOT NULL 
	group by Factura_Nro,Factura_Fecha,Factura_Fecha_Inicio,Factura_Fecha_Fin,c.cli_id
GO



/* FacturaViaje */
INSERT INTO OSNR.FacturaViaje
	SELECT DISTINCT fac_id ,
					via_id 
	FROM			gd_esquema.Maestra mas
					join OSNR.Factura on Factura_Nro = fac_numero
					join OSNR.Usuario uc on uc.usu_dni = mas.Cliente_Dni
					join OSNR.Usuario uch on uch.usu_dni = mas.Chofer_Dni
					join OSNR.Vehiculo v on v.veh_patente = mas.Auto_Patente
					join OSNR.Cliente c on cli_id_usuario = uc.usu_id
			        join OSNR.Chofer ch on ch.cho_id_usuario=uch.usu_id
	     			join OSNR.Viaje via on  Viaje_Fecha = via.via_fecha
										and via.via_id_chofer=ch.cho_id
										and via.via_id_cliente =c.cli_id
										and via.via_id_vehiculo=v.veh_id
	where factura.fac_numero is not null
GO


insert into OSNR.Rendicion
(ren_numero,ren_fecha,ren_importe,ren_id_chofer)
	select distinct mas.Rendicion_Nro,
			mas.Rendicion_Fecha,
			mas.Rendicion_Importe,
			ch.cho_id
	from	gd_esquema.Maestra mas
			join OSNR.Usuario uch on uch.usu_dni = mas.Chofer_Dni
			join OSNR.Chofer ch on ch.cho_id_usuario=uch.usu_id
	where	mas.Rendicion_Nro is not null	     			
GO



