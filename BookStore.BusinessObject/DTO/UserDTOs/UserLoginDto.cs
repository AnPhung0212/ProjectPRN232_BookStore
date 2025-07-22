using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BusinessObject.DTO.UserDTOs
{
    public class UserLoginDto
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class UserResponseDto
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
    }

}
