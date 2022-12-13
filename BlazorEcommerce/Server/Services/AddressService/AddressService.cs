﻿using BlazorEcommerce.Server.Services.AuthService;
using BlazorEcommerce.Server.Data;
using BlazorEcommerce.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlazorEcommerce.Server.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly DataContext _context;
        private readonly IAuthService _authService;

        public AddressService(DataContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        public async Task<ServiceResponse<Address>> AddOrUpdateAddress(Address address)
        {
            var response = new ServiceResponse<Address>();
            var dbAddress = (await GetAddress()).Data;
            if (dbAddress == null)
            {
                address.UserId = _authService.GetUserId();
                _context.Addresses.Add(address);
                response.Data = address;
            }
            else
            {
                dbAddress.FirstName = address.FirstName;
                dbAddress.LastName = address.LastName;
                dbAddress.City = address.City;
                dbAddress.Country = address.Country;
                dbAddress.State = address.State;
                dbAddress.Street =  address.Street;
                dbAddress.Zip = address.Zip;

                response.Data = dbAddress;
            }

            
            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<ServiceResponse<Address>> GetAddress()
        {
            var userId = _authService.GetUserId();
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId);

            return new ServiceResponse<Address> { Data = address };
        }
    }
}
