USE [GD1C2017]
GO

/*** BORRO LO EXISTENTE ***/
DECLARE @names_sp varchar(max)
DECLARE @names_func varchar(max)
DECLARE @names_veiws varchar(max)
DECLARE @names_secs varchar(max)
DECLARE @names_tables varchar(max)
DECLARE @names_types varchar(max)
DECLARE @names_triggers varchar(max)

DECLARE @table_name varchar(max)
DECLARE @fk_name varchar(max)

DECLARE @sql varchar(max)

-- Borro las check constraints
DECLARE tables_in_schema CURSOR FOR 
SELECT f.name, Object_NAME(f.parent_object_id)
FROM sys.check_constraints AS f JOIN
sys.schemas AS s ON s.schema_id = f.schema_id
WHERE s.name = 'OSNR'


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

--Borro las secuencias
SELECT @names_secs = coalesce(@names_secs + ', ','') + '[OSNR].' + SEQUENCE_NAME
FROM GD1C2017.INFORMATION_SCHEMA.SEQUENCES
WHERE SEQUENCE_SCHEMA = 'OSNR'

SET @sql = 'DROP SEQUENCE ' + @names_secs
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
	veh_patente nvarchar(255) UNIQUE NOT NULL,
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
	via_fecha_inicio datetime NOT NULL,
	via_fecha_fin datetime NOT NULL,
	via_id_chofer int REFERENCES OSNR.Chofer NOT NULL,
	via_id_cliente int REFERENCES OSNR.Cliente NOT NULL,
	via_id_vehiculo int REFERENCES OSNR.Vehiculo NOT NULL,
	via_id_turno int REFERENCES OSNR.Turno NOT NULL
	)
GO

CREATE TABLE OSNR.Rendicion (
	ren_id int IDENTITY(1,1) PRIMARY KEY,
	ren_numero int UNIQUE NOT NULL,
	ren_importe numeric(18,2) NOT NULL,
	ren_fecha date NOT NULL,
	ren_id_chofer int REFERENCES OSNR.Chofer NOT NULL,
	ren_id_turno int REFERENCES OSNR.Turno NOT NULL
	CONSTRAINT [UQ_Rendicion] UNIQUE (ren_fecha, ren_id_chofer, ren_id_turno)
	)
GO

CREATE TABLE OSNR.RendicionViaje (
	renvia_id_rendicion int REFERENCES OSNR.Rendicion NOT NULL,
	renvia_id_viaje int REFERENCES OSNR.Viaje UNIQUE NOT NULL, -- Un viaje no puede estar dos veces
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
	fac_id_cliente int REFERENCES OSNR.Cliente NOT NULL,
	CONSTRAINT [UQ_Factura] UNIQUE (fac_fecha_inicio, fac_fecha_fin, fac_id_cliente)
	)
GO

