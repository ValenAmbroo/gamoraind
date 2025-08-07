-- Paso 1: Crear solo la base de datos VentasDB
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'VentasDB')
BEGIN
    CREATE DATABASE VentasDB
    ON (
        NAME = 'VentasDB',
        FILENAME = 'C:\Users\ivanv\Desktop\gamoraind\Gamora Indumentaria\bin\Debug\VentasDB.mdf',
        SIZE = 10MB,
        MAXSIZE = 100MB,
        FILEGROWTH = 5MB
    )
    LOG ON (
        NAME = 'VentasDB_Log',
        FILENAME = 'C:\Users\ivanv\Desktop\gamoraind\Gamora Indumentaria\bin\Debug\VentasDB_Log.ldf',
        SIZE = 1MB,
        MAXSIZE = 10MB,
        FILEGROWTH = 1MB
    );
    
    PRINT 'Base de datos VentasDB creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La base de datos VentasDB ya existe.';
END
