CREATE DATABASE Soluciones_Desarrollo;

create table Soluciones_Desarrollo.dbo.Usuario(
IdUsuario int identity(1,1) primary key,
Nombre varchar(50) not null,
Apellidos  varchar(100) not null,
Usuario varchar(50) not null,
Contrasena VARBINARY(8000) not null
);

INSERT INTO Soluciones_Desarrollo.dbo.Usuario(Nombre,Apellidos,Usuario,Contrasena)VALUES('PRUEBA','PRUEBA PRUEBA','UserPrueba',ENCRYPTBYPASSPHRASE('password', 'user123'));

