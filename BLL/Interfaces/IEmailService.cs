﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string username, string resetToken, CancellationToken cancellationToken = default);
    }
}
