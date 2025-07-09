using FashionPay.Application.DTOs.Auth;
using FluentValidation;

namespace FashionPay.Application.Validators.Auth;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El nombre de usuario es requerido")
            .Length(3, 50).WithMessage("El nombre de usuario debe tener entre 3 y 50 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");
    }
}