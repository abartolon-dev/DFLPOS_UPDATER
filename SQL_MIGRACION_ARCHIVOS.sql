/*
Migracion manual para pasar del flujo ZIP al flujo por carpeta de version.
Ejecutar en SQL Server sobre la base DflPosUpdater si no usas dotnet ef migrations.
*/

IF COL_LENGTH('Versiones', 'RutaCarpeta') IS NULL
BEGIN
    ALTER TABLE Versiones ADD RutaCarpeta nvarchar(500) NOT NULL CONSTRAINT DF_Versiones_RutaCarpeta DEFAULT('');
END;

IF COL_LENGTH('Versiones', 'TotalArchivos') IS NULL
BEGIN
    ALTER TABLE Versiones ADD TotalArchivos int NOT NULL CONSTRAINT DF_Versiones_TotalArchivos DEFAULT(0);
END;

IF COL_LENGTH('Despliegues', 'RutaRemotaCarpeta') IS NULL
BEGIN
    ALTER TABLE Despliegues ADD RutaRemotaCarpeta nvarchar(500) NULL;
END;

IF OBJECT_ID('VersionArchivos', 'U') IS NULL
BEGIN
    CREATE TABLE VersionArchivos
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_VersionArchivos PRIMARY KEY,
        VersionAppId int NOT NULL,
        NombreArchivo nvarchar(255) NOT NULL,
        RutaRelativa nvarchar(700) NOT NULL,
        RutaWeb nvarchar(700) NOT NULL,
        TamanoBytes bigint NOT NULL,
        Sha256 nvarchar(128) NOT NULL,
        Activo bit NOT NULL CONSTRAINT DF_VersionArchivos_Activo DEFAULT(1),
        FechaCarga datetime2 NOT NULL CONSTRAINT DF_VersionArchivos_FechaCarga DEFAULT(GETDATE()),
        CONSTRAINT FK_VersionArchivos_Versiones_VersionAppId
            FOREIGN KEY (VersionAppId) REFERENCES Versiones(Id) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_VersionArchivos_VersionAppId_RutaRelativa'
      AND object_id = OBJECT_ID('VersionArchivos')
)
BEGIN
    CREATE UNIQUE INDEX IX_VersionArchivos_VersionAppId_RutaRelativa
    ON VersionArchivos(VersionAppId, RutaRelativa);
END;

UPDATE Versiones
SET RutaCarpeta = RutaZip
WHERE ISNULL(RutaCarpeta, '') = '';

UPDATE Despliegues
SET RutaRemotaCarpeta = RutaRemotaZip
WHERE RutaRemotaCarpeta IS NULL AND RutaRemotaZip IS NOT NULL;
