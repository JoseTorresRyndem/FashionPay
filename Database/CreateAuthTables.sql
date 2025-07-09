-- Script para crear las tablas de autenticación en FashionPay
-- Ejecutar en SQL Server Management Studio o Azure Data Studio

USE FashionPayCore;
GO

-- 1. Crear tabla User
CREATE TABLE [User] (
    IdUser INT IDENTITY(1,1) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Active BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_User PRIMARY KEY (IdUser),
    CONSTRAINT UQ_User_Username UNIQUE (Username),
    CONSTRAINT UQ_User_Email UNIQUE (Email)
);
GO

-- 2. Crear tabla Role
CREATE TABLE [Role] (
    IdRole INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    
    CONSTRAINT PK_Role PRIMARY KEY (IdRole),
    CONSTRAINT UQ_Role_Name UNIQUE (Name)
);
GO

-- 3. Crear tabla UserRole (tabla intermedia)
CREATE TABLE UserRole (
    IdUser INT NOT NULL,
    IdRole INT NOT NULL,
    
    CONSTRAINT PK_UserRole PRIMARY KEY (IdUser, IdRole),
    CONSTRAINT FK_UserRole_User FOREIGN KEY (IdUser) REFERENCES [User](IdUser) ON DELETE CASCADE,
    CONSTRAINT FK_UserRole_Role FOREIGN KEY (IdRole) REFERENCES [Role](IdRole) ON DELETE CASCADE
);
GO

-- Crear índices para mejorar el rendimiento
CREATE INDEX IX_User_Username ON [User](Username);
CREATE INDEX IX_User_Email ON [User](Email);
CREATE INDEX IX_User_Active ON [User](Active);
CREATE INDEX IX_Role_Name ON [Role](Name);
CREATE INDEX IX_UserRole_User ON UserRole(IdUser);
CREATE INDEX IX_UserRole_Role ON UserRole(IdRole);
GO

-- Insertar roles básicos
INSERT INTO [Role] (Name) VALUES 
('Admin'),
('User'),
('Manager');
GO

-- Crear usuario administrador por defecto (password: Admin123!)
-- Nota: En producción, cambiar esta contraseña inmediatamente
INSERT INTO [User] (Username, Password, Email, Active, CreatedAt, UpdatedAt) 
VALUES ('admin', '$2a$11$rOzG8K8f9vF9o9F9o9F9o.K8f9vF9o9F9o9F9o9F9o9F9o9F9o9F9oi', 'admin@fashionpay.com', 1, GETDATE(), GETDATE());
GO

-- Asignar rol Admin al usuario administrador
DECLARE @AdminUserId INT = (SELECT IdUser FROM [User] WHERE Username = 'admin');
DECLARE @AdminRoleId INT = (SELECT IdRole FROM [Role] WHERE Name = 'Admin');

INSERT INTO UserRole (IdUser, IdRole) VALUES (@AdminUserId, @AdminRoleId);
GO

-- Crear trigger para actualizar UpdatedAt automáticamente
CREATE TRIGGER tr_User_UpdatedAt
ON [User]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [User] 
    SET UpdatedAt = GETDATE()
    WHERE IdUser IN (SELECT IdUser FROM inserted);
END;
GO

PRINT 'Tablas de autenticación creadas exitosamente';
PRINT 'Usuario admin creado - Cambiar contraseña en producción';
PRINT 'Roles básicos: Admin, User, Manager';