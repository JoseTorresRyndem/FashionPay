using FashionPay.Application.DTOs.Abono;
using FashionPay.Core.Entities;

namespace FashionPay.Application.Services;

public interface IAbonoService
{
    Task<AbonoResponseDto> RegistrarAbonoAsync(AbonoCreateDto abonoDto);
    Task<AbonoResponseDto?> GetAbonoByIdAsync(int id);
    Task<IEnumerable<AbonoResponseDto>> GetAbonosByClienteAsync(int clienteId);
    Task<IEnumerable<AbonoResponseDto>> GetAbonosConFiltrosAsync(AbonoFiltrosDto filtros);
    Task<ResumenPagosClienteDto> GetResumenPagosClienteAsync(int clienteId);
}
