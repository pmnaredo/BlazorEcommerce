﻿using BlazorEcommerce.Server.Data;
using BlazorEcommerce.Shared;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BlazorEcommerce.Server.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private DataContext _context;

        public AuthService(DataContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            if (await UserExists(user.Email))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "User already exists."
                };
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            _ = await _context.SaveChangesAsync();

            return new ServiceResponse<int>
            {
                Data = user.Id,
                Message = "Registration successful!"
            };
        }

        public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email.ToLower()
                    .Equals(email.ToLower()));

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

    }
}
