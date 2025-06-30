

using FashionPay.Application.DTOs.Cliente;
using FashionPay.Core.Interfaces;
using FluentValidation;

namespace FashionPay.Application.Validators.Cliente;

public class ClienteCreateValidator : AbstractValidator<ClienteCreateDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public ClienteCreateValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(c => c.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .Length(2, 100).WithMessage("El nombre debe tener entre 2 y 100 caracteres.")
            .Matches(@"^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$").WithMessage("El nombre solo puede contener letras y espacios");

        RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio")
                .EmailAddress().WithMessage("El formato del email es inválido")
                .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres");

        RuleFor(x => x.Telefono)
            .Matches(@"^\d{3}-\d{3}-\d{4}$").WithMessage("El teléfono debe tener el formato: 777-123-4567")
            .When(x => !string.IsNullOrEmpty(x.Telefono));

        RuleFor(x => x.Direccion)
            .MaximumLength(200).WithMessage("La dirección no puede exceder 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Direccion));

        RuleFor(x => x.DiaPago)
            .InclusiveBetween(1, 31).WithMessage("El día de pago debe estar entre 1 y 31");

        RuleFor(x => x.LimiteCredito)
            .GreaterThan(0).WithMessage("El límite de crédito debe ser mayor a 0")
            .LessThanOrEqualTo(100000).WithMessage("El límite de crédito no puede exceder $100,000");

        RuleFor(x => x.CantidadMaximaPagos)
            .InclusiveBetween(1, 60).WithMessage("La cantidad máxima de pagos debe estar entre 1 y 60");

        RuleFor(x => x.ToleranciasMorosidad)
            .GreaterThanOrEqualTo(0).WithMessage("La tolerancia de morosidad no puede ser negativa")
            .LessThanOrEqualTo(30).WithMessage("La tolerancia de morosidad no puede exceder 30 días");
    }
}

