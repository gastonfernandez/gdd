GO
/****** Object:  StoredProcedure [OSNR].[ModificarCliente] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [OSNR].[ModificarCliente](@Cliente numeric(18, 0), @Nombre varchar(255), 
@Apellido varchar(255), @Documento numeric(18,0), 
@Domicilio varchar(255), @Telefono numeric(18,0), @Email varchar(255), @FechaNac datetime, 

AS
BEGIN

DECLARE @Existe INT

SELECT @Existe = COUNT(*) FROM OSNR.Cliente WHERE cli_id_usuario = @Cliente

IF (@Existe = 0) 
BEGIN
	INSERT INTO OSNR.Usuario (usu_nombre, usu_apellido, usu_dni, usu_direccion, 
	usu_telefono, usu_mail, usu_fecha_nacimiento, usu_login, usu_password, usu_cantidad_intentos) VALUES (@Nombre, @Apellido, 
	@Documento, @Domicilio, @Telefono, @Email, @FechaNac, @Nombre+@Apellido, HASHBYTES('SHA2_256', CAST(@Nombre AS VARCHAR(18))), 0)
	
	INSERT INTO OSNR.Cliente (cli_id, cli_id_usuario) VALUES (@@IDENTITY, ????)

END
ELSE
BEGIN
	DECLARE @IdUsuario numeric(18,0)
	SELECT @IdUsuario = cli_id FROM OSNR.Cliente WHERE cli_id_usuario = @Cliente

	UPDATE OSNR.Usuario SET usu_direccion = @Domicilio, usu_telefono = @Telefono, usu_mail = @Email,
	usu_fecha_nacimiento = @FechaNac, usu_login = @Nombre+@Apellido WHERE usu_id = @IdUsuario

END

END

/**************************************************************************************/

GO
/****** Object:  StoredProcedure [OSNR].[ObtenerNuevoIdCliente]  ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [OSNR].[ObtenerNuevoIdCliente] (@id numeric(18, 0) OUT)
AS BEGIN
	DECLARE @Existe INT = 1

	DECLARE @ultimo numeric(18, 0), @cadena varchar(18)

	SELECT @ultimo = MAX(cli_id) FROM OSNR.Cliente
	SET @cadena = CAST(@ultimo AS varchar(18))

	SET @ultimo = CAST(SUBSTRING(@cadena, 1, LEN(@cadena) - 2) AS numeric(16, 0))


	WHILE(@Existe <> 0)
		BEGIN
			SET @ultimo = @ultimo + 1
			SET @id = CAST(CAST(@ultimo AS varchar(16)) + '01' AS numeric(18, 0))
			SELECT @Existe = COUNT(*) FROM OSNR.Cliente WHERE cli_id = @id
		END

END

/**************************************************************************************/

GO
/****** Object:  StoredProcedure [OSNR].[CargarClientes]  ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [OSNR].[CargarClientes] (@Cliente varchar(18), @Nombre varchar(255), @Apellido varchar(255), @Documento varchar(18))
AS
BEGIN
SELECT cli_id AS 'Nro. Cliente', usu_nombre AS 'Nombre', usu_apellido AS 'Apellido', usu_dni AS 'Documento', 
usu_telefono AS 'Telefono', usu_mail AS 'Email'
FROM OSNR.Cliente JOIN OSNR.Usuario ON cli_id_usuario = usu_id
WHERE CONVERT(varchar(18), cli_id) LIKE '%'+ISNULL(@Cliente, '')+'%' 
AND usu_nombre LIKE '%'+ISNULL(@Nombre, '')+'%' 
AND usu_apellido LIKE '%'+ISNULL(@Apellido, '')+'%' 
AND CONVERT(varchar(18), usu_dni) LIKE '%'+ISNULL(@Documento, '')+'%'
ORDER BY cli_id
RETURN
END

/**************************************************************************************/

GO
/****** Object:  StoredProcedure [OSNR].[EliminarCliente]  ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [OSNR].[EliminarCliente](@Cliente numeric(18, 0), @Fecha datetime) 

/* 	NECESITAMOS GUARDAR LA FECHA DE BAJA??? */

AS
BEGIN

UPDATE OSNR.Usuario SET usu_habilitado = 0 WHERE usu_id = (SELECT cli_id_usuario FROM OSNR.Cliente WHERE cli_id = @Cliente)

END


















