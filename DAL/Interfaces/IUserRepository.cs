using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<User> UpdatePasswordAsync(Guid userId, string newPasswordHash, CancellationToken cancellationToken = default);

    }
}
