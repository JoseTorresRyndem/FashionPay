using FluentValidation;
using FashionPay.Application.DTOs.Proveedor;

namespace FashionPay.Application.Validators.Proveedor;

public class ProveedorCreateValidator : AbstractValidator<ProveedorCreateDto>
{
    public ProveedorCreateValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del proveedor es obligatorio")
            .Length(2, 100).WithMessage("El nombre debe tener entre 2 y 100 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s\-\.&0-9]+$").WithMessage("El nombre solo puede contener letras, números, espacios, guiones, puntos y &");

        RuleFor(x => x.Contacto)
            .MaximumLength(100).WithMessage("El contacto no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Contacto));

        RuleFor(x => x.Telefono)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("El teléfono solo puede contener números, espacios, guiones, paréntesis y signo más")
            .When(x => !string.IsNullOrEmpty(x.Telefono));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El formato del email no es válido")
            .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Direccion)
            .MaximumLength(200).WithMessage("La dirección no puede exceder 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Direccion));
    }
}