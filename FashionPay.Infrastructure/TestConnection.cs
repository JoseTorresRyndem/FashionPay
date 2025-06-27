using FashionPay.Core.Data;
using FashionPay.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FashionPay.Infrastructure;

public class TestConnection
{
    public static void TestCorePackage()
    {
        // Probar que podemos crear el contexto
        var options = new DbContextOptionsBuilder<FashionPayContext>()
            .UseSqlServer("Server=RYNL044\\JOSETRYNDEMSQL;Database=FashionPay;User Id=sa;Password=root;TrustServerCertificate=true;")
            .Options;

        using var context = new FashionPayContext(options);

        // Probar que podemos acceder a las entidades
        Console.WriteLine("✅ FashionPayContext creado correctamente");
        Console.WriteLine($"✅ DbSet Clientes: {context.Clientes != null}");
        Console.WriteLine($"✅ DbSet Productos: {context.Productos != null}");
        Console.WriteLine($"✅ DbSet Compras: {context.Compras != null}");

        // Probar conexión a BD (opcional)
        try
        {
            var canConnect = context.Database.CanConnect();
            Console.WriteLine($"✅ Conexión a BD: {(canConnect ? "EXITOSA" : "FALLIDA")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error de conexión: {ex.Message}");
        }
    }
}