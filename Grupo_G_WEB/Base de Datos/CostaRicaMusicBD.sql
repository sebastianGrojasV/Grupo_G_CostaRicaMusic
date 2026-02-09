-- =============================================================================
-- Grupo_G_WEB - Base de datos inspirada en Spotify
-- SQL Server - Tablas y procedimientos almacenados
-- =============================================================================

USE master;
GO

IF DB_ID('CostaRicaMusicBD') IS NOT NULL
    DROP DATABASE CostaRicaMusicBD;
GO

CREATE DATABASE CostaRicaMusicBD;
GO

USE CostaRicaMusicBD;
GO

-- =============================================================================
-- TABLAS
-- =============================================================================

-- Usuarios (autenticación y perfil)
CREATE TABLE Usuarios (
    Id              INT IDENTITY(1,1) NOT NULL,
    NombreUsuario   NVARCHAR(100) NOT NULL,
    Email           NVARCHAR(255) NULL,
    ContrasenaHash  NVARCHAR(500) NOT NULL,
    NombreCompleto  NVARCHAR(200) NULL,
    Activo          BIT NOT NULL DEFAULT 1,
    FechaRegistro   DATETIME2(0) NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Usuarios PRIMARY KEY (Id),
    CONSTRAINT UQ_Usuarios_NombreUsuario UNIQUE (NombreUsuario),
    CONSTRAINT UQ_Usuarios_Email UNIQUE (Email)
);
GO

-- Artistas (cantantes)
CREATE TABLE Artistas (
    Id            INT IDENTITY(1,1) NOT NULL,
    Nombre        NVARCHAR(200) NOT NULL,
    Biografia     NVARCHAR(MAX) NULL,
    UrlImagen     NVARCHAR(500) NULL,
    FechaCreacion DATETIME2(0) NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Artistas PRIMARY KEY (Id)
);
GO

-- Álbumes
CREATE TABLE Albumes (
    Id            INT IDENTITY(1,1) NOT NULL,
    Nombre        NVARCHAR(200) NOT NULL,
    IdArtista     INT NOT NULL,
    Anio          INT NULL,
    UrlPortada    NVARCHAR(500) NULL,
    FechaCreacion DATETIME2(0) NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Albumes PRIMARY KEY (Id),
    CONSTRAINT FK_Albumes_Artistas FOREIGN KEY (IdArtista) REFERENCES Artistas(Id) ON DELETE NO ACTION
);
GO

