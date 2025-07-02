using System;
using System.Collections.Generic;

namespace FashionPay.Core.Entities;

public partial class PlanPago
{
    public int Id { get; set; }

    public int IdCompra { get; set; }

    public int NumeroPago { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public decimal MontoProgramado { get; set; }

    public decimal MontoPagado { get; set; }

    public decimal SaldoPendiente { get; set; }

    public string Estado { get; set; } = null!;

    public int? DiasVencidos { get; set; }

    public virtual ICollection<Abono> Abonos { get; set; } = new List<Abono>();

    public virtual Compra Compra { get; set; } = null!;
}
