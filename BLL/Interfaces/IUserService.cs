﻿using BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDTO> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default);
        Task UpdateUserProfileAsync(Guid userId, UserUpdateDTO dto, CancellationToken cancellationToken = default);
    }
}
