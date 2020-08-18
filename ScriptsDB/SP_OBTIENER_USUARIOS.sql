CREATE PROCEDURE OBTIENER_USUARIOS
AS
BEGIN
SELECT 
IdUsuario,
Nombre,
Apellidos,
Usuario,
DECRYPTBYPASSPHRASE('password', Contrasena)as contrasena
FROM Soluciones_Desarrollo.dbo.Usuario
END 

