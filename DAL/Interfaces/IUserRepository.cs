using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid userId);
        Task<User> GetByEmailAsync(string email);
        Task UpdateAsync(User user);
        Task<User> AddAsync(User user);
        Task<bool> UpdatePasswordAsync(Guid userId, string newPasswordHash);
        Task<User> GetByRefreshTokenAsync(string refreshToken);


    }
}
