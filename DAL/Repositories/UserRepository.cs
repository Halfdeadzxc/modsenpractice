﻿using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task<User> UpdatePasswordAsync(Guid userId, string newPasswordHash, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
            if (user != null)
            {
                user.PasswordHash = newPasswordHash;
                await _context.SaveChangesAsync(cancellationToken);
            }
            return user;
        }

        public async Task<User> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
        }
    }
}
