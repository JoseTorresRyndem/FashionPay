using System;
using System.Collections.Generic;

namespace FashionPay.Core.Entities;

public partial class Cliente
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public int DiaPago { get; set; }

    public int PeriodicidadPago { get; set; }

    public decimal LimiteCredito { get; set; }

    public int CantidadMaximaPagos { get; set; }

    public int ToleranciasMorosidad { get; set; }

    public DateTime FechaRegistro { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Abono> Abonos { get; set; } = new List<Abono>();

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual EstadoCuenta? EstadoCuenta { get; set; }
}
