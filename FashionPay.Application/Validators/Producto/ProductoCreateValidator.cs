using FluentValidation;
using FashionPay.Application.DTOs.Producto;
using FashionPay.Core.Interfaces;

namespace FashionPay.Application.Validators.Producto;

public class ProductoCreateValidator : AbstractValidator<ProductoCreateDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductoCreateValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.ProveedorId)
           .GreaterThan(0).WithMessage("Debe seleccionar un proveedor válido")
           .MustAsync(ProveedorDebeExistir).WithMessage("El proveedor seleccionado no existe");

        RuleFor(x => x.Codigo)
          .NotEmpty().WithMessage("El código es obligatorio")
          .Length(3, 50).WithMessage("El código debe tener entre 3 y 50 caracteres")
          .Matches(@"^[A-Z0-9-]+$").WithMessage("El código solo puede contener letras mayúsculas, números y guiones")
          .MustAsync(CodigoDebeSerUnico).WithMessage("Ya existe un producto con este código");

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
    private async Task<bool> ProveedorDebeExistir(int proveedorId, CancellationToken cancellationToken)
    {
        var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(proveedorId);
        return proveedor != null && proveedor.Activo;
    }

    private async Task<bool> CodigoDebeSerUnico(string codigo, CancellationToken cancellationToken)
    {
        var productoExistente = await _unitOfWork.Productos.GetByCodigoAsync(codigo);
        return productoExistente == null;
    }
}