-- Canciones
CREATE TABLE Canciones (
    Id              INT IDENTITY(1,1) NOT NULL,
    Nombre          NVARCHAR(300) NOT NULL,
    IdAlbum         INT NOT NULL,
    IdArtista       INT NOT NULL,
    DuracionSegundos INT NOT NULL DEFAULT 0,
    NumeroPista     INT NOT NULL DEFAULT 1,
    RutaArchivo    NVARCHAR(500) NOT NULL,
    FechaCreacion   DATETIME2(0) NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Canciones PRIMARY KEY (Id),
    CONSTRAINT FK_Canciones_Albumes FOREIGN KEY (IdAlbum) REFERENCES Albumes(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Canciones_Artistas FOREIGN KEY (IdArtista) REFERENCES Artistas(Id) ON DELETE NO ACTION
);
GO

-- Playlists (listas de reproducción por usuario)
CREATE TABLE Playlists (
    Id            INT IDENTITY(1,1) NOT NULL,
    IdUsuario     INT NOT NULL,
    Nombre        NVARCHAR(200) NOT NULL,
    Descripcion   NVARCHAR(500) NULL,
    FechaCreacion DATETIME2(0) NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Playlists PRIMARY KEY (Id),
    CONSTRAINT FK_Playlists_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(Id) ON DELETE CASCADE
);
GO

-- Detalle: canciones que integran cada playlist (con orden)
CREATE TABLE PlaylistCanciones (
    Id            INT IDENTITY(1,1) NOT NULL,
    IdPlaylist    INT NOT NULL,
    IdCancion     INT NOT NULL,
    Orden         INT NOT NULL DEFAULT 0,
    FechaAgregado DATETIME2(0) NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_PlaylistCanciones PRIMARY KEY (Id),
    CONSTRAINT FK_PlaylistCanciones_Playlists FOREIGN KEY (IdPlaylist) REFERENCES Playlists(Id) ON DELETE CASCADE,
    CONSTRAINT FK_PlaylistCanciones_Canciones FOREIGN KEY (IdCancion) REFERENCES Canciones(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_PlaylistCanciones_Playlist_Cancion UNIQUE (IdPlaylist, IdCancion)
);
GO

-- Índices para búsqueda y listados paginados
CREATE NONCLUSTERED INDEX IX_Canciones_Nombre ON Canciones(Nombre);
CREATE NONCLUSTERED INDEX IX_Canciones_IdAlbum ON Canciones(IdAlbum);
CREATE NONCLUSTERED INDEX IX_Canciones_IdArtista ON Canciones(IdArtista);
CREATE NONCLUSTERED INDEX IX_Playlists_IdUsuario ON Playlists(IdUsuario);
CREATE NONCLUSTERED INDEX IX_PlaylistCanciones_IdPlaylist ON PlaylistCanciones(IdPlaylist);
GO

-- =============================================================================
-- DATOS INICIALES (opcional - ejemplo)
-- =============================================================================

INSERT INTO Artistas (Nombre, Biografia) VALUES
    (N'Artista Ejemplo 1', N'Biografía del artista 1'),
    (N'Artista Ejemplo 2', N'Biografía del artista 2');
GO

INSERT INTO Albumes (Nombre, IdArtista, Anio) VALUES
    (N'Álbum Demo 1', 1, 2024),
    (N'Álbum Demo 2', 2, 2023);
GO

INSERT INTO Canciones (Nombre, IdAlbum, IdArtista, DuracionSegundos, NumeroPista, RutaArchivo) VALUES
    (N'Canción de prueba 1', 1, 1, 180, 1, N'/audio/ejemplo1.mp3'),
    (N'Canción de prueba 2', 1, 1, 210, 2, N'/audio/ejemplo2.mp3'),
    (N'Otra canción', 2, 2, 195, 1, N'/audio/ejemplo3.mp3');
GO

-- Usuario de prueba (contraseña ejemplo: debe reemplazarse por hash real en tu app)
-- Hash ejemplo para "123456" con salt - en producción usar ASP.NET Identity o BCrypt
INSERT INTO Usuarios (NombreUsuario, Email, ContrasenaHash, NombreCompleto, Activo) VALUES
    (N'usuario', N'usuario@ejemplo.com', N'hash_cambiar_por_tu_app', N'Usuario Prueba', 1);
GO

-- =============================================================================
-- PROCEDIMIENTOS ALMACENADOS - AUTENTICACIÓN
-- =============================================================================

CREATE OR ALTER PROCEDURE sp_Usuario_Login
    @NombreUsuario   NVARCHAR(100),
    @ContrasenaHash NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, NombreUsuario, Email, NombreCompleto, Activo, FechaRegistro
    FROM Usuarios
    WHERE NombreUsuario = @NombreUsuario
      AND ContrasenaHash = @ContrasenaHash
      AND Activo = 1;
END;
GO

CREATE OR ALTER PROCEDURE sp_Usuario_ObtenerPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, NombreUsuario, Email, NombreCompleto, Activo, FechaRegistro
    FROM Usuarios
    WHERE Id = @Id AND Activo = 1;
END;
GO

-- =============================================================================
-- PROCEDIMIENTOS ALMACENADOS - CANCIONES (listado paginado y búsqueda)
-- =============================================================================

CREATE OR ALTER PROCEDURE sp_Canciones_Listar
    @Pagina         INT = 1,
    @FilasPorPagina INT = 10,
    @NombreFiltro  NVARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@Pagina - 1) * @FilasPorPagina;

    SELECT c.Id, c.Nombre AS NombreCancion, c.DuracionSegundos, c.NumeroPista, c.RutaArchivo,
           a.Id AS IdArtista, a.Nombre AS NombreArtista, a.Biografia AS BiografiaArtista, a.UrlImagen AS UrlImagenArtista,
           al.Id AS IdAlbum, al.Nombre AS NombreAlbum, al.Anio AS AnioAlbum, al.UrlPortada AS UrlPortadaAlbum
    FROM Canciones c
    INNER JOIN Artistas a ON c.IdArtista = a.Id
    INNER JOIN Albumes al ON c.IdAlbum = al.Id
    WHERE (@NombreFiltro IS NULL OR @NombreFiltro = '' OR c.Nombre LIKE N'%' + @NombreFiltro + N'%')
    ORDER BY c.Nombre
    OFFSET @Offset ROWS FETCH NEXT @FilasPorPagina ROWS ONLY;
END;
GO

CREATE OR ALTER PROCEDURE sp_Canciones_TotalRegistros
    @NombreFiltro NVARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) AS Total
    FROM Canciones
    WHERE (@NombreFiltro IS NULL OR @NombreFiltro = '' OR Nombre LIKE N'%' + @NombreFiltro + N'%');
