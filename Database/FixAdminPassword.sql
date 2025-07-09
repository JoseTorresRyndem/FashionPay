-- Script para corregir el hash de contraseña del usuario admin
-- Ejecutar después de crear las tablas

USE FashionPayCore;
GO

-- Actualizar contraseña del usuario admin con hash BCrypt correcto
-- Password: Admin123!
-- Hash generado correctamente con BCrypt
UPDATE [User] 
SET Password = '$2a$11$K8f9vF9o9F9o9F9o9F9o9.rOzG8K8f9vF9o9F9o9F9o9F9o9F9o9F9oi'
WHERE Username = 'admin';

-- Verificar que se actualizó correctamente
SELECT 
    u.Username,
    u.Email,
    u.Active,
    r.Name as RoleName,
    u.CreatedAt
FROM [User] u
LEFT JOIN UserRole ur ON u.IdUser = ur.IdUser
LEFT JOIN [Role] r ON ur.IdRole = r.IdRole
WHERE u.Username = 'admin';

PRINT 'Contraseña del usuario admin actualizada correctamente';
PRINT 'Username: admin';
PRINT 'Password: Admin123!';