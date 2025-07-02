using FluentValidation;
using FashionPay.Application.DTOs.Compra;

namespace FashionPay.Application.Validators.Compra;

public class CompraCreateValidator : AbstractValidator<CompraCreateDto>
{
    public CompraCreateValidator()
    {
        RuleFor(x => x.IdCliente)
            .GreaterThan(0).WithMessage("Debe seleccionar un cliente válido");

        RuleFor(x => x.CantidadPagos)
            .GreaterThan(0).WithMessage("La cantidad de pagos debe ser mayor a 0")
            .LessThanOrEqualTo(60).WithMessage("La cantidad de pagos no puede exceder 60");

        RuleFor(x => x.Detalles)
            .NotEmpty().WithMessage("Debe incluir al menos un producto en la compra")
            .Must(DetallesNoDuplicados).WithMessage("No puede incluir el mismo producto múltiples veces");

        RuleForEach(x => x.Detalles).SetValidator(new DetalleCompraCreateValidator());
    }

    private bool DetallesNoDuplicados(List<DetalleCompraCreateDto> detalles)
    {
        var productosIds = detalles.Select(d => d.IdProducto).ToList();
        return productosIds.Count == productosIds.Distinct().Count();
    }
}