END;
GO

CREATE OR ALTER PROCEDURE sp_Cancion_ObtenerPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.Id, c.Nombre AS NombreCancion, c.DuracionSegundos, c.NumeroPista, c.RutaArchivo, c.FechaCreacion,
           a.Id AS IdArtista, a.Nombre AS NombreArtista, a.Biografia AS BiografiaArtista, a.UrlImagen AS UrlImagenArtista,
           al.Id AS IdAlbum, al.Nombre AS NombreAlbum, al.Anio AS AnioAlbum, al.UrlPortada AS UrlPortadaAlbum
    FROM Canciones c
    INNER JOIN Artistas a ON c.IdArtista = a.Id
    INNER JOIN Albumes al ON c.IdAlbum = al.Id
    WHERE c.Id = @Id;
END;
GO

-- =============================================================================
-- PROCEDIMIENTOS ALMACENADOS - ARTISTAS (info al hacer clic en cantante)
-- =============================================================================

CREATE OR ALTER PROCEDURE sp_Artista_ObtenerPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Biografia, UrlImagen, FechaCreacion
    FROM Artistas
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Artista_Canciones_Listar
    @IdArtista INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.Id, c.Nombre AS NombreCancion, c.DuracionSegundos, c.NumeroPista, c.RutaArchivo,
           al.Id AS IdAlbum, al.Nombre AS NombreAlbum, al.Anio
    FROM Canciones c
    INNER JOIN Albumes al ON c.IdAlbum = al.Id
    WHERE c.IdArtista = @IdArtista
    ORDER BY al.Nombre, c.NumeroPista;
END;
GO

-- =============================================================================
-- PROCEDIMIENTOS ALMACENADOS - ÁLBUMES (lista de canciones al hacer clic en álbum)
-- =============================================================================

CREATE OR ALTER PROCEDURE sp_Album_ObtenerPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT al.Id, al.Nombre, al.Anio, al.UrlPortada, al.FechaCreacion,
           a.Id AS IdArtista, a.Nombre AS NombreArtista
    FROM Albumes al
    INNER JOIN Artistas a ON al.IdArtista = a.Id
    WHERE al.Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Album_Canciones_Listar
    @IdAlbum INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.Id, c.Nombre AS NombreCancion, c.DuracionSegundos, c.NumeroPista, c.RutaArchivo,
           a.Id AS IdArtista, a.Nombre AS NombreArtista
    FROM Canciones c
    INNER JOIN Artistas a ON c.IdArtista = a.Id
    WHERE c.IdAlbum = @IdAlbum
    ORDER BY c.NumeroPista;
