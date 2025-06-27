using System;
using System.Collections.Generic;

namespace FashionPay.Core.Entities;

public partial class EstadoCuenta
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public string Clasificacion { get; set; } = null!;

    public decimal DeudaTotal { get; set; }

    public DateTime? FechaUltimoPago { get; set; }

    public int CantidadPagosVencidos { get; set; }

    public int DiasMaximoVencimiento { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;
}
