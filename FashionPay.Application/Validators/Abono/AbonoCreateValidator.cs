using FluentValidation;
using FashionPay.Application.DTOs.Abono;

namespace FashionPay.Application.Validators.Abono;

public class AbonoCreateValidator : AbstractValidator<AbonoCreateDto>
{
    public AbonoCreateValidator()
    {
        RuleFor(x => x.ClienteId)
            .GreaterThan(0).WithMessage("Debe seleccionar un cliente válido");

        RuleFor(x => x.MontoAbono)
            .GreaterThan(0).WithMessage("El monto del abono debe ser mayor a 0")
            .LessThanOrEqualTo(999999.99m).WithMessage("El monto del abono no puede exceder $999,999.99");

        RuleFor(x => x.FormaPago)
            .NotEmpty().WithMessage("La forma de pago es obligatoria")
            .Must(formaPago => new[] { "EFECTIVO", "TRANSFERENCIA", "TARJETA" }.Contains(formaPago.ToUpper()))
            .WithMessage("La forma de pago debe ser: EFECTIVO, TRANSFERENCIA o TARJETA");

        RuleFor(x => x.Observaciones)
            .MaximumLength(300).WithMessage("Las observaciones no pueden exceder 300 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observaciones));
    }
}