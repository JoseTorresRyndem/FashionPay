using System;
using System.Collections.Generic;

namespace FashionPay.Core.Entities;

public partial class EstadoCuenta
{
    public int IdEstadoCuenta { get; set; }

    public int IdCliente { get; set; }

    public string Clasificacion { get; set; } = null!;

    public decimal DeudaTotal { get; set; }

    public DateTime? FechaUltimoPago { get; set; }

    public int CantidadPagosVencidos { get; set; }

    public int DiasMaximoVencimiento { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}