END;
GO

-- =============================================================================
-- PROCEDIMIENTOS ALMACENADOS - PLAYLISTS (crear, editar, borrar, gestionar canciones)
-- =============================================================================

CREATE OR ALTER PROCEDURE sp_Playlists_ListarPorUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Descripcion, FechaCreacion
    FROM Playlists
    WHERE IdUsuario = @IdUsuario
    ORDER BY FechaCreacion DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Playlist_ObtenerPorId
    @Id        INT,
    @IdUsuario INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, IdUsuario, Nombre, Descripcion, FechaCreacion
    FROM Playlists
    WHERE Id = @Id
      AND (@IdUsuario IS NULL OR IdUsuario = @IdUsuario);
END;
GO

CREATE OR ALTER PROCEDURE sp_Playlist_Crear
    @IdUsuario   INT,
    @Nombre      NVARCHAR(200),
    @Descripcion NVARCHAR(500) = NULL,
    @NuevoId     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Playlists (IdUsuario, Nombre, Descripcion)
    VALUES (@IdUsuario, @Nombre, @Descripcion);
    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

CREATE OR ALTER PROCEDURE sp_Playlist_Actualizar
    @Id          INT,
    @Nombre      NVARCHAR(200),
    @Descripcion NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Playlists
    SET Nombre = @Nombre, Descripcion = @Descripcion
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Playlist_Eliminar
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Playlists WHERE Id = @Id;
END;
GO

-- Canciones de una playlist (para mostrar y para el reproductor)
CREATE OR ALTER PROCEDURE sp_PlaylistCanciones_Listar
    @IdPlaylist INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT pc.Id, pc.Orden, pc.FechaAgregado,
           c.Id AS IdCancion, c.Nombre AS NombreCancion, c.DuracionSegundos, c.RutaArchivo, c.NumeroPista,
           a.Id AS IdArtista, a.Nombre AS NombreArtista,
           al.Id AS IdAlbum, al.Nombre AS NombreAlbum
    FROM PlaylistCanciones pc
    INNER JOIN Canciones c ON pc.IdCancion = c.Id
    INNER JOIN Artistas a ON c.IdArtista = a.Id
    INNER JOIN Albumes al ON c.IdAlbum = al.Id
    WHERE pc.IdPlaylist = @IdPlaylist
    ORDER BY pc.Orden, pc.FechaAgregado;
END;
GO

CREATE OR ALTER PROCEDURE sp_PlaylistCanciones_Agregar
    @IdPlaylist INT,
    @IdCancion  INT,
    @Orden      INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Orden IS NULL
        SET @Orden = ISNULL((SELECT MAX(Orden) + 1 FROM PlaylistCanciones WHERE IdPlaylist = @IdPlaylist), 0);
    INSERT INTO PlaylistCanciones (IdPlaylist, IdCancion, Orden)
    VALUES (@IdPlaylist, @IdCancion, @Orden);
END;
GO

CREATE OR ALTER PROCEDURE sp_PlaylistCanciones_Quitar
    @IdPlaylist INT,
    @IdCancion  INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM PlaylistCanciones
    WHERE IdPlaylist = @IdPlaylist AND IdCancion = @IdCancion;
END;
GO

CREATE OR ALTER PROCEDURE sp_PlaylistCanciones_Reordenar
    @IdPlaylist INT,
    @IdCancion  INT,
    @NuevoOrden INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE PlaylistCanciones
    SET Orden = @NuevoOrden
    WHERE IdPlaylist = @IdPlaylist AND IdCancion = @IdCancion;
END;
GO

-- =============================================================================
-- FIN DEL SCRIPT
-- =============================================================================

PRINT N'Base de datos CostaRicaMusicBD creada correctamente (tablas y procedimientos almacenados).';
GO
