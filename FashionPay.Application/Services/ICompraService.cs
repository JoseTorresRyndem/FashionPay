using FashionPay.Application.DTOs.Compra;

namespace FashionPay.Application.Services;
public interface ICompraService
{
    Task<CompraResponseDto> CreatePurchaseAsync(CompraCreateDto purchaseDto);
    Task<CompraResponseDto?> GetPurchaseByIdAsync(int id);
    Task<IEnumerable<CompraResponseDto>> GetPurchasesAsync();
    Task<IEnumerable<CompraResponseDto>> GetPurchasesByClientAsync(int clientId);
    Task<IEnumerable<CompraResponseDto>> GetPurchasesWithFiltersAsync(CompraFiltrosDto filters);

}
