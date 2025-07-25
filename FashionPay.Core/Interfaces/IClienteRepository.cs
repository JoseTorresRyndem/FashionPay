﻿using FashionPay.Core.Entities;
namespace FashionPay.Core.Interfaces;

public interface IClienteRepository : IBaseRepository<Cliente>
{
    Task<Cliente?> GetByEmailAsync(string email);
    Task<decimal> GetTotalDebtAsync(int clientId);
    Task<EstadoCuenta?> GetAccountStatusAsync(int clientId);
    Task<IEnumerable<Cliente>> GetClientsByClassificationAsync(string classification);
    Task ExecuteCalculateBalanceAsync(int clientId);
    Task<Cliente?> GetByIdWithAccountAsync(int id);

}