using FluentValidation;
using FashionPay.Application.DTOs.Proveedor;

namespace FashionPay.Application.Validators.Proveedor;

public class ProveedorFiltrosValidator : AbstractValidator<ProveedorFiltrosDto>
{
    public ProveedorFiltrosValidator()
    {
        RuleFor(x => x.Nombre)
            .MaximumLength(100).WithMessage("El filtro de nombre no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Nombre));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El formato del email no es válido para el filtro")
            .MaximumLength(100).WithMessage("El filtro de email no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Telefono)
            .MaximumLength(20).WithMessage("El filtro de teléfono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefono));

        RuleFor(x => x.FechaRegistroDesde)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha desde no puede ser futura")
            .When(x => x.FechaRegistroDesde.HasValue);

        RuleFor(x => x.FechaRegistroHasta)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha hasta no puede ser futura")
            .GreaterThanOrEqualTo(x => x.FechaRegistroDesde).WithMessage("La fecha hasta debe ser mayor o igual a la fecha desde")
            .When(x => x.FechaRegistroHasta.HasValue);
    }
}