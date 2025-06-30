using FluentValidation;
using FashionPay.Application.DTOs.Producto;

namespace FashionPay.Application.Validators.Producto;

public class ProductoCreateValidator : AbstractValidator<ProductoCreateDto>
{
    public ProductoCreateValidator()
    {
        RuleFor(x => x.ProveedorId)
            .GreaterThan(0).WithMessage("Debe seleccionar un proveedor válido");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es obligatorio")
            .Length(3, 50).WithMessage("El código debe tener entre 3 y 50 caracteres")
            .Matches(@"^[A-Z0-9-]+$").WithMessage("El código solo puede contener letras mayúsculas, números y guiones");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio")
            .Length(2, 100).WithMessage("El nombre debe tener entre 2 y 100 caracteres");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descripcion));

        RuleFor(x => x.Precio)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0")
            .LessThanOrEqualTo(999999.99m).WithMessage("El precio no puede exceder $999,999.99");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo")
            .LessThanOrEqualTo(99999).WithMessage("El stock no puede exceder 99,999 unidades");
    }
}