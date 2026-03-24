using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using IndianArtVilla.Core.Entities;
using IndianArtVilla.Core.Exceptions;
using IndianArtVilla.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IndianArtVilla.Infrastructure.Services;

public class AddressService : IAddressService
{
    private readonly IUnitOfWork _uow;

    public AddressService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<AddressDto>> GetUserAddressesAsync(string userId)
    {
        return await _uow.Addresses.Query()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.Id)
            .Select(a => MapToDto(a))
            .ToListAsync();
    }

    public async Task<AddressDto?> GetAddressAsync(int id, string userId)
    {
        var address = await _uow.Addresses
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        return address == null ? null : MapToDto(address);
    }

    public async Task<AddressDto> CreateAddressAsync(CreateAddressDto dto, string userId)
    {
        if (dto.IsDefault)
            await ClearDefaultAsync(userId);

        var address = new Address
        {
            UserId = userId,
            FullName = dto.FullName,
            Phone = dto.Phone,
            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2,
            City = dto.City,
            State = dto.State,
            PinCode = dto.PinCode,
            Country = dto.Country,
            IsDefault = dto.IsDefault,
            Type = dto.Type,
        };

        _uow.Addresses.Add(address);
        await _uow.SaveChangesAsync();

        return MapToDto(address);
    }

    public async Task<AddressDto> UpdateAddressAsync(int id, CreateAddressDto dto, string userId)
    {
        var address = await _uow.Addresses
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId)
            ?? throw new NotFoundException("Address", id);

        if (dto.IsDefault && !address.IsDefault)
            await ClearDefaultAsync(userId);

        address.FullName = dto.FullName;
        address.Phone = dto.Phone;
        address.AddressLine1 = dto.AddressLine1;
        address.AddressLine2 = dto.AddressLine2;
        address.City = dto.City;
        address.State = dto.State;
        address.PinCode = dto.PinCode;
        address.Country = dto.Country;
        address.IsDefault = dto.IsDefault;
        address.Type = dto.Type;

        await _uow.SaveChangesAsync();

        return MapToDto(address);
    }

    public async Task DeleteAddressAsync(int id, string userId)
    {
        var address = await _uow.Addresses
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId)
            ?? throw new NotFoundException("Address", id);

        _uow.Addresses.Remove(address);
        await _uow.SaveChangesAsync();
    }

    public async Task SetDefaultAddressAsync(int id, string userId)
    {
        var address = await _uow.Addresses
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId)
            ?? throw new NotFoundException("Address", id);

        await ClearDefaultAsync(userId);
        address.IsDefault = true;
        await _uow.SaveChangesAsync();
    }

    private async Task ClearDefaultAsync(string userId)
    {
        var defaults = await _uow.Addresses.Query()
            .Where(a => a.UserId == userId && a.IsDefault)
            .ToListAsync();

        foreach (var a in defaults) a.IsDefault = false;
    }

    private static AddressDto MapToDto(Address a) => new()
    {
        Id = a.Id,
        FullName = a.FullName,
        Phone = a.Phone,
        AddressLine1 = a.AddressLine1,
        AddressLine2 = a.AddressLine2,
        City = a.City,
        State = a.State,
        PinCode = a.PinCode,
        Country = a.Country,
        IsDefault = a.IsDefault,
        Type = a.Type
    };
}
