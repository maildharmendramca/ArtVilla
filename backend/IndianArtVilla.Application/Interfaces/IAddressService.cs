using IndianArtVilla.Application.DTOs;

namespace IndianArtVilla.Application.Interfaces;

public interface IAddressService
{
    Task<IEnumerable<AddressDto>> GetUserAddressesAsync(string userId);
    Task<AddressDto?> GetAddressAsync(int id, string userId);
    Task<AddressDto> CreateAddressAsync(CreateAddressDto dto, string userId);
    Task<AddressDto> UpdateAddressAsync(int id, CreateAddressDto dto, string userId);
    Task DeleteAddressAsync(int id, string userId);
    Task SetDefaultAddressAsync(int id, string userId);
}
