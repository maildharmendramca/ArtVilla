using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndianArtVilla.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<AddressDto>>>> GetAddresses()
    {
        var addresses = await _addressService.GetUserAddressesAsync(UserId);
        return Ok(ApiResponse<IEnumerable<AddressDto>>.Ok(addresses));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<AddressDto>>> GetAddress(int id)
    {
        var address = await _addressService.GetAddressAsync(id, UserId);
        if (address is null)
            return NotFound(ApiResponse<AddressDto>.Fail("Address not found."));

        return Ok(ApiResponse<AddressDto>.Ok(address));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AddressDto>>> CreateAddress([FromBody] CreateAddressDto dto)
    {
        var address = await _addressService.CreateAddressAsync(dto, UserId);
        return CreatedAtAction(nameof(GetAddress),
            new { id = address.Id },
            ApiResponse<AddressDto>.Ok(address, "Address added."));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<AddressDto>>> UpdateAddress(int id, [FromBody] CreateAddressDto dto)
    {
        var address = await _addressService.UpdateAddressAsync(id, dto, UserId);
        return Ok(ApiResponse<AddressDto>.Ok(address, "Address updated."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteAddress(int id)
    {
        await _addressService.DeleteAddressAsync(id, UserId);
        return Ok(ApiResponse<string>.Ok("Address deleted."));
    }

    [HttpPatch("{id}/default")]
    public async Task<ActionResult<ApiResponse<string>>> SetDefaultAddress(int id)
    {
        await _addressService.SetDefaultAddressAsync(id, UserId);
        return Ok(ApiResponse<string>.Ok("Default address updated."));
    }
}
