using FluentValidation;
using FashionPay.Application.DTOs.Compra;

namespace FashionPay.Application.Validators.Compra;

public class DetalleCompraCreateValidator : AbstractValidator<DetalleCompraCreateDto>
{
    public DetalleCompraCreateValidator()
    {
        RuleFor(x => x.ProductoId)
            .GreaterThan(0).WithMessage("Debe seleccionar un producto válido");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0")
            .LessThanOrEqualTo(999).WithMessage("La cantidad no puede exceder 999 unidades");

        RuleFor(x => x.PrecioUnitario)
            .GreaterThan(0).WithMessage("El precio unitario debe ser mayor a 0")
            .LessThanOrEqualTo(999999.99m).WithMessage("El precio unitario no puede exceder $999,999.99");
    }
}
