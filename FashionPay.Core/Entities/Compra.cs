﻿using System;
using System.Collections.Generic;

namespace FashionPay.Core.Entities;

public partial class Compra
{
    public int IdCompra { get; set; }

    public int IdCliente { get; set; }

    public string NumeroCompra { get; set; } = null!;

    public DateTime FechaCompra { get; set; }

    public decimal MontoTotal { get; set; }

    public int CantidadPagos { get; set; }

    public decimal MontoMensual { get; set; }

    public string? Observaciones { get; set; }

    public string? EstadoCompra { get; set; }

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual ICollection<PlanPago> PlanPagos { get; set; } = new List<PlanPago>();
}
