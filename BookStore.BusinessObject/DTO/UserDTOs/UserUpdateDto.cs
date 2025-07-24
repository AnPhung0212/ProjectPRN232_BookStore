using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BusinessObject.DTO.UserDTOs
{
    public class UserUpdateDto
    {
        public string Username { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Address { get; set; }

        public string Password { get; set; } = null!;
        public string? NewPassword { get; set; }
        public string? ReconfirmPassword { get; set; }
    }
}
