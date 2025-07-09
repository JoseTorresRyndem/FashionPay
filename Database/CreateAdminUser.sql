-- Script para crear usuario administrador con hash BCrypt correcto
-- Ejecutar después de CreateAuthTables.sql

USE FashionPayCore;
GO

-- Eliminar usuario admin si existe (para recrearlo con hash correcto)
DELETE FROM UserRole WHERE IdUser IN (SELECT IdUser FROM [User] WHERE Username = 'admin');
DELETE FROM [User] WHERE Username = 'admin';
GO

-- Crear usuario administrador con hash BCrypt para contraseña "Admin123!"
-- Hash generado con BCrypt para la contraseña: Admin123!
INSERT INTO [User] (Username, Password, Email, Active, CreatedAt, UpdatedAt) 
VALUES (
    'admin', 
    '$2a$11$rGKqV5i5O4KlBzvNGXhRfeFbRfAcWj8UVYBvF2LcvL6Qo8a3K4i8S',  -- Password: Admin123!
    'admin@fashionpay.com', 
    1, 
    GETDATE(), 
    GETDATE()
);
GO

-- Asignar rol Admin al usuario administrador
DECLARE @AdminUserId INT = (SELECT IdUser FROM [User] WHERE Username = 'admin');
DECLARE @AdminRoleId INT = (SELECT IdRole FROM [Role] WHERE Name = 'Admin');

INSERT INTO UserRole (IdUser, IdRole) VALUES (@AdminUserId, @AdminRoleId);
GO

-- Verificar que el usuario fue creado correctamente
SELECT 
    u.IdUser,
    u.Username,
    u.Email,
    u.Active,
    r.Name as RoleName
FROM [User] u
LEFT JOIN UserRole ur ON u.IdUser = ur.IdUser
LEFT JOIN [Role] r ON ur.IdRole = r.IdRole
WHERE u.Username = 'admin';
GO

PRINT 'Usuario administrador creado:';
PRINT 'Username: admin';
PRINT 'Password: Admin123!';
PRINT 'Email: admin@fashionpay.com';
PRINT '';
PRINT '¡IMPORTANTE: Cambiar esta contraseña inmediatamente en producción!';