using Microsoft.EntityFrameworkCore;
using FashionPay.Core.Data;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;

namespace FashionPay.Infrastructure.Repositories;

public class EstadoCuentaRepository : BaseRepository<EstadoCuenta>, IEstadoCuentaRepository
{
    public EstadoCuentaRepository(FashionPayContext context) : base(context)
    {
    }
}