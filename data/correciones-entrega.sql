/*****************************************************************/
/********************** CORRECIONES ENTREGA 1 ********************/
/*****************************************************************/


-- CORRECION PUNTO 1
ALTER PROCEDURE OSNR.ModificarOCrearTurno
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

-- CORRECCION PUNTO 2

-- Validacion de fechas para los viajes
DELETE FROM OSNR.Viaje WHERE via_fecha_inicio > via_fecha_fin
ALTER TABLE OSNR.Viaje WITH CHECK ADD CHECK (via_fecha_inicio <= via_fecha_fin)
GO

ALTER PROCEDURE OSNR.RegistrarViaje
@idChofer   bigint    ,
@idCliente  bigint    ,
@idTurno    bigint    ,
@idVehiculo bigint    ,
@fechaDesde datetime    ,
@fechaHasta datetime    ,
@cantKm     int    
AS
	-- Obtengo el turno pedido
	DECLARE @tur_hora_inicio numeric(18,0)
	DECLARE @tur_hora_fin numeric(18,0)
	SELECT	@tur_hora_inicio = tur_hora_inicio, @tur_hora_fin = tur_hora_fin 
	FROM	OSNR.Turno 
	WHERE	tur_id=@idTurno AND tur_habilitado=1

	IF @tur_hora_inicio IS NULL
		THROW 60001, 'No existe el turno (o esta deshabilitado)..', 1

	DECLARE @viaje_hora_desde numeric(18,0) = DATEPART(HOUR, @fechaDesde)
	DECLARE @viaje_hora_hasta numeric(18,0) = DATEPART(HOUR, @fechaHasta)

	IF @viaje_hora_desde < @tur_hora_inicio OR @viaje_hora_desde > @tur_hora_fin OR @viaje_hora_hasta < @tur_hora_inicio OR @viaje_hora_hasta > @tur_hora_fin
		THROW 60002, 'Las fechas no estan dentro del rango del turno seleccionado', 1

	insert into OSNR.Viaje
	(via_id_chofer,via_id_cliente,via_id_turno,via_id_vehiculo,via_fecha_inicio,via_fecha_fin,via_cantidad_km)
	values (@idChofer,@idCliente,@idTurno,@idVehiculo,@fechaDesde,@fechaHasta,@cantKm)
GO

-- CORRECION PUNTO 3
CREATE PROCEDURE OSNR.PrevisualizarViajesRendicion
@fecha date,
@idTurno int,
@idChofer int,
@porcentaje numeric(18, 2)
AS
	IF(NOT EXISTS (SELECT cho_id from OSNR.Chofer WHERE cho_id = @idChofer AND cho_habilitado = 1))
		THROW 60001, 'El chofer no existe o no esta habilitado', 1

	IF(NOT EXISTS (SELECT tur_id from OSNR.Turno WHERE tur_id = @idTurno AND tur_habilitado = 1))
		THROW 60002, 'El turno no existe o no esta habilitado', 1

	IF (EXISTS (SELECT via_id FROM OSNR.Viaje WHERE via_id_turno=@idTurno AND
													via_id_chofer=@idChofer AND
													CAST(via_fecha_inicio AS DATE)=@fecha AND
													via_id IN (SELECT renvia_id_viaje FROM OSNR.RendicionViaje)))
		THROW 60003, 'La rendicion para ese dia, chofer y fecha ya existe..', 1

	------------------------------------

	SELECT	via_fecha_inicio FechaInicioViaje,
			via_fecha_fin FechaFinViaje,
			via_cantidad_km CantidadKmViaje,
			ucli.usu_nombre NombreCliente,
			(tur_precio_base + tur_valor_km * via_cantidad_km) AS ValorViaje,
			((tur_precio_base + tur_valor_km * via_cantidad_km) * @porcentaje /100) GananciaChofer
	FROM	OSNR.Viaje
			JOIN OSNR.Turno on tur_id=via_id_turno
			JOIN OSNR.Cliente on cli_id=via_id_cliente
			JOIN OSNR.Usuario ucli on usu_id=cli_id_usuario
	WHERE	via_id_turno=@idTurno AND
			via_id_chofer=@idChofer AND
			CAST(via_fecha_inicio AS DATE)=@fecha
GO
