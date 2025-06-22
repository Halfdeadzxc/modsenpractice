using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class UserUpdateDTO
    {
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public string Bio { get; set; }
        public bool IsPrivate { get; set; }
    }
}
