using FashionPay.Application.DTOs.Compra;

namespace FashionPay.Application.Services;
public interface ICompraService
{
    Task<CompraResponseDto> CrearCompraAsync(CompraCreateDto compraDto);
    Task<CompraResponseDto?> GetCompraByIdAsync(int id);
    Task<IEnumerable<CompraResponseDto>> GetComprasAsync();
    Task<IEnumerable<CompraResponseDto>> GetComprasByClienteAsync(int clienteId);
    Task<IEnumerable<CompraResponseDto>> GetComprasConFiltrosAsync(CompraFiltrosDto filtros);
}