CREATE TABLE OSNR.FacturaViaje (
	facvia_id_factura int REFERENCES OSNR.Factura NOT NULL,
	facvia_id_viaje int REFERENCES OSNR.Viaje UNIQUE NOT NULL, -- Un viaje no puede estar dos veces
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

-- Seteo el rol de cada chofer
INSERT INTO OSNR.UsuarioRol(usurol_id_usuario, usurol_id_rol)
	SELECT DISTINCT
		usu_id,	1
	FROM gd_esquema.Maestra JOIN OSNR.Usuario on usu_telefono = Chofer_Telefono
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
-- Seteo el rol de cada cliente
INSERT INTO OSNR.UsuarioRol(usurol_id_usuario, usurol_id_rol)
	SELECT DISTINCT
		usu_id,	1
	FROM gd_esquema.Maestra JOIN OSNR.Usuario on usu_telefono = Cliente_Telefono
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
	(via_cantidad_km,via_fecha_inicio,via_fecha_fin,via_id_chofer,via_id_cliente,via_id_vehiculo,via_id_turno)
	SELECT DISTINCT Viaje_Cant_Kilometros, 
					Viaje_Fecha, 
					Viaje_Fecha, -- en la migracion ponemos misma fecha inicio que fin
					ch.cho_id, 
					c.cli_id, 
					v.veh_id,
					tur_id
	FROM gd_esquema.Maestra	mas
		 join OSNR.Usuario uc on uc.usu_dni = Cliente_Dni
		 join OSNR.Cliente c on cli_id_usuario = uc.usu_id
	     join OSNR.Vehiculo v on veh_patente = Auto_Patente
   		 join OSNR.Usuario uch on uch.usu_dni = Chofer_Dni
		 join OSNR.Chofer ch on ch.cho_id_usuario=uch.usu_id
		 JOIN OSNR.Turno ON tur_descripcion = Turno_Descripcion
	WHERE Viaje_Cant_Kilometros IS NOT NULL AND Rendicion_Nro IS NOT NULL
GO

/* Factura */
INSERT INTO OSNR.Factura
SELECT		    Factura_Nro, 
					Factura_Fecha, 
					Factura_Fecha_Inicio, 
					Factura_Fecha_Fin,
					(SELECT SUM(SQ.viaje_cant_kilometros*SQ.turno_valor_kilometro + SQ.turno_precio_base)
						FROM (SELECT DISTINCT * FROM gd_esquema.Maestra masInner WHERE masInner.Factura_Nro=mas.Factura_Nro) SQ),
					c.cli_id
	FROM	gd_esquema.Maestra mas
			join OSNR.Usuario us on us.usu_dni = Cliente_Dni
			join OSNR.Cliente c on  cli_id_usuario = usu_id
	WHERE Factura_Nro IS NOT NULL 
	group by Factura_Nro,Factura_Fecha,Factura_Fecha_Inicio,Factura_Fecha_Fin,c.cli_id
GO

-- Secuencia para los numeros de facturas..
DECLARE @maxFacturaNro INT = (SELECT MAX(fac_numero)+1 FROM OSNR.Factura)
DECLARE @sql NVARCHAR(MAX)
SET @sql = 'CREATE SEQUENCE OSNR.SecuenciaFacturaNumero AS INT
			 START WITH '  + CAST(@maxFacturaNro AS VARCHAR) + '
			 INCREMENT BY 1 MINVALUE 0 NO MAXVALUE'
EXEC(@sql)
ALTER TABLE OSNR.Factura ADD DEFAULT (NEXT VALUE FOR OSNR.SecuenciaFacturaNumero) FOR fac_numero
--

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
	     			join OSNR.Viaje via on  Viaje_Fecha = via.via_fecha_inicio
										and via.via_id_chofer=ch.cho_id
										and via.via_id_cliente =c.cli_id
										and via.via_id_vehiculo=v.veh_id
	where factura.fac_numero is not null
GO

/* Rendicion */

INSERT INTO OSNR.Rendicion
	SELECT DISTINCT 
		mas.Rendicion_Nro,
		SUM(mas.Rendicion_Importe), -- El importe de la rendicion es la suma de todos los viajes que la componen
		mas.Rendicion_Fecha,
		cho_id,
		tur_id
	FROM	gd_esquema.Maestra mas
			JOIN OSNR.Usuario uch on uch.usu_dni = mas.Chofer_Dni
			JOIN OSNR.Chofer ch on ch.cho_id_usuario=uch.usu_id
			JOIN OSNR.Turno ON tur_descripcion=mas.Turno_Descripcion
	WHERE	mas.Rendicion_Nro is not null
	GROUP BY mas.Rendicion_Nro, mas.Rendicion_Fecha, cho_id, tur_id
GO

-- Secuencia para los numeros de rendiciones..
DECLARE @maxRendicionNro INT = (SELECT MAX(ren_numero)+1 FROM OSNR.Rendicion)
DECLARE @sql NVARCHAR(MAX)
SET @sql = 'CREATE SEQUENCE OSNR.SecuenciaRendicionNumero AS INT
			 START WITH '  + CAST(@maxRendicionNro AS VARCHAR) + '
			 INCREMENT BY 1 MINVALUE 0 NO MAXVALUE'
EXEC(@sql)
ALTER TABLE OSNR.Rendicion ADD DEFAULT (NEXT VALUE FOR OSNR.SecuenciaRendicionNumero) FOR ren_numero
--

INSERT INTO OSNR.RendicionViaje
	(renvia_id_rendicion, renvia_id_viaje, renvia_porcentaje)
	SELECT DISTINCT 
		ren_id,
		via_id,
		30 --El calculo nos dio que todos los registros guardados tienen 30 %
	FROM	gd_esquema.Maestra
			join OSNR.Rendicion ON ren_numero=Rendicion_Nro
			join OSNR.Viaje ON via_cantidad_km=Viaje_Cant_Kilometros
							AND via_fecha_inicio=Viaje_Fecha
							AND via_id_chofer=ren_id_chofer
	WHERE	Rendicion_Nro IS NOT NULL     			
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
	WHERE via_fecha_inicio between @fecha_inicio and @fecha_fin
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
	WHERE via_fecha_inicio between @fecha_inicio and @fecha_fin
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


CREATE PROCEDURE OSNR.RegistrarViaje
@idChofer   bigint    ,
@idCliente  bigint    ,
@idTurno    bigint    ,
@idVehiculo bigint    ,
@fechaDesde datetime    ,
@fechaHasta datetime    ,
@cantKm     int    
AS
	insert into OSNR.Viaje
	(via_id_chofer,via_id_cliente,via_id_turno,via_id_vehiculo,via_fecha_inicio,via_fecha_fin,via_cantidad_km)
	values (@idChofer,@idCliente,@idTurno,@idVehiculo,@fechaDesde,@fechaHasta,@cantKm)

GO

create procedure OSNR.BuscarViajesCliente
@idCliente bigint,
@fechaDesde datetime,
@fechaHasta datetime
AS
select * 
from OSNR.Viaje
where via_id_cliente=@idCliente
and  ( (@fechaDesde between via_fecha_inicio and via_fecha_fin)
		or (@fechaHasta between via_fecha_inicio and via_fecha_fin)
     )
GO
create procedure OSNR.BuscarViajesChofer
@idChofer bigint,
@fechaDesde datetime,
@fechaHasta datetime
AS
select * 
from OSNR.Viaje
where via_id_chofer=@idChofer
and  ( (@fechaDesde>=  via_fecha_inicio and  @fechaDesde<=via_fecha_fin)
		or (@fechaHasta >= via_fecha_inicio and @fechaHasta<=via_fecha_fin)
     )
GO

-- ABM Choferes
CREATE PROCEDURE OSNR.BuscarChoferes
@nombre varchar(255),
@apellido varchar(255),
@dni varchar(18)
AS
 SELECT
  cho_id 'Nro. Chofer',
  usu_nombre 'Nombre',
  usu_apellido 'Apellido',
  usu_dni 'Documento',
  usu_telefono 'Telefono',
  usu_direccion 'Direccion',
  usu_mail 'Email',
  cho_habilitado 'Habilitado'
 FROM OSNR.Chofer 
  JOIN OSNR.Usuario ON cho_id_usuario = usu_id
 WHERE
  usu_nombre LIKE '%'+ISNULL(@nombre, '')+'%' 
  AND usu_apellido LIKE '%'+ISNULL(@apellido, '')+'%' 
  AND (@dni IS NULL OR @dni = '' OR CONVERT(varchar(18), usu_dni) = @dni)
 ORDER BY cho_id
GO

CREATE PROCEDURE OSNR.DeshabilitarChofer
@choferId numeric(18, 0)
AS
 UPDATE OSNR.Chofer
 SET cho_habilitado = 0 
 WHERE cho_id = @choferId
GO

CREATE PROCEDURE OSNR.HabilitarChofer
@choferId numeric(18, 0)
AS
 UPDATE OSNR.Chofer
 SET cho_habilitado = 1 
 WHERE cho_id = @choferId
GO

CREATE PROCEDURE OSNR.ModificarOCrearChofer
@choferId numeric(18, 0) = NULL,
@Nombre varchar(255), @Apellido varchar(255), @Dni numeric(18,0),
@Direccion varchar(255), @Telefono numeric(18,0), @Email varchar(255), @FechaNac datetime
AS
 DECLARE @usuarioId INT
 SELECT @usuarioId = cho_id_usuario FROM OSNR.Chofer WHERE cho_id = @choferId

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
   INSERT INTO OSNR.Chofer (cho_id_usuario) VALUES (@usuarioId)
   INSERT INTO OSNR.UsuarioRol(usurol_id_usuario, usurol_id_rol)
    values(@usuarioId, 3)
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

-- ABM Turnos
CREATE PROCEDURE OSNR.BuscarTurnos
@descripcion varchar(255)
AS
 SELECT
  tur_id 'Nro. Turno',
  tur_descripcion 'Descripcion',
  tur_hora_inicio 'Hora Inicio',
  tur_hora_fin 'Hora Fin',
  tur_valor_km 'Valor Km',
  tur_precio_base 'Precio Base',
  tur_habilitado 'Habilitado'
 FROM OSNR.Turno 
 WHERE
  tur_descripcion LIKE '%'+ISNULL(@descripcion, '')+'%' 
 ORDER BY tur_id
GO

CREATE PROCEDURE OSNR.DeshabilitarTurno
@turnoId numeric(18, 0)
AS
 UPDATE OSNR.Turno
 SET tur_habilitado = 0 
 WHERE tur_id = @turnoId
GO

CREATE PROCEDURE OSNR.HabilitarTurno
@turnoId numeric(18, 0)
AS
 UPDATE OSNR.Turno
 SET tur_habilitado = 1 
 WHERE tur_id = @turnoId
GO

CREATE PROCEDURE OSNR.ModificarOCrearTurno
@turnoId numeric(18, 0) = NULL,
@descripcion varchar(255), @horaInicio numeric(18, 0), @horaFin numeric(18,0),
@valorKm numeric(18, 2), @precioBase numeric(18,2)
AS
 IF (@turnoId IS NULL) 
  BEGIN
   INSERT INTO OSNR.Turno 
   (
	tur_descripcion,
	tur_hora_inicio,
	tur_hora_fin,
	tur_valor_km,
	tur_precio_base
   ) 
   VALUES (
    @descripcion,
	@horaInicio,
	@horaFin,
	@valorKm,
	@precioBase 
   )

   SET @turnoId = @@IDENTITY
   INSERT INTO OSNR.Turno (tur_id) VALUES (@turnoId)
  END
 ELSE
  BEGIN
   UPDATE OSNR.Turno 
   SET
	tur_descripcion = @descripcion,
	tur_hora_inicio = @horaInicio,
	tur_hora_fin = @horaFin,
	tur_valor_km = @valorKm,
	tur_precio_base = @precioBase 
   WHERE tur_id = @turnoId
  END
GO

-- Rendicion
CREATE PROCEDURE OSNR.CrearRendicion
@fecha date,
@idTurno int,
@idChofer int,
@porcentaje numeric(18, 2)
AS

	IF(NOT EXISTS (SELECT cho_id from OSNR.Chofer WHERE cho_id = @idChofer AND cho_habilitado = 1))
		THROW 60001, 'El chofer no existe o no esta habilitado', 1

	-- Obtengo el turno pedido
	DECLARE @tur_valor_km numeric(18,2)
	DECLARE @tur_precio_base numeric(18,2)
	SELECT	@tur_valor_km = tur_valor_km, @tur_precio_base = tur_precio_base 
	FROM	OSNR.Turno 
	WHERE	tur_id=@idTurno AND tur_habilitado=1

	IF @tur_valor_km IS NULL
		THROW 60002, 'No existe el turno (o esta deshabilitado)..', 1
	------------------------------------
	-- Obtengo el valor total para la rendicion
	DECLARE @valorTotal numeric(18, 2)
	SELECT	@valorTotal = SUM((@tur_precio_base + @tur_valor_km * via_cantidad_km) * @porcentaje /100)
	FROM	OSNR.Viaje 
	WHERE	via_id_turno=@idTurno AND
			via_id_chofer=@idChofer AND
			CAST(via_fecha_inicio AS DATE)=@fecha

	-------------------------------------
	DECLARE CViajesRendicion CURSOR FOR
		SELECT  via_id, via_cantidad_km
		FROM	OSNR.Viaje
		WHERE	via_id_turno=@idTurno AND
				via_id_chofer=@idChofer AND
				CAST(via_fecha_inicio AS DATE)=@fecha
	OPEN CViajesRendicion

	DECLARE @rendicionId INT
	BEGIN TRY
		BEGIN TRANSACTION
			--Creo la rendicion
			INSERT INTO OSNR.Rendicion(ren_importe, ren_fecha, ren_id_chofer, ren_id_turno)
			VALUES (@valorTotal, @fecha, @idChofer, @idTurno)
			SET @rendicionId = @@IDENTITY

			-------------------------------------
			-- Itero sobre todos los viajes que aplican..
			DECLARE @via_id int
			DECLARE @via_cantidad_km datetime
		
			FETCH CViajesRendicion INTO @via_id, @via_cantidad_km
			WHILE(@@FETCH_STATUS = 0)
				BEGIN
				INSERT INTO OSNR.RendicionViaje(renvia_id_rendicion, renvia_id_viaje, renvia_porcentaje)
					values(@rendicionId, @via_id, @porcentaje)
				FETCH CVIajesRendicion INTO @via_id, @via_cantidad_km
				END
		COMMIT
	END TRY
	BEGIN CATCH
		IF(@@TRANCOUNT > 0)
			ROLLBACK TRANSACTION;

		THROW 60003, 'La rendicion para ese dia, chofer y fecha ya existe..', 1
	END CATCH

	CLOSE CViajesRendicion
	DEALLOCATE CViajesRendicion

	SELECT	via_fecha_inicio FechaInicioViaje,
			via_fecha_fin FechaFinViaje,
			via_cantidad_km CantidadKmViaje,
			ucli.usu_nombre NombreCliente,
			(tur_precio_base + tur_valor_km * via_cantidad_km) AS ValorViaje,
			((tur_precio_base + tur_valor_km * via_cantidad_km) * renvia_porcentaje /100) GananciaChofer
	FROM	OSNR.RendicionViaje
			JOIN OSNR.Viaje on via_id=renvia_id_viaje
			JOIN OSNR.Turno on tur_id=via_id_turno
			JOIN OSNR.Cliente on cli_id=via_id_cliente
			JOIN OSNR.Usuario ucli on usu_id=cli_id_usuario
	WHERE	renvia_id_rendicion = @rendicionId
GO

CREATE PROCEDURE OSNR.ObtenerRendicion
@fecha date,
@idTurno int,
@idChofer int
AS
	SELECT	ren_numero NumeroRendicion, ren_importe ImporteTotal
	FROM	OSNR.Rendicion
	WHERE 
			@fecha=ren_fecha AND
			@idChofer=ren_id_chofer AND
			@idTurno=ren_id_turno
GO


-- Factura
CREATE PROCEDURE OSNR.CrearFactura
@fechaInicio date,
@fechaFin date,
@idCliente int,
@hoy date
AS
	IF(NOT EXISTS (SELECT cli_id from OSNR.Cliente WHERE cli_id = @idCliente AND cli_habilitado = 1))
		THROW 60001, 'El cliente no existe o no esta habilitado', 1

	------------------------------------
	-- Obtengo el valor total para la factura
	DECLARE @valorTotal numeric(18, 2)
	SELECT	@valorTotal = SUM(tur_precio_base + tur_valor_km * via_cantidad_km)
	FROM	OSNR.Viaje
			JOIN OSNR.Turno on tur_id=via_id_turno
	WHERE	via_id_cliente=@idCliente AND
			CAST(via_fecha_inicio AS DATE) >= @fechaInicio AND
			CAST(via_fecha_inicio AS DATE) <= @fechaFin

	-------------------------------------
	DECLARE CViajesFactura CURSOR FOR
		SELECT  via_id, via_cantidad_km
		FROM	OSNR.Viaje
		WHERE	via_id_cliente=@idCliente AND
				CAST(via_fecha_inicio AS DATE) >= @fechaInicio AND
				CAST(via_fecha_inicio AS DATE) <= @fechaFin
	OPEN CViajesFactura

	DECLARE @facturaId INT
	BEGIN TRY
		BEGIN TRANSACTION
			--Creo la factura
			INSERT INTO OSNR.Factura(fac_fecha, fac_fecha_inicio, fac_fecha_fin, fac_importe, fac_id_cliente)
			VALUES (@hoy, @fechaInicio, @fechaFin, @valorTotal, @idCliente)
			SET @facturaId = @@IDENTITY

			-------------------------------------
			-- Itero sobre todos los viajes que aplican..
			DECLARE @via_id int
			DECLARE @via_cantidad_km datetime
		
			FETCH CViajesFactura INTO @via_id, @via_cantidad_km
			WHILE(@@FETCH_STATUS = 0)
				BEGIN
				INSERT INTO OSNR.FacturaViaje(facvia_id_factura, facvia_id_viaje)
					values(@facturaId, @via_id)
				FETCH CViajesFactura INTO @via_id, @via_cantidad_km
				END
		COMMIT
	END TRY
	BEGIN CATCH
		IF(@@TRANCOUNT > 0)
			ROLLBACK TRANSACTION;

		THROW 60003, 'La factura para esos dias y cliente ya existe..', 1
	END CATCH

	CLOSE CViajesFactura
	DEALLOCATE CViajesFactura

	SELECT	via_fecha_inicio FechaInicioViaje,
			via_fecha_fin FechaFinViaje,
			via_cantidad_km CantidadKmViaje,
			ucho.usu_nombre NombreChofer,
			(tur_precio_base + tur_valor_km * via_cantidad_km) AS ValorViaje
	FROM	OSNR.FacturaViaje
			JOIN OSNR.Viaje on via_id=facvia_id_viaje
			JOIN OSNR.Turno on tur_id=via_id_turno
			JOIN OSNR.Chofer on cho_id=via_id_chofer
			JOIN OSNR.Usuario ucho on usu_id=cho_id_usuario
	WHERE	facvia_id_factura = @facturaId
GO

CREATE PROCEDURE OSNR.ObtenerFactura
@fechaInicio date,
@fechaFin date,
@idCliente int
AS
	SELECT	fac_numero NumeroFactura, fac_importe ImporteTotal
	FROM	OSNR.Factura
	WHERE 
			CAST(fac_fecha_inicio AS DATE) = @fechaInicio AND
			CAST(fac_fecha_fin AS DATE) = @fechaFin AND
			@idCliente=fac_id_cliente
GO


/*****************************************************************/
/*************************** Functions ***************************/
/*****************************************************************/

-- Validacion de overlap en turnos
CREATE FUNCTION OSNR.CheckTurnoOverlap(
@HoraInicio numeric(18, 2),
@HoraFin numeric(18, 2),
@IdTurno int)
RETURNS BIT 
AS
BEGIN
	DECLARE @retval BIT
  
	IF EXISTS (
		SELECT	*
		FROM	OSNR.Turno
		WHERE		
				@IdTurno <> tur_id
				AND ((@HoraInicio >= tur_hora_inicio AND @HoraInicio < tur_hora_fin) OR (@HoraFin > tur_hora_inicio AND @HoraFin <= tur_hora_fin))
		)
		BEGIN
			RETURN 1
		END

	RETURN 0
END
GO

ALTER TABLE OSNR.Turno WITH CHECK ADD CONSTRAINT CK_HorarioTurno CHECK (OSNR.CheckTurnoOverlap(tur_hora_inicio, tur_hora_fin, tur_id)<>1)
