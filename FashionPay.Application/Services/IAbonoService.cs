using FashionPay.Application.DTOs.Abono;
using FashionPay.Core.Entities;

namespace FashionPay.Application.Services;

public interface IAbonoService
{
    Task<AbonoResponseDto> RegisterPaymentAsync(AbonoCreateDto paymentDto);
    Task<AbonoResponseDto?> GetPaymentByIdAsync(int id);
    Task<IEnumerable<AbonoResponseDto>> GetPaymentsByClientAsync(int clientId);
    Task<IEnumerable<AbonoResponseDto>> GetPaymentsWithFiltersAsync(AbonoFiltrosDto filters);
    Task<ResumenPagosClienteDto> GetClientPaymentSummaryAsync(int clientId);

}
