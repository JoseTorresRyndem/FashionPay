﻿namespace FashionPay.Application.DTOs.Compra;

public class DetalleCompraDto
{
    public int IdDetalleCompra { get; set; }
    public int IdProducto { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public string ProductoCodigo { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}