using System;
using System.Collections.Generic;

namespace FashionPay.Core.Entities;

public partial class Producto
{
    public int Id { get; set; }

    public int ProveedorId { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public DateTime FechaRegistro { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual Proveedor Proveedor { get; set; } = null!;
}
