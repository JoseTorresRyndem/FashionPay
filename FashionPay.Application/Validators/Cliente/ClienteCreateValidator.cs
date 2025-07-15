

using FashionPay.Application.DTOs.Cliente;
using FashionPay.Application.Common;
using FashionPay.Core.Interfaces;
using FluentValidation;

namespace FashionPay.Application.Validators.Cliente;

public class ClienteCreateValidator : AbstractValidator<ClienteCreateDto>
{
    public ClienteCreateValidator()
    {
        RuleFor(c => c.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .Length(BusinessConstants.Validation.MIN_NAME_LENGTH, BusinessConstants.Validation.MAX_NAME_LENGTH)
            .WithMessage($"El nombre debe tener entre {BusinessConstants.Validation.MIN_NAME_LENGTH} y {BusinessConstants.Validation.MAX_NAME_LENGTH} caracteres.")
            .Matches(@"^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$").WithMessage("El nombre solo puede contener letras y espacios");

        RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio")
                .EmailAddress().WithMessage("El formato del email es inválido")
                .Length(BusinessConstants.Validation.MIN_EMAIL_LENGTH, BusinessConstants.Validation.MAX_EMAIL_LENGTH)
                .WithMessage($"El email debe tener entre {BusinessConstants.Validation.MIN_EMAIL_LENGTH} y {BusinessConstants.Validation.MAX_EMAIL_LENGTH} caracteres");

        RuleFor(x => x.Telefono)
            .Matches(@"^\d{3}-\d{3}-\d{4}$").WithMessage("El teléfono debe tener el formato: 000-000-0000")
            .When(x => !string.IsNullOrEmpty(x.Telefono));

        RuleFor(x => x.Direccion)
            .MaximumLength(BusinessConstants.Validation.MAX_ADDRESS_LENGTH)
            .WithMessage($"La dirección no puede exceder {BusinessConstants.Validation.MAX_ADDRESS_LENGTH} caracteres")
            .When(x => !string.IsNullOrEmpty(x.Direccion));

        RuleFor(x => x.DiaPago)
            .InclusiveBetween(1, 31).WithMessage("El día de pago debe estar entre 1 y 31");

        RuleFor(x => x.LimiteCredito)
            .InclusiveBetween(BusinessConstants.Client.MIN_CREDIT_LIMIT, BusinessConstants.Client.MAX_CREDIT_LIMIT)
            .WithMessage($"El límite de crédito debe estar entre ${BusinessConstants.Client.MIN_CREDIT_LIMIT:N0} y ${BusinessConstants.Client.MAX_CREDIT_LIMIT:N0}");

        RuleFor(x => x.CantidadMaximaPagos)
            .InclusiveBetween(BusinessConstants.Client.MIN_PAYMENTS, BusinessConstants.Client.MAX_PAYMENTS)
            .WithMessage($"La cantidad máxima de pagos debe estar entre {BusinessConstants.Client.MIN_PAYMENTS} y {BusinessConstants.Client.MAX_PAYMENTS}");

        RuleFor(x => x.ToleranciasMorosidad)
            .GreaterThanOrEqualTo(0).WithMessage("La tolerancia de morosidad no puede ser negativa")
            .LessThanOrEqualTo(30).WithMessage("La tolerancia de morosidad no puede exceder 30 días");
    }
}

