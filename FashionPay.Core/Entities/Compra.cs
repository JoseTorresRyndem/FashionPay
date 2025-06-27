using System;
using System.Collections.Generic;

namespace FashionPay.Core.Entities;

public partial class Compra
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public string NumeroCompra { get; set; } = null!;

    public DateTime FechaCompra { get; set; }

    public decimal MontoTotal { get; set; }

    public int CantidadPagos { get; set; }

    public decimal MontoMensual { get; set; }

    public string? Observaciones { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual ICollection<PlanPago> PlanPagos { get; set; } = new List<PlanPago>();
}
