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
	usu_direccion nvarchar(255) NOT NULL,
	usu_telefono numeric(18, 0) UNIQUE NOT NULL,
	usu_mail nvarchar(255),
	usu_fecha_nacimiento datetime NOT NULL,

	/* Esto es propio del Usuario de login y no de la persona */
	usu_login nvarchar(255) UNIQUE NOT NULL,
	usu_password varbinary(255) NOT NULL,
	usu_cantidad_intentos smallint DEFAULT 0 NOT NULL
	)
GO

CREATE TABLE OSNR.Chofer (
	cho_id int IDENTITY(1,1) PRIMARY KEY,
	cho_id_usuario int REFERENCES OSNR.Usuario UNIQUE NOT NULL,
	cho_habilitado bit DEFAULT 1 NOT NULL
	)
GO

CREATE TABLE OSNR.Cliente (
	cli_id int IDENTITY(1,1) PRIMARY KEY,
	cli_id_usuario int REFERENCES OSNR.Usuario UNIQUE NOT NULL,
	cli_habilitado bit DEFAULT 1 NOT NULL
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

INSERT INTO OSNR.Usuario (usu_nombre, usu_apellido, usu_dni, usu_direccion, usu_telefono, usu_fecha_nacimiento, usu_login, usu_password)
	values ('Administrador', 'OSNR', '11111', 'En la esquina, a la vuelta', '34346557634987', GETDATE(), 'admin', HASHBYTES('SHA2_256', 'w23e'))
GO
INSERT INTO OSNR.UsuarioRol(usurol_id_usuario, usurol_id_rol)
	values(1, 1)
GO

INSERT INTO OSNR.UsuarioRol(usurol_id_usuario, usurol_id_rol)
	values(1, 2)

/* Agregamos las funcionalidades.. */
INSERT INTO OSNR.Funcionalidad (fun_nombre) values ('ABM Rol')
INSERT INTO OSNR.Funcionalidad (fun_nombre) values ('ABM Cliente')
INSERT INTO OSNR.Funcionalidad (fun_nombre) values ('ABM Chofer')
INSERT INTO OSNR.Funcionalidad (fun_nombre) values ('ABM Automovil')
INSERT INTO OSNR.Funcionalidad (fun_nombre) values ('ABM Turno')
INSERT INTO OSNR.Funcionalidad (fun_nombre) values ('Registro de Viaje')
INSERT INTO OSNR.Funcionalidad (fun_nombre) values ('Rendicion de Viaje')
INSERT INTO OSNR.Funcionalidad (fun_nombre) values ('Facturacion de Cliente')
INSERT INTO OSNR.Funcionalidad (fun_nombre) values ('Listados Estadistico')
GO

/* (id_rol, id_funcionalidad) (primero todas las del admin..) */
-- Admin
INSERT INTO OSNR.FuncionalidadRol values (1,1) -- ABM Rol
INSERT INTO OSNR.FuncionalidadRol values (1,2) -- ABM Cliente
INSERT INTO OSNR.FuncionalidadRol values (1,3) -- ABM Chofer
INSERT INTO OSNR.FuncionalidadRol values (1,4) -- ABM Automovil
INSERT INTO OSNR.FuncionalidadRol values (1,5) -- ABM Turno
INSERT INTO OSNR.FuncionalidadRol values (1,6) -- Registro de Viaje
INSERT INTO OSNR.FuncionalidadRol values (1,7) -- Rendicion de Viaje
INSERT INTO OSNR.FuncionalidadRol values (1,8) -- Facturacion de Cliente
INSERT INTO OSNR.FuncionalidadRol values (1,9) -- Listados Estadistico

-- Cliente
--INSERT INTO OSNR.FuncionalidadRol values (2,6) -- QUE BOSTA PUEDE HACER EL CLIENTE???


-- Chofer
INSERT INTO OSNR.FuncionalidadRol values (3,6) -- Registro de Viaje
GO


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
/* Tomamos como usuario el nombre y como password el nombre */
INSERT INTO OSNR.Usuario
	SELECT DISTINCT
		Chofer_Dni,
		Chofer_Nombre,
		Chofer_Apellido,
		Chofer_Direccion,
		Chofer_Telefono,
		Chofer_Mail,
		Chofer_Fecha_Nac,
		CONVERT(VARCHAR(18), Chofer_Telefono),						 -- Usuario
		HASHBYTES('SHA2_256', CONVERT(VARCHAR(18), Chofer_Telefono)),-- Password
		0 --Intentos login
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
		CONVERT(VARCHAR(18), Cliente_Telefono),						  -- Usuario
		HASHBYTES('SHA2_256', CONVERT(VARCHAR(18), Cliente_Telefono)),-- Password
		0 --Intentos login
	FROM gd_esquema.Maestra
GO


/* Chofer */
INSERT INTO OSNR.Chofer
	SELECT DISTINCT usu_id,
					 1  --Habilitado
	FROM gd_esquema.Maestra, OSNR.Usuario
	WHERE usu_dni = Chofer_Dni
GO

/* Cliente */
INSERT INTO OSNR.Cliente
	SELECT DISTINCT usu_id,
				     1  --Habilitado
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
					1 --Habilitado
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
					1  --Habilitado
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

/* Viaje */
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

/* Rendicion */
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

	
/*****************************************************************/
/*********************** Stored Procedures ***********************/
/*****************************************************************/


/*Listados Estadisticos*/

--Choferes con mayor recaudación
CREATE PROCEDURE OSNR.TOP5ChoferesConMayorRecaudacion
@fecha_inicio datetime,
@fecha_fin datetime
AS
	SELECT TOP 5
		usu_nombre Nombre,
		usu_apellido Apellido,
		usu_dni DNI,
		usu_telefono Telefono,
		usu_mail Mail,
		SUM(ren_importe) FacturacionTotal
	FROM OSNR.Chofer
		JOIN OSNR.Rendicion ON ren_id_chofer=cho_id
		JOIN OSNR.Usuario ON usu_id=cho_id_usuario
	WHERE ren_fecha between @fecha_inicio and @fecha_fin
	GROUP BY cho_id, usu_nombre, usu_apellido, usu_dni, usu_telefono, usu_mail
	ORDER BY FacturacionTotal DESC
GO

--Choferes con el viaje más largo realizado
CREATE PROCEDURE OSNR.TOP5ChoferesConViajeMasLargo
@fecha_inicio datetime,
@fecha_fin datetime
AS
	SELECT TOP 5
		usu_nombre Nombre,
		usu_apellido Apellido,
		usu_dni DNI,
		usu_telefono Telefono,
		usu_mail Mail,
		via_cantidad_km KmViajeMasLargo
	FROM OSNR.Viaje
		JOIN OSNR.Chofer ON via_id_chofer=cho_id
		JOIN OSNR.Usuario ON usu_id=cho_id_usuario
	WHERE via_fecha between @fecha_inicio and @fecha_fin
	ORDER BY KmViajeMasLargo DESC
GO

--Clientes con mayor consumo
CREATE PROCEDURE OSNR.TOP5ClientesConMayorConsumo
@fecha_inicio datetime,
@fecha_fin datetime
AS
	SELECT TOP 5
		usu_nombre Nombre,
		usu_apellido Apellido,
		usu_dni DNI,
		usu_telefono Telefono,
		usu_mail Mail,
		SUM(fac_importe) ConsumoTotal
	FROM OSNR.Cliente
		JOIN OSNR.Factura ON fac_id_cliente=cli_id
		JOIN OSNR.Usuario ON usu_id=cli_id_usuario
	WHERE fac_fecha between @fecha_inicio and @fecha_fin
	GROUP BY cli_id, usu_nombre, usu_apellido, usu_dni, usu_telefono, usu_mail
	ORDER BY ConsumoTotal DESC
GO

--Cliente que utilizo más veces el mismo automóvil en los viajes que ha realizado
CREATE PROCEDURE OSNR.TOP5ClientesConMayorCantidadDeMismoAutomovil
@fecha_inicio datetime,
@fecha_fin datetime
AS
	SELECT TOP 5
		usu_nombre Nombre,
		usu_apellido Apellido,
		usu_dni DNI,
		usu_telefono Telefono,
		usu_mail Mail,
		veh_patente Patente,
		veh_licencia Licencia,
		veh_rodado Rodado,
		COUNT(via_id_vehiculo) CantidadVecesUtilizado
	FROM OSNR.Cliente
		JOIN OSNR.Viaje ON via_id_cliente=cli_id
		JOIN OSNR.Usuario ON usu_id=cli_id_usuario
		JOIN OSNR.Vehiculo ON veh_id=via_id_vehiculo
	WHERE via_fecha between @fecha_inicio and @fecha_fin
	GROUP BY cli_id, usu_nombre, usu_apellido, usu_dni, usu_telefono, usu_mail, via_id_vehiculo, veh_patente, veh_licencia, veh_rodado
	ORDER BY CantidadVecesUtilizado DESC
GO

-- ABM Clientes
CREATE PROCEDURE OSNR.BuscarClientes
@nombre varchar(255),
@apellido varchar(255),
@dni varchar(18)
AS
	SELECT
		cli_id 'Nro. Cliente',
		usu_nombre 'Nombre',
		usu_apellido 'Apellido',
		usu_dni 'Documento',
		usu_telefono 'Telefono',
		usu_direccion 'Direccion',
		usu_mail 'Email',
		cli_habilitado 'Habilitado'
	FROM OSNR.Cliente 
		JOIN OSNR.Usuario ON cli_id_usuario = usu_id
	WHERE
		usu_nombre LIKE '%'+ISNULL(@nombre, '')+'%' 
		AND usu_apellido LIKE '%'+ISNULL(@apellido, '')+'%' 
		AND (@dni IS NULL OR @dni = '' OR CONVERT(varchar(18), usu_dni) = @dni)
	ORDER BY cli_id
GO


CREATE PROCEDURE OSNR.DeshabilitarCliente
@clienteId numeric(18, 0)
AS
	UPDATE OSNR.Cliente 
	SET cli_habilitado = 0 
	WHERE cli_id = @clienteId
GO


CREATE PROCEDURE OSNR.ModificarOCrearCliente
@clienteId numeric(18, 0) = NULL,
@Nombre varchar(255), @Apellido varchar(255), @Dni numeric(18,0),
@Direccion varchar(255), @Telefono numeric(18,0), @Email varchar(255), @FechaNac datetime
AS

DECLARE @usuarioId INT
SELECT @usuarioId = cli_id_usuario FROM OSNR.Cliente WHERE cli_id = @clienteId

IF (@usuarioId IS NULL) 
	BEGIN
		INSERT INTO OSNR.Usuario 
		(
			usu_nombre,
			usu_apellido, 
			usu_dni, 
			usu_direccion, 
			usu_telefono,
			usu_mail, 
			usu_fecha_nacimiento, 
			usu_login, 
			usu_password
		) 
		VALUES (
			@Nombre, 
			@Apellido, 
			@Dni, 
			@Direccion,
			@Telefono,
			@Email,
			@FechaNac, 
			CONVERT(VARCHAR(18), @Telefono),
			HASHBYTES('SHA2_256', CONVERT(VARCHAR(18), @Telefono))
		)

		SET @usuarioId = @@IDENTITY
		INSERT INTO OSNR.Cliente (cli_id_usuario) VALUES (@usuarioId)
		INSERT INTO OSNR.UsuarioRol(usurol_id_usuario, usurol_id_rol)
			values(@usuarioId, 2)
	END
ELSE
	BEGIN
		UPDATE OSNR.Usuario 
		SET 
			usu_nombre = @Nombre,
			usu_apellido = @Apellido,
			usu_dni = @Dni,
			usu_direccion = @Direccion, 
			usu_telefono = @Telefono,
			usu_mail = @Email,
			usu_fecha_nacimiento = @FechaNac
		WHERE usu_id = @usuarioId
	END
GO

