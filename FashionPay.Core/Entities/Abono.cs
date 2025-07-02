using System;
using System.Collections.Generic;

namespace FashionPay.Core.Entities;

public partial class Abono
{
    public int Id { get; set; }

    public int IdCliente { get; set; }

    public int PlanPagoId { get; set; }

    public string NumeroRecibo { get; set; } = null!;

    public DateTime FechaAbono { get; set; }

    public decimal MontoAbono { get; set; }

    public string FormaPago { get; set; } = null!;

    public string? Observaciones { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual PlanPago PlanPago { get; set; } = null!;
}
