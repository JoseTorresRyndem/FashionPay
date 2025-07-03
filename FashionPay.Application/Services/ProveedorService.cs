using AutoMapper;
using FashionPay.Core.Interfaces;
using FashionPay.Core.Entities;
using FashionPay.Application.DTOs.Proveedor;
using FashionPay.Application.Exceptions;

namespace FashionPay.Application.Services;

public class ProveedorService : IProveedorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProveedorService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProveedorResponseDto>> GetProveedoresAsync()
    {
        var proveedores = await _unitOfWork.Proveedores.GetAllAsync();
        var proveedoresDto = _mapper.Map<IEnumerable<ProveedorResponseDto>>(proveedores);

        // Enriquecer con información adicional
        foreach (var proveedorDto in proveedoresDto)
        {
            await EnriquecerProveedorAsync(proveedorDto);
        }

        return proveedoresDto.OrderBy(p => p.Nombre);
    }

    public async Task<ProveedorResponseDto?> GetProveedorByIdAsync(int id)
    {
        var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(id);
        if (proveedor == null) return null;

        var proveedorDto = _mapper.Map<ProveedorResponseDto>(proveedor);
        await EnriquecerProveedorAsync(proveedorDto);

        return proveedorDto;
    }

    public async Task<IEnumerable<ProveedorResponseDto>> GetProveedoresConFiltrosAsync(ProveedorFiltrosDto filtros)
    {
        var query = await _unitOfWork.Proveedores.GetAllAsync();

        // Aplicar filtros
        var proveedoresFiltrados = query.Where(p =>
            (string.IsNullOrEmpty(filtros.Nombre) || p.Nombre.Contains(filtros.Nombre, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(filtros.Email) || (!string.IsNullOrEmpty(p.Email) && p.Email.Contains(filtros.Email, StringComparison.OrdinalIgnoreCase))) &&
            (string.IsNullOrEmpty(filtros.Telefono) || (!string.IsNullOrEmpty(p.Telefono) && p.Telefono.Contains(filtros.Telefono))) &&
            (!filtros.Activo.HasValue || p.Activo == filtros.Activo.Value) &&
            (!filtros.FechaRegistroDesde.HasValue || p.FechaRegistro >= filtros.FechaRegistroDesde.Value) &&
            (!filtros.FechaRegistroHasta.HasValue || p.FechaRegistro <= filtros.FechaRegistroHasta.Value)
        );

        var resultado = _mapper.Map<IEnumerable<ProveedorResponseDto>>(proveedoresFiltrados);

        // Si se requieren solo proveedores con productos
        if (filtros.ConProductos)
        {
            var resultadoConProductos = new List<ProveedorResponseDto>();
            foreach (var proveedor in resultado)
            {
                await EnriquecerProveedorAsync(proveedor);
                if (proveedor.TotalProductos > 0)
                {
                    resultadoConProductos.Add(proveedor);
                }
            }
            return resultadoConProductos.OrderBy(p => p.Nombre);
        }

        // Enriquecer todos
        foreach (var proveedor in resultado)
        {
            await EnriquecerProveedorAsync(proveedor);
        }

        return resultado.OrderBy(p => p.Nombre);
    }

    public async Task<ProveedorResponseDto> CrearProveedorAsync(ProveedorCreateDto proveedorDto)
    {
        // Validaciones de negocio
        await ValidarProveedorAsync(proveedorDto);

        var proveedor = _mapper.Map<Proveedor>(proveedorDto);
        proveedor.FechaRegistro = DateTime.Now;
        proveedor.Activo = true;

        var proveedorCreado = await _unitOfWork.Proveedores.AddAsync(proveedor);
        await _unitOfWork.SaveChangesAsync();

        var resultado = _mapper.Map<ProveedorResponseDto>(proveedorCreado);
        await EnriquecerProveedorAsync(resultado);

        return resultado;
    }

    public async Task<ProveedorResponseDto> ActualizarProveedorAsync(int id, ProveedorUpdateDto proveedorDto)
    {
        var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(id);
        if (proveedor == null)
            throw new NotFoundException($"Proveedor con ID {id} no encontrado");

        // Validaciones de negocio
        await ValidarProveedorActualizacionAsync(proveedorDto, id);

        // Mapear cambios
        _mapper.Map(proveedorDto, proveedor);

        await _unitOfWork.Proveedores.UpdateAsync(proveedor);
        await _unitOfWork.SaveChangesAsync();

        var resultado = _mapper.Map<ProveedorResponseDto>(proveedor);
        await EnriquecerProveedorAsync(resultado);

        return resultado;
    }

    public async Task DesactivarProveedorAsync(int id)
    {
        var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(id);
        if (proveedor == null)
            throw new NotFoundException($"Proveedor con ID {id} no encontrado");

        // Validar que no tenga productos activos
        var productosActivos = await _unitOfWork.Proveedores.GetProveedorWithProductosAsync(id);
        if (productosActivos != null)
        {
            throw new BusinessException($"No se puede desactivar el proveedor porque tiene {productosActivos.Productos.Count()} productos activos. Desactive primero todos sus productos.");
        }

        proveedor.Activo = false;
        await _unitOfWork.Proveedores.UpdateAsync(proveedor);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ProveedorResponseDto> ReactivarProveedorAsync(int id)
    {
        var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(id);
        if (proveedor == null)
            throw new NotFoundException($"Proveedor con ID {id} no encontrado");

        proveedor.Activo = true;
        await _unitOfWork.Proveedores.UpdateAsync(proveedor);
        await _unitOfWork.SaveChangesAsync();

        var resultado = _mapper.Map<ProveedorResponseDto>(proveedor);
        await EnriquecerProveedorAsync(resultado);

        return resultado;
    }

    public async Task<bool> ExisteProveedorAsync(int id)
    {
        return await _unitOfWork.Proveedores.GetByIdAsync(id) != null;
    }

    public async Task<bool> ExisteProveedorPorNombreAsync(string nombre, int? excludeId = null)
    {
        var proveedores = await _unitOfWork.Proveedores.GetAllAsync();
        return proveedores.Any(p =>
            p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
            (!excludeId.HasValue || p.IdProveedor != excludeId.Value));
    }

    #region Métodos privados

    private async Task ValidarProveedorAsync(ProveedorCreateDto proveedorDto)
    {
        // Validar nombre único
        if (await ExisteProveedorPorNombreAsync(proveedorDto.Nombre))
            throw new BusinessException($"Ya existe un proveedor con el nombre '{proveedorDto.Nombre}'");

        // Validar email único si se proporciona
        if (!string.IsNullOrEmpty(proveedorDto.Email))
        {
            var proveedores = await _unitOfWork.Proveedores.GetAllAsync();
            if (proveedores.Any(p => !string.IsNullOrEmpty(p.Email) && p.Email.Equals(proveedorDto.Email, StringComparison.OrdinalIgnoreCase)))
                throw new BusinessException($"Ya existe un proveedor con el email '{proveedorDto.Email}'");
        }
    }

    private async Task ValidarProveedorActualizacionAsync(ProveedorUpdateDto proveedorDto, int id)
    {
        // Validar nombre único
        if (await ExisteProveedorPorNombreAsync(proveedorDto.Nombre, id))
            throw new BusinessException($"Ya existe otro proveedor con el nombre '{proveedorDto.Nombre}'");

        // Validar email único si se proporciona
        if (!string.IsNullOrEmpty(proveedorDto.Email))
        {
            var proveedores = await _unitOfWork.Proveedores.GetAllAsync();
            if (proveedores.Any(p => p.IdProveedor != id && !string.IsNullOrEmpty(p.Email) &&
                                    p.Email.Equals(proveedorDto.Email, StringComparison.OrdinalIgnoreCase)))
                throw new BusinessException($"Ya existe otro proveedor con el email '{proveedorDto.Email}'");
        }
    }

    private async Task EnriquecerProveedorAsync(ProveedorResponseDto proveedorDto)
    {
        var productos = await _unitOfWork.Productos.GetProductosByProveedorAsync(proveedorDto.IdProveedor);
        var productosLista = productos.ToList();

        proveedorDto.TotalProductos = productosLista.Count;
        proveedorDto.ValorInventario = productosLista.Sum(p => p.Precio * p.Stock);
        proveedorDto.UltimaActualizacion = DateTime.Now;
    }

    #endregion
}